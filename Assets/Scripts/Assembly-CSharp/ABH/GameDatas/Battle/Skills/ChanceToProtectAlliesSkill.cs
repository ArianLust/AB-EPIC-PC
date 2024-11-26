using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class ChanceToProtectAlliesSkill : SkillBattleDataBase
	{
		private int m_BuffDuration;

		private float m_AmountOfAllies = 1f;

		private float m_ChanceToProtect = 1f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("amount_of_targets", out m_AmountOfAllies);
			base.Model.SkillParameters.TryGetValue("chance_to_protect", out m_ChanceToProtect);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			ICombatant source2 = source;
			List<ICombatant> allAllies = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction && c != source2).ToList();
			m_Targets = new List<ICombatant>();
			while ((float)m_Targets.Count < m_AmountOfAllies && allAllies.Count > 0)
			{
				ICombatant chosenOne = allAllies[Random.Range(0, allAllies.Count)];
				m_Targets.Add(chosenOne);
				allAllies.Remove(chosenOne);
			}
			foreach (ICombatant protectTarget in m_Targets)
			{
				List<float> valueList = new List<float> { 0f, m_ChanceToProtect };
				BattleEffectGameData effect = new BattleEffectGameData(source, protectTarget, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.AfterTargetSelection,
						EffectType = BattleEffectType.Sheltered,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = m_BuffDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, m_BuffDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.SetPersistanceAfterDefeat(false).AddEffect();
				protectTarget.CombatantView.PlayCheerCharacter();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_3}", string.Empty + m_ChanceToProtect);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_1}", string.Empty + m_AmountOfAllies);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
