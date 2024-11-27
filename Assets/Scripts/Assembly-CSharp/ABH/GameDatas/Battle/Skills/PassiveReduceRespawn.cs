using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveReduceRespawn : SkillBattleDataBase
	{
		private float m_ReduceChargeTurns;

		private float m_Chance;

		private CharacterAssetControllerSquire m_SquireAssetController;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("reduce_charge_turns", out m_ReduceChargeTurns);
			model.SkillParameters.TryGetValue("chance", out m_Chance);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			m_Source = source;
			m_Targets = new List<ICombatant>();
			m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c != m_Source && c.IsAlive).ToList());
			if (m_SquireAssetController == null)
			{
				SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
				Object @object = Object.FindObjectOfType<CharacterAssetControllerSquire>();
				if (@object == null)
				{
					DebugLog.Error(GetType(), "Spawning Squire Pig - Squire not found! (not yet instantiated? CharacterAssetControllerSquire script on prefab?)");
				}
				m_SquireAssetController = @object as CharacterAssetControllerSquire;
				source.CombatantView.LastingVisualEffects.Remove(m_VisualEffectSetting.BalancingId);
				m_SquireAssetController.transform.parent = source.CombatantView.transform.parent;
				source.KnockedOut += PlaySquireDefeated;
				battle.BattleEndedWithWinner += BattleEndedHandler;
			}
			List<float> list = new List<float>();
			list.Add(m_ReduceChargeTurns);
			List<float> values = list;
			foreach (ICombatant target2 in m_Targets)
			{
				if (m_Chance >= (float)Random.Range(1, 100))
				{
					if (target2.KnockedOutSkill == null)
					{
						target2.KnockedOutSkill = new SkillGameData("pvp_support_knockout_revive_self").GenerateSkillBattleData();
					}
					target2.KnockedOutSkill.SetChargeDuration((int)(3f - m_ReduceChargeTurns));
					RemoveSquireEventHandler(target2);
					AddSquireEventHandlers(target2);
				}
				else
				{
					if (target2.KnockedOutSkill == null)
					{
						target2.KnockedOutSkill = new SkillGameData("pvp_support_knockout_revive_self").GenerateSkillBattleData();
					}
					target2.KnockedOutSkill.SetChargeDuration(3);
					RemoveSquireEventHandler(target2);
				}
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.None,
						EffectType = BattleEffectType.None,
						Values = values,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
		}

		private void PlaySquireDefeated()
		{
			float num = 0f;
			num = m_SquireAssetController.PlaySquireAnimation("Defeated");
		}

		private void BattleEndedHandler(Faction winningFaction)
		{
			if (m_SquireAssetController != null && winningFaction == m_Source.CombatantFaction)
			{
				m_SquireAssetController.PlaySquireAnimation("Victory");
			}
		}

		private void AddSquireEventHandlers(ICombatant skillTarget)
		{
			skillTarget.KnockedOut += TriggerSquireAnimation;
		}

		private void RemoveSquireEventHandler(ICombatant skillTarget)
		{
			skillTarget.KnockedOut -= TriggerSquireAnimation;
		}

		private void TriggerSquireAnimation()
		{
			if (m_SquireAssetController != null)
			{
				m_SquireAssetController.PlaySquireAnimation("Trigger");
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_ReduceChargeTurns.ToString("0"));
			dictionary.Add("{value_2}", m_Chance.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
