using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class RageBlueBird : AttackSkillTemplate
	{
		protected float m_PurgeChance;

		protected float m_StunChance;

		protected float m_StunDuration;

		protected bool m_UseSkillAssetId;

		private float m_DamageMod;

		private ICombatant m_secondTarget;

		private ICombatant m_thirdTarget;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("purge_chance", out m_PurgeChance);
			base.Model.SkillParameters.TryGetValue("stun_chance", out m_StunChance);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageMod);
			base.Model.SkillParameters.TryGetValue("stun_duration", out m_StunDuration);
			m_UseSkillAssetId = base.Model.SkillParameters.ContainsKey("use_skill_asset");
			base.Init(model);
			m_FallBackTime *= 4f;
			m_AttackAnimation = (ICombatant c) => c.CombatantView.PlayRageSkillAnimation();
			ActionsAfterTargetSelection.Add(delegate(BattleGameData battle, ICombatant source, ICombatant target)
			{
				List<ICombatant> list2 = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c.IsParticipating).ToList();
				ICombatant item = list2[UnityEngine.Random.Range(0, list2.Count)];
				m_Targets = new List<ICombatant> { item };
				source.AttackTarget = m_Targets.FirstOrDefault();
				return 0f;
			});
			ActionsBeforeDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				List<ICombatant> list = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c.IsParticipating).ToList();
				if (list.Count > 0)
				{
					m_secondTarget = list[UnityEngine.Random.Range(0, list.Count)];
					m_thirdTarget = list[UnityEngine.Random.Range(0, list.Count)];
					if (m_StunChance / 100f > UnityEngine.Random.value)
					{
						DebugLog.Log("Stunned");
						m_secondTarget.CombatantView.PlayStunnedAnimation();
						List<float> values = new List<float> { 1f };
						BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, m_secondTarget, new List<BattleEffect>
						{
							new BattleEffect
							{
								EffectTrigger = EffectTriggerType.BeforeStartOfTurn,
								EffectType = BattleEffectType.Stun,
								AfflicionType = SkillEffectTypes.Curse,
								Values = values,
								Duration = (int)m_StunDuration,
								EffectAssetId = ((!m_UseSkillAssetId) ? "Stun" : base.Model.Balancing.EffectIconAssetId),
								EffectAtlasId = ((!m_UseSkillAssetId) ? "Skills_Generic" : base.Model.Balancing.EffectIconAtlasId)
							}
						}, (int)m_StunDuration, battle, "Stun", SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
						battleEffectGameData.AddEffect();
						battleEffectGameData.EffectRemovedAction = delegate(BattleEffectGameData e)
						{
							e.m_Target.CombatantView.PlayIdle();
						};
					}
					if (UnityEngine.Random.value <= m_PurgeChance / 100f)
					{
						int num = DIContainerLogic.GetBattleService().RemoveBattleEffects(m_thirdTarget, SkillEffectTypes.Blessing);
						VisualEffectSetting setting = null;
						if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("Purge", out setting))
						{
							SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { m_thirdTarget });
						}
					}
				}
			});
			m_ApplyPerks = false;
			m_IsMelee = false;
			m_UseFocusPosition = true;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			List<ICombatant> aliveEnemiesBefore = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive).ToList();
			battle.SetRageAvailable(source.CombatantFaction, false);
			yield return DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(base.DoAction(battle, source, target, shared, false));
			if (source.CombatantFaction == Faction.Pigs)
			{
				battle.SetFactionRage(Faction.Pigs, 0f);
				battle.RegisterRageUsed(100f, source);
			}
			battle.SetRageAvailable(source.CombatantFaction, true);
			if (battle.IsPvP && source.CombatantFaction == Faction.Birds && !battle.IsUnranked)
			{
				CalculateRageKills(battle, aliveEnemiesBefore);
			}
		}

		private void CalculateRageKills(BattleGameData battle, List<ICombatant> aliveEnemiesBefore)
		{
			List<ICombatant> list = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive).ToList();
			foreach (ICombatant item in list)
			{
				aliveEnemiesBefore.Remove(item);
			}
			if (aliveEnemiesBefore.Count <= 0)
			{
				return;
			}
			foreach (ICombatant item2 in aliveEnemiesBefore)
			{
				DIContainerLogic.GetPvpObjectivesService().RageUsedToKill(item2);
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_6}", m_StunChance.ToString("0"));
			dictionary.Add("{value_2}", m_StunDuration.ToString("0"));
			dictionary.Add("{value_3}", m_PurgeChance.ToString("0"));
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack / 100f);
			dictionary.Add("{value_1}", string.Empty + num);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
