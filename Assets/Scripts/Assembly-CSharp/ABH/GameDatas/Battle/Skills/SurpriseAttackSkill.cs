using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SurpriseAttackSkill : AttackSkillTemplate
	{
		protected float m_PurgeChance;

		protected float m_StunChance;

		protected float m_StunDuration;

		protected bool m_UseSkillAssetId;

		private float m_DamageMod;

		private float m_ChargeTurns;

		private ICombatant m_secondTarget;

		private ICombatant m_thirdTarget;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("purge_chance", out m_PurgeChance);
			base.Model.SkillParameters.TryGetValue("stun_chance", out m_StunChance);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageMod);
			base.Model.SkillParameters.TryGetValue("stun_duration", out m_StunDuration);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_UseSkillAssetId = base.Model.SkillParameters.ContainsKey("use_skill_asset");
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			m_FallBackTime *= 4f;
			ActionsAfterTargetSelection.Add(delegate(BattleGameData battle, ICombatant source, ICombatant target)
			{
				List<ICombatant> list2 = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c.IsParticipating).ToList();
				ICombatant item = list2[UnityEngine.Random.Range(0, list2.Count)];
				m_Targets = new List<ICombatant> { item };
				source.AttackTarget = m_Targets.FirstOrDefault();
				return 0f;
			});
			ActionsOnEnd.Add(delegate(BattleGameData battle, ICombatant source, ICombatant target)
			{
				List<ICombatant> list = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c.IsParticipating).ToList();
				if (list.Count <= 0)
				{
					return 0f;
				}
				m_secondTarget = list[UnityEngine.Random.Range(0, list.Count)];
				m_thirdTarget = list[UnityEngine.Random.Range(0, list.Count)];
				if (m_StunChance / 100f > UnityEngine.Random.value)
				{
					DebugLog.Log("Stunned");
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
					battleEffectGameData.m_Target.CombatantView.PlayStunnedAnimation();
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
				return 0f;
			});
			m_ApplyPerks = false;
			m_IsMelee = false;
			m_UseFocusPosition = true;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_6}", m_StunChance.ToString("0"));
			dictionary.Add("{value_2}", m_StunDuration.ToString("0"));
			dictionary.Add("{value_3}", m_PurgeChance.ToString("0"));
			dictionary.Add("{value_5}", m_ChargeTurns.ToString("0"));
			dictionary.Add("{value_1}", Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
