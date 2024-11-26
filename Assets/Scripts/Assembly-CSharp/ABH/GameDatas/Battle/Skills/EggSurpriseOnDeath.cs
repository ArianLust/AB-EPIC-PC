using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class EggSurpriseOnDeath : SkillBattleDataBase
	{
		protected float m_PurgeChance;

		protected float m_StunChance;

		protected float m_StunDuration;

		private float m_DamageMod;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("purge_chance", out m_PurgeChance);
			base.Model.SkillParameters.TryGetValue("stun_chance", out m_StunChance);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageMod);
			base.Model.SkillParameters.TryGetValue("stun_duration", out m_StunDuration);
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger set bonus skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			List<float> list = new List<float>();
			list.Add(m_PurgeChance);
			list.Add(m_StunChance);
			list.Add(m_StunDuration);
			list.Add(m_DamageMod);
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, source, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnAllHealthLost,
					EffectType = BattleEffectType.TriggerEggSurprise,
					Values = list,
					AfflicionType = base.Model.Balancing.EffectType,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.SetPassive, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
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
