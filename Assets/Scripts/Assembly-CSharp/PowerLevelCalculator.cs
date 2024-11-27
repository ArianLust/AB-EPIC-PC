using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Models;
using UnityEngine;

public class PowerLevelCalculator
{
	private PowerLevelBalancingData m_playerPowerLevelBalancing;

	private int m_currentPlayerLevel;

	private ScoreBalancingData m_scorebalancing;

	private Dictionary<string, float> m_pigTypePowerLevelBalancing;

	public PowerLevelCalculator()
	{
		m_scorebalancing = DIContainerBalancing.Service.GetBalancingData<ScoreBalancingData>("default_score");
		m_pigTypePowerLevelBalancing = new Dictionary<string, float>();
		foreach (PigTypePowerLevelBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<PigTypePowerLevelBalancingData>())
		{
			m_pigTypePowerLevelBalancing.Add(balancingData.NameId, balancingData.PowerLevelWeight);
		}
	}

	public void ClearCache()
	{
		m_currentPlayerLevel = 0;
		m_playerPowerLevelBalancing = null;
		m_scorebalancing = DIContainerBalancing.Service.GetBalancingData<ScoreBalancingData>("default_score");
		m_pigTypePowerLevelBalancing.Clear();
		foreach (PigTypePowerLevelBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<PigTypePowerLevelBalancingData>())
		{
			m_pigTypePowerLevelBalancing.Add(balancingData.NameId, balancingData.PowerLevelWeight);
		}
	}

	public int GetBannerPowerLevel(BannerGameData banner)
	{
		float num = 0f;
		num += banner.BaseHealth;
		if (banner.BannerTip.IsSetItem && banner.BannerCenter.IsSetItem)
		{
			num += num * ((float)m_scorebalancing.PowerLevelFactorPerSetItemBanner * 2f / 100f);
		}
		else if (banner.BannerTip.IsSetItem || banner.BannerCenter.IsSetItem)
		{
			num += num * ((float)m_scorebalancing.PowerLevelFactorPerSetItemBanner / 100f);
		}
		if (banner.BannerCenter.IsSetCompleted(banner))
		{
			num += num * ((float)m_scorebalancing.PowerLevelFactorForCompleteSetBanner / 100f);
		}
		num /= (float)m_scorebalancing.PowerLevelDivideEndValue;
		return Mathf.RoundToInt(num);
	}

	public int GetBirdPowerLevel(ICharacter bird)
	{
		float num = 0f;
		num += bird.BaseHealth;
		num += bird.BaseAttack * (float)m_scorebalancing.PowerLevelFactorForDamage / 100f;
		if (bird.MainHandItem != null && bird.OffHandItem != null)
		{
			if (bird.MainHandItem.IsSetItem && bird.OffHandItem.IsSetItem)
			{
				num += num * ((float)m_scorebalancing.PowerLevelFactorPerSetItemBird * 2f / 100f);
			}
			else if (bird.MainHandItem.IsSetItem || bird.OffHandItem.IsSetItem)
			{
				num += num * ((float)m_scorebalancing.PowerLevelFactorPerSetItemBird / 100f);
			}
			if (bird.MainHandItem.IsSetCompleted(bird))
			{
				num += num * ((float)m_scorebalancing.PowerLevelFactorForCompleteSetBird / 100f);
			}
		}
		num /= (float)m_scorebalancing.PowerLevelDivideEndValue;
		return Mathf.RoundToInt(num);
	}

	public int GetPvPTeamPowerLevel(PublicPlayerData player, List<int> selectedBirds)
	{
		float unroundedTeamPowerLevel = GetUnroundedTeamPowerLevel(player, selectedBirds);
		unroundedTeamPowerLevel += (float)GetBannerPowerLevel(new BannerGameData(player.Banner));
		return Mathf.RoundToInt(unroundedTeamPowerLevel);
	}

	public int GetTeamPowerLevel(PublicPlayerData player, List<int> selectedBirds)
	{
		float unroundedTeamPowerLevel = GetUnroundedTeamPowerLevel(player, selectedBirds);
		return Mathf.RoundToInt(unroundedTeamPowerLevel);
	}

	private float GetUnroundedTeamPowerLevel(PublicPlayerData player, List<int> selectedBirds)
	{
		float num = 0f;
		for (int i = 0; i < player.Birds.Count; i++)
		{
			if (selectedBirds == null || selectedBirds.Contains(i))
			{
				num += (float)GetBirdPowerLevel(new BirdGameData(player.Birds[i]));
			}
		}
		return num;
	}

	public int GetNormalizedTeamPowerLevel(PlayerGameData player, int birdsAllowed)
	{
		int playerHighestPowerLevel = GetPlayerHighestPowerLevel(player);
		float num = playerHighestPowerLevel / DIContainerInfrastructure.GetCurrentPlayer().Birds.Count;
		return Mathf.RoundToInt(num * (float)birdsAllowed);
	}

	public int GetPlayerHighestPowerLevel(PlayerGameData player)
	{
		float num = 0f;
		for (int i = 0; i < player.Birds.Count; i++)
		{
			BirdGameData bird = player.Birds[i];
			int birdPowerLevel = GetBirdPowerLevel(bird);
			num += (float)birdPowerLevel;
		}
		return Mathf.RoundToInt(num);
	}

	private PowerLevelBalancingData GetPlayerPowerLevel(int level)
	{
		if (m_currentPlayerLevel != level && m_playerPowerLevelBalancing != null)
		{
			m_currentPlayerLevel = level;
			m_playerPowerLevelBalancing = DIContainerBalancing.Service.GetBalancingData<PowerLevelBalancingData>(string.Format("PlayerLevel_{0}", level.ToString("00")));
		}
		return m_playerPowerLevelBalancing;
	}

	public int GetPigPowerLevel(ICharacter pig, BattleBalancingData battle)
	{
		float num = 0f;
		PowerLevelBalancingData playerPowerLevel = GetPlayerPowerLevel(pig.Level);
		float num2 = 0f;
		float num3 = 0f;
		if (playerPowerLevel != null)
		{
			num3 = playerPowerLevel.HealthModifier / 100f;
			num2 = playerPowerLevel.AttackModifier / 100f;
		}
		float num4 = (float)battle.Difficulty / 100f;
		num += pig.BaseHealth + pig.BaseHealth * num3 + pig.BaseHealth * num4;
		num += pig.BaseAttack + pig.BaseAttack * num2 + pig.BaseAttack * num4;
		float value = 0f;
		if (m_pigTypePowerLevelBalancing.TryGetValue(pig.AssetName, out value))
		{
			num *= value / 100f;
		}
		num /= (float)m_scorebalancing.PigPowerLevelDivideValue;
		return Mathf.RoundToInt(num);
	}
}
