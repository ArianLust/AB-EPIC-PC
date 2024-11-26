using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Models;
using UnityEngine;

public class CharacterChunkHealthBar : CharacterHealthBar
{
	[Header("HealthChunks")]
	[SerializeField]
	protected List<Animation> m_HealthChunks;

	protected int m_NumberOfChunks;

	[Header("Defines")]
	private string m_aniForChunkActive = "Filled";

	private string m_aniForChunkInactive = "Empty";

	private string m_aniForChunkLost = "Destroy";

	private void Awake()
	{
		m_NumberOfChunks = m_HealthChunks.Count;
	}

	protected new void DeregisterEventHandler()
	{
		if (m_Model != null)
		{
			m_Model.HealthChanged -= OnHealthChanged;
		}
	}

	public override void SetModel(ICombatant model)
	{
		m_Model = model;
		if (model == null)
		{
			return;
		}
		RegisterEventHandler();
		if (GetComponent<Animation>() != null)
		{
			GetComponent<Animation>().Play("HealthBar_Show");
		}
		int num = (int)((m_Model.ModifiedHealth - m_Model.CurrentHealth) / (m_Model.ModifiedHealth / (float)m_NumberOfChunks));
		for (int i = 0; i < m_NumberOfChunks; i++)
		{
			Animation animation = m_HealthChunks[m_HealthChunks.Count - 1 - i];
			animation.gameObject.PlayAnimationOrAnimatorState((i >= num) ? m_aniForChunkActive : m_aniForChunkInactive);
		}
		if (model is BossCombatant && m_BossLevel != null)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss == null)
			{
				m_BossLevel.text = "0";
			}
			else
			{
				m_BossLevel.text = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.DeathCount.ToString();
			}
		}
	}

	protected override IEnumerator ChangeHealthBar(float maxHealth, float oldHealth, float newHealth)
	{
		float chunkSize = maxHealth / (float)m_NumberOfChunks;
		int formerChunksGone = (int)((maxHealth - oldHealth) / chunkSize);
		if (oldHealth == newHealth)
		{
			for (int i = 0; i < m_NumberOfChunks; i++)
			{
				if (i < formerChunksGone)
				{
					m_HealthChunks[m_NumberOfChunks - 1 - i].Play(m_aniForChunkInactive);
				}
				else
				{
					m_HealthChunks[m_NumberOfChunks - 1 - i].Play(m_aniForChunkActive);
				}
			}
			yield break;
		}
		int totalChunksGone = (int)((maxHealth - newHealth) / chunkSize);
		int nrOfPlops = totalChunksGone - formerChunksGone;
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowHealthBarChangeInSec);
		for (int j = 0; j < nrOfPlops; j++)
		{
			Animation cChunk = m_HealthChunks[m_HealthChunks.Count - 1 - formerChunksGone - j];
			yield return new WaitForSeconds(0.5f * cChunk.gameObject.PlayAnimationOrAnimatorState(m_aniForChunkLost));
		}
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForSetCurrentHealthBarInSec);
		if (!m_Model.IsAlive)
		{
			StartCoroutine(RemoveHealthBar());
		}
		DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.LastDisplayedBossHealth = newHealth;
	}

	public override void UpdateHealth()
	{
		WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
		if (worldBoss.LastDisplayedBossHealth <= 0f)
		{
			worldBoss.LastDisplayedBossHealth = m_Model.CurrentHealth;
		}
		StartCoroutine(ChangeHealthBar(m_Model.ModifiedHealth, worldBoss.LastDisplayedBossHealth, m_Model.CurrentHealth));
	}

	protected override void OnHealthChanged(float oldHealth, float newHealth)
	{
		if (!GetComponent<Animation>().IsPlaying("HealthBar_Show") && base.gameObject.activeSelf)
		{
			WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
			if (worldBoss.LastDisplayedBossHealth <= 0f)
			{
				worldBoss.LastDisplayedBossHealth = m_Model.CurrentHealth;
			}
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(ChangeHealthBar(m_Model.ModifiedHealth, worldBoss.LastDisplayedBossHealth, m_Model.CurrentHealth));
			}
		}
	}

	public void SetHealthInstant()
	{
		if (m_Model == null)
		{
			return;
		}
		int num = (int)((m_Model.ModifiedHealth - m_Model.CurrentHealth) / (m_Model.ModifiedHealth / (float)m_NumberOfChunks));
		for (int i = 0; i < m_NumberOfChunks; i++)
		{
			Animation animation = m_HealthChunks[m_HealthChunks.Count - 1 - i];
			if (i < num)
			{
				animation.gameObject.PlayAnimationOrAnimatorState(m_aniForChunkInactive);
			}
			else
			{
				animation.gameObject.PlayAnimationOrAnimatorState(m_aniForChunkActive);
			}
		}
		WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
		worldBoss.LastDisplayedBossHealth = m_Model.CurrentHealth;
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}
}
