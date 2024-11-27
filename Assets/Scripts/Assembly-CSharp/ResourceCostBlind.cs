using UnityEngine;

public class ResourceCostBlind : MonoBehaviour
{
	[SerializeField]
	private UISprite m_Icon;

	[SerializeField]
	public UILabel m_Value;

	[SerializeField]
	private UILabel m_YouHaveText;

	[SerializeField]
	public UISprite m_CostBody;

	public void CenterValue()
	{
		if ((bool)m_Value)
		{
			m_Value.transform.localPosition = new Vector3(0f, m_Value.transform.localPosition.y, m_Value.transform.localPosition.z);
			m_Value.lineWidth = 100;
		}
	}

	public void CenterValueY()
	{
		if ((bool)m_Value)
		{
			m_Value.transform.localPosition = new Vector3(m_Value.transform.localPosition.x, 0f, m_Value.transform.localPosition.z);
		}
	}

	public void SetModel(string assetId, UIAtlas atlas, float value, string youHaveText = "", bool ignoreforfree = false)
	{
		if (atlas != null && m_Icon != null)
		{
			m_Icon.atlas = atlas;
		}
		if (m_Icon != null)
		{
			m_Icon.gameObject.SetActive(!string.IsNullOrEmpty(assetId));
			if (m_Icon.atlas.name.Contains("Arena") || m_Icon.atlas.name.Contains("Event") && assetId.Equals("Resource_Coin"))
				assetId = "Snoutling";
			
			m_Icon.spriteName = assetId;
		}
		if (value == 0f && !ignoreforfree)
		{
			m_Value.text = DIContainerInfrastructure.GetLocaService().Tr("gen_offer_free", "FREE!");
			m_Icon.gameObject.SetActive(false);
		}
		else
		{
			m_Value.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(Mathf.RoundToInt(value));
		}
		if ((bool)m_YouHaveText)
		{
			m_YouHaveText.text = youHaveText;
		}
	}

	public void SetLabelOnly(float value)
	{
		m_Value.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(Mathf.RoundToInt(value));
	}

	public void SetModel(string assetId, UIAtlas atlas, string value, string youHaveText = "", bool ignoreforfree = false)
	{
		if (atlas != null && m_Icon != null)
		{
			m_Icon.atlas = atlas;
		}
		if (m_Icon != null)
		{
			m_Icon.gameObject.SetActive(!string.IsNullOrEmpty(assetId));
			m_Icon.spriteName = assetId;
		}
		m_Value.text = value;
		if ((bool)m_YouHaveText)
		{
			m_YouHaveText.text = youHaveText;
		}
	}

	public void SetColor(Color color)
	{
		m_Value.color = color;
	}
}
