using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using UnityEngine;

public class UpgradeItemSlot : BaseItemSlot
{
	[SerializeField]
	private List<UISprite> m_DiceRoots = new List<UISprite>();

	[SerializeField]
	private List<GameObject> m_AnvilPrefabs = new List<GameObject>();

	[SerializeField]
	private List<GameObject> m_CauldronPrefabs = new List<GameObject>();

	[SerializeField]
	private UISprite m_ButtonBody;

	[SerializeField]
	public UIInputTrigger m_InputTrigger;

	private GameObject m_SelectionFrame;

	[SerializeField]
	private GameObject m_SelectionFramePrefab;

	[SerializeField]
	private CHMotionTween m_Tween;

	private IInventoryItemGameData m_Model;

	[HideInInspector]
	public bool m_Used;

	private CHMotionTween m_LocalTween;

	private Vector3 m_Position;

	private bool m_IsSetToDestroy;

	[method: MethodImpl(32)]
	public event Action<UpgradeItemSlot> OnSelected;

	[method: MethodImpl(32)]
	public event Action<UpgradeItemSlot> OnUsed;

	[method: MethodImpl(32)]
	public event Action<bool> OnModifyHorizontalDrag;

	public override bool SetModel(IInventoryItemGameData item, bool isPvp)
	{
		if (item == null)
		{
			base.gameObject.SetActive(false);
			return false;
		}
		m_Model = item;
		DeRegisterEventHandler();
		RegisterEventHandler();
		if (item.ItemData.Level >= 3)
		{
			foreach (GameObject cauldronPrefab in m_CauldronPrefabs)
			{
				cauldronPrefab.SetActive(false);
			}
			foreach (GameObject anvilPrefab in m_AnvilPrefabs)
			{
				anvilPrefab.SetActive(false);
			}
			base.gameObject.SetActive(false);
			return false;
		}
		if (item.ItemBalancing.NameId == "forge_leveled")
		{
			base.gameObject.SetActive(true);
			Deselect();
			for (int i = 0; i < m_AnvilPrefabs.Count; i++)
			{
				GameObject gameObject = m_AnvilPrefabs[i];
				if (item.ItemData.Level == i + 1)
				{
					gameObject.SetActive(true);
					m_DiceRoots[i].gameObject.SetActive(true);
				}
				else
				{
					gameObject.SetActive(false);
					m_DiceRoots[i].gameObject.SetActive(false);
				}
			}
			foreach (GameObject cauldronPrefab2 in m_CauldronPrefabs)
			{
				cauldronPrefab2.SetActive(false);
			}
		}
		else if (item.ItemBalancing.NameId == "cauldron_leveled")
		{
			base.gameObject.SetActive(true);
			Deselect();
			for (int j = 0; j < m_CauldronPrefabs.Count; j++)
			{
				GameObject gameObject2 = m_CauldronPrefabs[j];
				if (item.ItemData.Level == j + 1)
				{
					gameObject2.SetActive(true);
					m_DiceRoots[j].gameObject.SetActive(true);
				}
				else
				{
					gameObject2.SetActive(false);
					m_DiceRoots[j].gameObject.SetActive(false);
				}
			}
			foreach (GameObject anvilPrefab2 in m_AnvilPrefabs)
			{
				anvilPrefab2.SetActive(false);
			}
		}
		m_LocalTween = GetComponent<CHMotionTween>();
		if ((bool)m_Tween)
		{
			m_Position = m_Tween.transform.localPosition;
		}
		return true;
	}

	public IEnumerator MoveOffset(Vector2 offset, float duration)
	{
		Vector3 move = new Vector3(offset.x, offset.y, 0f);
		if ((bool)m_LocalTween)
		{
			m_LocalTween.m_EndOffset = offset;
			m_LocalTween.m_DurationInSeconds = duration;
			m_LocalTween.Play();
			yield return new WaitForSeconds(duration);
		}
	}

	public void ShowTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_Model.ItemData.Level + 1, 1, m_Model.ItemBalancing.NameId, 1), true, false);
	}

	public override IInventoryItemGameData GetModel()
	{
		return m_Model;
	}

	public void SelectItemData()
	{
		RaiseOnSelected();
	}

	private void RaiseOnUsed()
	{
		DebugLog.Log("Raised Used!");
		if (!m_Used && this.OnUsed != null)
		{
			this.OnUsed(this);
		}
	}

	public void RaiseOnSelected()
	{
		if (!m_Used && this.OnSelected != null)
		{
			this.OnSelected(this);
		}
	}

	public override void Select(bool classPreviewIsThis = false)
	{
		m_Used = true;
		StopCoroutine("DeselectCoroutine");
		if (!m_SelectionFrame)
		{
			m_SelectionFrame = UnityEngine.Object.Instantiate(m_SelectionFramePrefab, base.transform.position, Quaternion.identity) as GameObject;
			m_SelectionFrame.transform.parent = base.transform;
		}
		m_SelectionFrame.SetActive(true);
		if ((bool)m_SelectionFrame.GetComponent<Animation>()["Show"])
		{
			m_SelectionFrame.GetComponent<Animation>().Play("Show");
		}
		if ((bool)m_SelectionFrame.GetComponent<Animation>()["Loop"])
		{
			m_SelectionFrame.GetComponent<Animation>().PlayQueued("Loop");
		}
		UIPlayAnimation[] componentsInChildren = m_InputTrigger.GetComponentsInChildren<UIPlayAnimation>();
		UIPlayAnimation[] array = componentsInChildren;
		foreach (UIPlayAnimation uIPlayAnimation in array)
		{
			uIPlayAnimation.enabled = false;
		}
	}

	public override void Deselect(bool classPreviewIsNext = false)
	{
		m_Used = false;
		StartCoroutine("DeselectCoroutine");
	}

	private IEnumerator DeselectCoroutine()
	{
		if (m_SelectionFrame != null && (bool)m_SelectionFrame.GetComponent<Animation>()["Hide"])
		{
			m_SelectionFrame.GetComponent<Animation>().Play("Hide");
			yield return new WaitForSeconds(m_SelectionFrame.GetComponent<Animation>()["Hide"].length);
		}
		UIPlayAnimation[] buttonAnimations = m_InputTrigger.GetComponentsInChildren<UIPlayAnimation>();
		UIPlayAnimation[] array = buttonAnimations;
		foreach (UIPlayAnimation UIPlayAnimation in array)
		{
			UIPlayAnimation.enabled = true;
		}
		if (m_SelectionFrame != null)
		{
			UnityEngine.Object.Destroy(m_SelectionFrame);
		}
	}

	public void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_InputTrigger)
		{
			m_InputTrigger.Clicked += RaiseOnUsed;
		}
	}

	public void DeRegisterEventHandler()
	{
		if ((bool)m_InputTrigger)
		{
			m_InputTrigger.Clicked -= RaiseOnUsed;
		}
	}

	private void OnDisable()
	{
		DeRegisterEventHandler();
	}

	public void RefreshAssets(IInventoryItemGameData inventoryItemGameData)
	{
		SetModel(inventoryItemGameData, false);
	}

	internal void SetUsed(bool used)
	{
		m_Used = used;
	}
}
