using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackSameTargetBlessing : SkillBattleDataBase
	{
		private float m_Chance;

		private float m_Damage;

		private float m_Mirror;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_Damage);
			base.Model.SkillParameters.TryGetValue("chance", out m_Chance);
			base.Model.SkillParameters.TryGetValue("mirror_self", out m_Mirror);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (!base.Model.SkillParameters.ContainsKey("all"))
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant target2 = target;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			List<float> valueList = new List<float> { m_Chance, m_Damage, m_Mirror };
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (skillTarget != source)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
				}
				if (m_Mirror == 0f)
				{
					BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.AfterAttack,
							EffectType = BattleEffectType.AttackSameEnemy,
							AfflicionType = base.Model.Balancing.EffectType,
							Values = valueList,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
					effect.SetPersistanceAfterDefeat(false);
					effect.AddEffect();
				}
				else
				{
					BattleEffectGameData effect2 = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.BeforeAttack,
							EffectType = BattleEffectType.AttackSameEnemy,
							AfflicionType = base.Model.Balancing.EffectType,
							Values = valueList,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
					effect2.SetPersistanceAfterDefeat(false);
					effect2.AddEffect();
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_6}", string.Empty + m_Damage);
			dictionary.Add("{value_2}", string.Empty + base.Model.Balancing.EffectDuration);
			dictionary.Add("{value_3}", string.Empty + m_Chance);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
