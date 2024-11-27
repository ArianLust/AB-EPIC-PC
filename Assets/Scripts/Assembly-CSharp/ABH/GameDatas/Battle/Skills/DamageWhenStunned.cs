using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class DamageWhenStunned : SkillBattleDataBase
	{
		protected float m_DamageInPercent;

		protected float m_Fixed;

		protected float m_CleanseChance;

		protected bool m_All;

		protected bool m_Self;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageInPercent);
			base.Model.SkillParameters.TryGetValue("damage_fix", out m_Fixed);
			m_All = base.Model.SkillParameters.ContainsKey("all");
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger set bonus skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			m_Targets = new List<ICombatant>();
			m_Targets.AddRange(battle.m_CombatantsPerFaction[DIContainerLogic.GetBattleService().GetOppositeFaction(source.CombatantFaction)]);
			foreach (ICombatant target2 in m_Targets)
			{
				List<float> list = new List<float>();
				BattleEffectGameData battleEffectGameData;
				if (m_Fixed > 0f)
				{
					list.Add(m_Fixed);
					battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
							EffectType = BattleEffectType.AddDamageWhenStunnedFixed,
							Values = list,
							AfflicionType = base.Model.Balancing.EffectType,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.SetPassive, GetLocalizedName(), base.Model.SkillNameId);
				}
				else
				{
					list.Add(m_DamageInPercent);
					battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
							EffectType = BattleEffectType.AddDamageWhenStunned,
							Values = list,
							AfflicionType = base.Model.Balancing.EffectType,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.SetPassive, GetLocalizedName(), base.Model.SkillNameId);
				}
				battleEffectGameData.AddEffect();
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_Fixed > 0f)
			{
				dictionary.Add("{value_1}", m_Fixed.ToString("0"));
			}
			else
			{
				dictionary.Add("{value_1}", m_DamageInPercent.ToString("0"));
			}
			dictionary.Add("{value_2}", base.Model.Balancing.EffectDuration.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
