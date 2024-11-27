using System.Collections;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using UnityEngine;

public class CharacterHealthBar : MonoBehaviour
{
	protected ICombatant m_Model;

	[SerializeField]
	private UISprite m_ChangedBar;

	[SerializeField]
	private UISprite m_CurrentBar;

	[SerializeField]
	private UILabel m_InitiativeValue;

	[SerializeField]
	protected UILabel m_BossLevel;

	[SerializeField]
	private GameObject m_SpacerPrefab;

	[SerializeField]
	private float m_HealthBarsize;

	[SerializeField]
	private Transform m_SpacerParent;

	public virtual void SetModel(ICombatant model)
	{
		m_Model = model;
		RegisterEventHandler();
		OnHealthChanged(model.CurrentHealth, model.CurrentHealth);
		if (GetComponent<Animation>() != null)
		{
			GetComponent<Animation>().Play("HealthBar_Show");
		}
		RefreshInitative();
		if (model is BossCombatant && (bool)m_BossLevel)
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
		if (m_SpacerPrefab != null)
		{
			StartCoroutine(SetHealthBarSpacer());
		}
	}

	public void RefreshInitative()
	{
		if ((bool)m_InitiativeValue)
		{
			m_InitiativeValue.text = m_Model.CurrentInitiative.ToString();
		}
	}

	public void Focus()
	{
		if (GetComponent<Animation>() != null)
		{
			GetComponent<Animation>().Play("HealthBar_InitiativeFocus");
		}
	}

	protected void RegisterEventHandler()
	{
		DeregisterEventHandler();
		if (m_Model != null)
		{
			m_Model.HealthChanged += OnHealthChanged;
		}
	}

	protected void DeregisterEventHandler()
	{
		if (m_Model != null)
		{
			m_Model.HealthChanged -= OnHealthChanged;
		}
	}

	protected virtual void OnHealthChanged(float oldHealth, float newHealth)
	{
		if (!GetComponent<Animation>().IsPlaying("HealthBar_Show") && base.gameObject.activeSelf && base.gameObject.activeSelf)
		{
			StartCoroutine(ChangeHealthBar(m_Model.ModifiedHealth, oldHealth, newHealth));
		}
	}

	protected virtual IEnumerator ChangeHealthBar(float maxHealth, float oldHealth, float newHealth)
	{
		if (oldHealth == newHealth)
		{
			m_ChangedBar.fillAmount = newHealth / maxHealth;
			m_CurrentBar.fillAmount = newHealth / maxHealth;
			yield break;
		}
		if (oldHealth >= newHealth)
		{
			m_ChangedBar.spriteName = "Healthbar_Small_Bar_Damage";
			if (GetComponent<Animation>() != null)
			{
				GetComponent<Animation>().Play("HealthBar_Damage");
			}
		}
		else
		{
			m_ChangedBar.spriteName = "Healthbar_Small_Bar_Heal";
			if (GetComponent<Animation>() != null)
			{
				GetComponent<Animation>().Play("HealthBar_Heal");
			}
		}
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowHealthBarChangeInSec);
		m_ChangedBar.fillAmount = oldHealth / maxHealth;
		m_CurrentBar.fillAmount = newHealth / maxHealth;
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForSetCurrentHealthBarInSec);
		yield return StartCoroutine(ChangeValueOverTime(m_ChangedBar, maxHealth, oldHealth, newHealth, DIContainerLogic.GetPacingBalancing().TimeForFillHealthBarChangedInSec));
		if (!m_Model.IsAlive && DIContainerInfrastructure.LocationStateMgr == null)
		{
			StartCoroutine(RemoveHealthBar());
		}
	}

	public void EmptyBarOverTime(float time)
	{
		StartCoroutine(EmptyBarAndRemove(time));
	}

	protected IEnumerator EmptyBarAndRemove(float time)
	{
		yield return StartCoroutine(ChangeValueOverTime(m_CurrentBar, m_Model.ModifiedHealth, m_Model.CurrentHealth, 0f, time));
		StartCoroutine(RemoveHealthBar());
	}

	protected IEnumerator ChangeValueOverTime(UISprite bar, float maxHealth, float oldValue, float newValue, float time)
	{
		float delta = newValue - oldValue;
		float absDelta = Mathf.Abs(delta);
		float step = delta / time;
		float absStep = Mathf.Abs(step);
		float timeLeft = time;
		float currentDelta = 0f;
		while (timeLeft > 0f)
		{
			yield return new WaitForEndOfFrame();
			timeLeft -= Time.deltaTime;
			bar.fillAmount = (oldValue + delta * Mathf.Min(1f, 1f - timeLeft / time)) / maxHealth;
		}
	}

	public IEnumerator RemoveHealthBar()
	{
		if (GetComponent<Animation>() != null)
		{
			GetComponent<Animation>().Play("HealthBar_Hide");
			yield return new WaitForSeconds(GetComponent<Animation>()["HealthBar_Hide"].length);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	public virtual void UpdateHealth()
	{
		StartCoroutine(ChangeBarAfterDelay());
	}

	protected IEnumerator ChangeBarAfterDelay()
	{
		yield return new WaitForSeconds(0.3f);
		m_ChangedBar.fillAmount = m_Model.CurrentHealth / m_Model.ModifiedHealth;
		m_CurrentBar.fillAmount = m_Model.CurrentHealth / m_Model.ModifiedHealth;
	}

	private IEnumerator SetHealthBarSpacer()
	{
		foreach (Transform spacer in m_SpacerParent.transform)
		{
			Object.Destroy(spacer.gameObject);
		}
		yield return new WaitForEndOfFrame();
		int amountOfSpacers = GetSpacerAmount();
		if (amountOfSpacers != 0)
		{
			for (int i = 1; i <= amountOfSpacers; i++)
			{
				GameObject spacerBar = Object.Instantiate(m_SpacerPrefab);
				spacerBar.transform.parent = m_SpacerParent.transform;
				spacerBar.transform.localScale = Vector3.one;
				float unevenXpos = m_HealthBarsize / ((float)amountOfSpacers + 1f) * (float)i;
				int roundedXpos = (int)(unevenXpos / 2f) * 2;
				spacerBar.transform.localPosition = new Vector3(roundedXpos, 0f, 0f);
			}
		}
	}

	private int GetSpacerAmount()
	{
		float modifiedHealth = m_Model.ModifiedHealth;
		WorldBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island");
		int maxHPChunks = balancingData.MaxHPChunks;
		float hPChunkSteps = balancingData.HPChunkSteps;
		int hPChunksLowest = balancingData.HPChunksLowest;
		int hPChunksHighest = balancingData.HPChunksHighest;
		if (hPChunkSteps <= 0f)
		{
			return 0;
		}
		int num = 0;
		for (float num2 = hPChunksLowest; num2 < (float)hPChunksHighest; num2 *= hPChunkSteps)
		{
			if (modifiedHealth <= num2)
			{
				return num;
			}
			num++;
		}
		return maxHPChunks;
	}
}
