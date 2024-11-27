using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class BattleOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public UILabel m_Desc;

	public LootDisplayContoller m_MainPrice;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	public float m_OffsetLeft = 50f;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialSize = m_ContainerControl.m_Size;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
	}

	public void ShowBattleOverlay(Transform root, HotspotGameData hotspot, BattleBalancingData battleBalancing, string overrideIdent, Camera orientatedCamera)
	{
		SetContent(hotspot, battleBalancing, overrideIdent);
		DebugLog.Log("Begin show Battle Overlay " + hotspot.StageName);
		Show(root, orientatedCamera);
	}

	private void Show(Transform root, Camera orientatedCamera)
	{
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void SetContent(HotspotGameData hotspot, BattleBalancingData battleBalancing, string overrideIdent)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			if (hotspot.WorldMapView != null && hotspot.WorldMapView.m_MiniCampaignHotspot)
			{
				m_Header.text = DIContainerInfrastructure.GetLocaService().Tr(hotspot.BalancingData.ZoneLocaIdent + "_battleground");
				if (hotspot.WorldMapView.GetOutgoingHotspots().Count > 0)
				{
					m_Header.text = m_Header.text + " " + hotspot.BalancingData.ZoneStageIndex;
				}
			}
			else
			{
				m_Header.text = hotspot.StageName;
			}
		}
		if (battleBalancing != null)
		{
			if (!string.IsNullOrEmpty(overrideIdent))
			{
				m_Desc.text = DIContainerInfrastructure.GetLocaService().Tr(overrideIdent);
			}
			else if (DIContainerLogic.GetBattleService().IsWaveBattle(battleBalancing))
			{
				m_Desc.text = DIContainerInfrastructure.GetLocaService().Tr("hotspot_tt_wavebattleground");
			}
			else
			{
				m_Desc.text = DIContainerInfrastructure.GetLocaService().Tr("hotspot_tt_battleground");
			}
		}
		int level = ((battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : battleBalancing.BaseLevel);
		LootTableBalancingData balancing = null;
		int num = 0;
		int level2 = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		ExperienceLevelBalancingData balancing2;
		if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + level2.ToString("00"), out balancing2) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (level2 - 1).ToString("00"), out balancing2))
		{
			num = balancing2.MatchmakingRangeIndex;
		}
		string nameId = battleBalancing.LootTableWheel.FirstOrDefault().Key.Replace("{levelrange}", num.ToString("00"));
		DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(nameId, out balancing);
		DIContainerLogic.EventSystemService.UpdateCachedFallbackLoot(hotspot.BalancingData);
		LootTableBalancingData balancing3 = null;
		if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(balancing.LootTableEntries[0].NameId, out balancing3))
		{
			m_MainPrice.SetModel(null, new List<IInventoryItemGameData>(), LootDisplayType.Major);
			return;
		}
		int num2 = balancing.LootTableEntries[0].BaseValue;
		if (hotspot.IsDungeon() && DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing != null && DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing.BonusType == BonusEventType.DungeonBonus)
		{
			float num3 = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing.BonusFactor / 100f;
			num2 += (int)((float)num2 * num3);
		}
		IInventoryItemGameData mainItem = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, level, 1, balancing.LootTableEntries[0].NameId, num2, EquipmentSource.LootBird);
		m_MainPrice.SetModel(mainItem, new List<IInventoryItemGameData>(), LootDisplayType.Major);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) < 0f)
		{
			return new Vector3(initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
		}
		return new Vector3(0f - initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y, initialContainerControlPos.z);
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			GetComponent<Animation>().Play("InfoOverlay_Leave");
			Invoke("Disable", GetComponent<Animation>()["InfoOverlay_Leave"].length);
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}
}
