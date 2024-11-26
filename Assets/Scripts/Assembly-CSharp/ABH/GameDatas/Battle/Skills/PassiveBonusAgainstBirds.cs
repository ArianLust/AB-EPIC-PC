using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveBonusAgainstBirds : SkillBattleDataBase
	{
		private float m_DamageBonusVsRed = 100f;

		private float m_DamageBonusVsYellow = 100f;

		private float m_DamageBonusVsWhite = 100f;

		private float m_DamageBonusVsBlack = 100f;

		private float m_DamageBonusVsBlues = 100f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_red", out m_DamageBonusVsRed);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_yellow", out m_DamageBonusVsYellow);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_white", out m_DamageBonusVsWhite);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_black", out m_DamageBonusVsBlack);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_blues", out m_DamageBonusVsBlues);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger passive skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			BattleEffectType effectType = BattleEffectType.IncreaseDamageVsRed;
			float item = 100f;
			if (m_DamageBonusVsRed > 100f)
			{
				effectType = BattleEffectType.IncreaseDamageVsRed;
				item = m_DamageBonusVsRed;
			}
			else if (m_DamageBonusVsYellow > 100f)
			{
				effectType = BattleEffectType.IncreaseDamageVsYellow;
				item = m_DamageBonusVsYellow;
			}
			else if (m_DamageBonusVsWhite > 100f)
			{
				effectType = BattleEffectType.IncreaseDamageVsWhite;
				item = m_DamageBonusVsWhite;
			}
			else if (m_DamageBonusVsBlack > 100f)
			{
				effectType = BattleEffectType.IncreaseDamageVsBlack;
				item = m_DamageBonusVsBlack;
			}
			else if (m_DamageBonusVsBlues > 100f)
			{
				effectType = BattleEffectType.IncreaseDamageVsBlues;
				item = m_DamageBonusVsBlues;
			}
			List<float> list = new List<float>();
			list.Add(item);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, source, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					EffectType = effectType,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
			battleEffectGameData.SetPersistanceAfterDefeat(false);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_DamageBonusVsRed > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsRed);
			}
			if (m_DamageBonusVsYellow > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsYellow);
			}
			if (m_DamageBonusVsWhite > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsWhite);
			}
			if (m_DamageBonusVsBlack > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsBlack);
			}
			if (m_DamageBonusVsBlues > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsBlues);
			}
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
