using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class FocusAttackSkill : SkillBattleDataBase
	{
		protected bool m_All;

		private bool m_UseSkillAssetId;

		private float m_Charge;

		private bool m_CurseWithSpotlight;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_UseSkillAssetId = base.Model.SkillParameters.ContainsKey("use_skill_asset");
			m_CurseWithSpotlight = base.Model.SkillParameters.ContainsKey("spotlight_enemy");
			base.Model.SkillParameters.TryGetValue("charge", out m_Charge);
			if (!m_UseSkillAssetId)
			{
				DIContainerLogic.GetVisualEffectsBalancing().TryGetBubbleSetting("Taunt", out m_BubbleSetting);
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger attack skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			m_Source = source;
			m_InitialTarget = target;
			m_Targets = new List<ICombatant> { target };
			List<ICombatant> tauntedCharacters = new List<ICombatant>();
			tauntedCharacters = new List<ICombatant>();
			ICombatant source2 = source;
			tauntedCharacters.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction && (base.Model.Balancing.TargetSelfPossible || c != source2)).ToList());
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant taunted in tauntedCharacters)
			{
				taunted.CombatantView.PlayLaughAnimation();
				List<float> valueList = new List<float> { 1f };
				BattleEffectGameData effect2 = new BattleEffectGameData(target, taunted, source, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnTarget,
						EffectType = BattleEffectType.Taunt,
						Values = valueList,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, (!m_UseSkillAssetId) ? "Force_Target" : base.Model.Balancing.EffectIconAssetId, SkillEffectTypes.Blessing, GetLocalizedName(), base.Model.SkillNameId);
				effect2.SetPersistanceAfterDefeat(false).AddEffect();
				CharacterSpeechBubble targetingBubble = taunted.CombatantView.m_SpeechBubbles.Values.FirstOrDefault();
				if (targetingBubble != null && targetingBubble.m_IsTargetedBubble)
				{
					targetingBubble.SetTargetIcon("Target_" + target.CombatantAssetId);
					targetingBubble.UpdateSkill();
				}
			}
			if (m_CurseWithSpotlight)
			{
				BattleEffectGameData effect = new BattleEffectGameData(source, target, source, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnTarget,
						EffectType = BattleEffectType.None,
						Values = new List<float>(),
						AfflicionType = SkillEffectTypes.Curse,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, "InTheSpotlight", SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
			}
			m_Targets = tauntedCharacters;
			m_Source = target;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", base.Model.Balancing.EffectDuration.ToString("0"));
			dictionary.Add("{value_5}", m_Charge.ToString());
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
