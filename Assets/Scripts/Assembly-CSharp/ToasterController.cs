using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class ToasterController : MonoBehaviour
{
	[SerializeField]
	private UISprite m_Icon;

	[SerializeField]
	private AutoScalingTextBox m_TextBox;

	[SerializeField]
	private UIGrid m_Grid;

	[SerializeField]
	private UILabel m_FixedText;

	[SerializeField]
	private List<LootDisplayContoller> m_Items;

	public void SetMessage(string message)
	{
		base.gameObject.SetActive(true);
		if ((bool)m_TextBox)
		{
			m_TextBox.SetText(message);
		}
	}

	public void SetMessage(string message, string assetId)
	{
		base.gameObject.SetActive(true);
		if ((bool)m_TextBox)
		{
			m_TextBox.SetText(message);
		}
		if ((bool)m_Icon)
		{
			m_Icon.spriteName = assetId;
		}
	}

	public void SetMessage(string message, List<IInventoryItemGameData> items)
	{
		base.gameObject.SetActive(true);
		if ((bool)m_TextBox)
		{
			m_TextBox.SetText(message);
		}
		if ((bool)m_FixedText)
		{
			m_FixedText.text = message;
		}
		if (items != null)
		{
			int num = 0;
			for (num = 0; num < Mathf.Min(items.Count, m_Items.Count); num++)
			{
				m_Items[num].gameObject.SetActive(true);
				m_Items[num].SetModel(items[num], new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			for (int i = num; i < m_Items.Count; i++)
			{
				m_Items[i].gameObject.SetActive(false);
			}
			StartCoroutine(RefreshGrid());
		}
	}

	private IEnumerator RefreshGrid()
	{
		yield return new WaitForEndOfFrame();
		if ((bool)m_Grid)
		{
			m_Grid.Reposition();
		}
	}

	public float Enter()
	{
		base.gameObject.SetActive(true);
		GetComponent<Animation>().Play("Toaster_Enter");
		return GetComponent<Animation>()["Toaster_Enter"].clip.length;
	}

	public float Leave()
	{
		GetComponent<Animation>().Play("Toaster_Leave");
		Invoke("Disable", GetComponent<Animation>()["Toaster_Leave"].clip.length);
		return GetComponent<Animation>()["Toaster_Leave"].clip.length;
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
	}
}
