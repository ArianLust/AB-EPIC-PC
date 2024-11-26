using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class HotSpotWorldMapViewGoldenPig : MonoBehaviour
{
	private HotspotGameData Model;

	private BattleBalancingData m_AssociatedBattle;

	[SerializeField]
	private MakeOrthoIndependent m_OrthoIndependent;

	[SerializeField]
	private string m_OverrideLocaIdent;

	private bool m_ClickActive;

	private WorldMapStateMgr m_StateMgr;

	private bool m_entered;

	[SerializeField]
	private GameObject[] m_flagPrefabs;

	[SerializeField]
	private Transform m_flagRoot;

	private GameObject m_currentFlag;

	[SerializeField]
	private CharacterAssetController m_GoldenPigAsset;

	private void OnEnable()
	{
		base.transform.localScale = Vector3.one;
	}

	public void ShowTooltip()
	{
		if (m_AssociatedBattle == null)
		{
			string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(Model.BalancingData.BattleId, DIContainerInfrastructure.GetCurrentPlayer());
			m_AssociatedBattle = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle);
		}
		if (m_AssociatedBattle != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowBattleOverlay(base.transform, Model, m_AssociatedBattle, m_OverrideLocaIdent, false);
		}
	}

	public HotSpotWorldMapViewGoldenPig SetModel(HotspotGameData hotspot)
	{
		Model = hotspot;
		return this;
	}

	public HotSpotWorldMapViewGoldenPig SetStateMgr(WorldMapStateMgr stateMgr)
	{
		m_StateMgr = stateMgr;
		return this;
	}

	public void Enter()
	{
		if (!m_entered)
		{
			m_entered = true;
			base.gameObject.SetActive(true);
			RefreshAsset();
			if (m_OrthoIndependent != null)
			{
				m_OrthoIndependent.Init();
			}
			StartCoroutine(EnterCoroutine());
		}
	}

	private IEnumerator EnterCoroutine()
	{
		if ((bool)m_currentFlag)
		{
			Object.Destroy(m_currentFlag);
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("GoldenPigsCloud_Enter"));
		base.gameObject.PlayAnimationOrAnimatorState("GoldenPigsCloud_Idle");
		RegisterEventHandlers();
		InvokeRepeating("PlayDelayedPigCheer", 5f, 30f);
	}

	private void PlayDelayedPigCheer()
	{
		if (!m_StateMgr.m_WorldMenuUI.m_OptionsMgr.m_CreditsMgr.enabled)
		{
			float time = Random.value * 5f;
			m_GoldenPigAsset.Invoke("PlayCheerAnim", time);
		}
	}

	public void Leave()
	{
		if (m_entered)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	public void RefreshAsset()
	{
		m_GoldenPigAsset.SetModel("pig_golden_pig", true, true, true);
	}

	public IEnumerator LeaveCoroutine()
	{
		DeReregisterEventHandlers();
		m_currentFlag = Object.Instantiate(m_flagPrefabs[Mathf.Clamp(Model.Data.StarCount - 1, 0, 3)]);
		if (m_currentFlag != null)
		{
			m_currentFlag.transform.parent = m_flagRoot;
			m_currentFlag.transform.localPosition = Vector3.zero;
			m_currentFlag.transform.localScale = Vector3.one;
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("GoldenPigsCloud_Leave"));
		m_entered = false;
		CancelInvoke("PlayDelayedPigCheer");
		base.gameObject.SetActive(false);
	}

	private void RegisterEventHandlers()
	{
		DeReregisterEventHandlers();
		m_ClickActive = true;
	}

	private void DeReregisterEventHandlers()
	{
		m_ClickActive = false;
	}

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	private void HandleClicked()
	{
		if (!DIContainerInfrastructure.CurrentDragController.m_dragging && m_ClickActive)
		{
			HandleMouseButtonUp();
		}
	}

	public void HandleMouseButtonUp()
	{
		if (m_ClickActive)
		{
			DebugLog.Log("Hotspot Clicked!");
			m_StateMgr.ShowGoldenPigBattlePreperationScreen();
		}
	}
}
