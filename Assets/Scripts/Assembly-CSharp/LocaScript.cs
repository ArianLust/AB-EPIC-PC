using UnityEngine;

[AddComponentMenu("Chimera/LocaScript")]
public class LocaScript : MonoBehaviour
{
	public string m_locaIdent;

	public string m_defaultText;

	public bool m_startupLoca;

	private TextMesh textMesh
	{
		get
		{
			return GetComponent<TextMesh>();
		}
	}

	private UILabel label
	{
		get
		{
			return GetComponent<UILabel>();
		}
	}

	public void ReloadLoca()
	{
		SetLoca();
	}

	public void SetLoca()
	{
		if (!string.IsNullOrEmpty(m_locaIdent))
		{
			ABHLocaService aBHLocaService = ((!m_startupLoca) ? DIContainerInfrastructure.GetLocaService() : DIContainerInfrastructure.GetStartupLocaService());
			string text = aBHLocaService.Tr(m_locaIdent, m_defaultText);
			if (label != null)
			{
				label.text = text;
			}
			else if (textMesh != null)
			{
				textMesh.text = text;
			}
		}
	}

	private void Start()
	{
		SetLoca();
		InvokeRepeating("CheckLocaFile", 0f, 1f);
	}

	private void CheckLocaFile()
	{
		if (DIContainerInfrastructure.GetLocaService().LocaConfig.LocaDictionary != null)
		{
			SetLoca();
			CancelInvoke();
		}
	}
}
