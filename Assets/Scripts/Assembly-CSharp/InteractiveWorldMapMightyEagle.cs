using System.Collections;
using System.Linq;
using ABH.GameDatas;
using UnityEngine;

internal class InteractiveWorldMapMightyEagle : MonoBehaviour
{
	[SerializeField]
	protected GameObject m_CharacterRoot;

	[SerializeField]
	protected GameObject m_BubbleRoot;

	[SerializeField]
	protected float m_SpeechBubbleLength = 2f;

	[SerializeField]
	protected HotSpotWorldMapViewBase m_DojoHotspot;

	private bool m_IsPlaying;

	private void Start()
	{
		if (!CheckVisibilityRequirement())
		{
			m_CharacterRoot.SetActive(false);
			m_BubbleRoot.SetActive(false);
		}
		else
		{
			m_CharacterRoot.SetActive(true);
		}
	}

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	public virtual void HandleClicked()
	{
		if (!m_IsPlaying && IsResponsive())
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "mighty_eagle_dojo", out data))
			{
				string param = m_DojoHotspot.Model.BalancingData.HotspotContents.Keys.FirstOrDefault();
				DIContainerInfrastructure.LocationStateMgr.ShowDojoScreen(param, m_DojoHotspot);
			}
			else
			{
				StartCoroutine(PlayResponseAnimations());
			}
		}
	}

	public virtual bool IsResponsive()
	{
		if (!m_CharacterRoot.activeInHierarchy)
		{
			return false;
		}
		return true;
	}

	protected virtual bool CheckVisibilityRequirement()
	{
		return true;
	}

	protected virtual IEnumerator PlayResponseAnimations()
	{
		m_IsPlaying = true;
		m_BubbleRoot.SetActive(true);
		m_CharacterRoot.PlayAnimationOrAnimatorState("Talk");
		m_BubbleRoot.PlayAnimationOrAnimatorState("Bubble_Show");
		yield return new WaitForSeconds(m_SpeechBubbleLength);
		m_BubbleRoot.PlayAnimationOrAnimatorState("Bubble_Hide");
		yield return new WaitForSeconds(m_BubbleRoot.GetAnimationOrAnimatorStateLength("Bubble_Hide"));
		m_IsPlaying = false;
	}
}
