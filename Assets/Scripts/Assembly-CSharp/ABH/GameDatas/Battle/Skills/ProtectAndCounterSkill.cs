using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class ProtectAndCounterSkill : SkillBattleDataBase
	{
		private int m_BuffDuration;

		private float m_Percent;

		private float m_CounterAttackDamage;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
			base.Model.SkillParameters.TryGetValue("chance", out m_Percent);
			base.Model.SkillParameters.TryGetValue("counter_damage", out m_CounterAttackDamage);
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
			target.CombatantView.PlayCheerCharacter();
			List<float> valueList = new List<float> { m_Percent, m_CounterAttackDamage };
			string effectIdent = base.Model.Balancing.AssetId;
			List<ICombatant> possibleHolders = battle.m_CombatantsPerFaction[Faction.Birds];
			for (int i = 0; i < possibleHolders.Count; i++)
			{
				foreach (BattleEffectGameData effects in possibleHolders[i].CurrrentEffects.Values)
				{
					if (effects.m_EffectIdent == effectIdent)
					{
						effectIdent += "_secondary";
						i = possibleHolders.Count;
						break;
					}
				}
			}
			if (target != m_Source)
			{
				BattleEffectGameData effect = new BattleEffectGameData(target, source, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.AfterTargetSelection,
						EffectType = BattleEffectType.ShelterAndCounter,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = m_BuffDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, m_BuffDuration, battle, effectIdent, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.SetPersistanceAfterDefeat(false).AddEffect();
				BattleEffectGameData effect2 = new BattleEffectGameData(effectIdent: base.Model.Balancing.AssetId + "_counter", source: source, target: target, effects: new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.AfterTargetSelection,
						EffectType = BattleEffectType.CounterWithSource,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = m_BuffDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, duration: m_BuffDuration, battle: battle, type: base.Model.Balancing.EffectType, locaIdent: GetLocalizedName(), skillIdent: base.Model.SkillNameId);
				effect2.SetPersistanceAfterDefeat(false).LinkEffect(effect).AddEffect();
			}
			else
			{
				BattleEffectGameData effect3 = new BattleEffectGameData(effectIdent: base.Model.Balancing.AssetId + "_counter_only", source: source, target: target, effects: new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.Counter,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = m_BuffDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, duration: m_BuffDuration, battle: battle, type: base.Model.Balancing.EffectType, locaIdent: GetLocalizedName(), skillIdent: base.Model.SkillNameId);
				effect3.SetPersistanceAfterDefeat(false).AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_3}", string.Empty + m_Percent);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_1}", string.Empty + m_CounterAttackDamage);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
