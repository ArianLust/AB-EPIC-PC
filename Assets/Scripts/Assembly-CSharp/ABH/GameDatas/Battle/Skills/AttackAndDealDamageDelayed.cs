using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackAndDealDamageDelayed : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_DelayModifier;

		private bool m_DelayDamagesAll = true;

		private bool m_PlaySurprise;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("damage_delayed", out m_DelayModifier);
			m_DelayDamagesAll = base.Model.SkillParameters.Keys.Contains("delayed_all");
			m_PlaySurprise = base.Model.SkillParameters.Keys.Contains("surprise");
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			foreach (string key in target.CurrrentEffects.Keys)
			{
				if (key == "YouAreBugged")
				{
					return;
				}
			}
			List<float> list = new List<float>();
			list.Add(m_DelayModifier);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
					EffectType = BattleEffectType.None,
					Values = values,
					AfflicionType = base.Model.Balancing.EffectType,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.EffectRemovedAction = delegate(BattleEffectGameData e)
			{
				OnEffectRemoved(target, m_DelayModifier / 100f * source.ModifiedAttack, source, battle, e);
			};
			battleEffectGameData.AddEffect();
			if (m_PlaySurprise)
			{
				target.CombatantView.PlaySurprisedAnimation();
			}
		}

		private void OnEffectRemoved(ICombatant target, float damageValue, ICombatant source, BattleGameData battle, BattleEffectGameData effect)
		{
			if (effect.GetTurnsLeft() > 0)
			{
				return;
			}
			float num = 1f;
			float num2 = 1f;
			List<ICombatant> list = new List<ICombatant>();
			if (m_DelayDamagesAll)
			{
				list = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source.CombatantFaction).ToList();
			}
			else
			{
				list.Add(target);
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting, list);
			for (int i = 0; i < list.Count; i++)
			{
				ICombatant combatant = list[i];
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeDealDamage, m_Source, m_Source);
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeReceiveDamage, combatant, source);
				num2 = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(num2, EffectTriggerType.OnDealDamage, m_Source, combatant);
				damageValue *= num2;
				num = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(num, EffectTriggerType.OnReceiveDamage, combatant, m_Source);
				float num3 = damageValue * num;
				combatant.ReceiveDamage(num3, source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(combatant, battle, source);
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(num3, EffectTriggerType.AfterReceiveDamage, combatant, m_Source);
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_5}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_7}", string.Empty + Convert.ToInt32(m_DelayModifier / 100f * invoker.ModifiedAttack).ToString("0"));
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_2}", string.Empty + base.Model.Balancing.EffectDuration);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, dictionary);
		}
	}
}
