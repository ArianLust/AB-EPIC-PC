using System.Collections.Generic;
using UnityEngine;

public class AdCanvas : MonoBehaviour
{
	[SerializeField]
	private UITexture m_Texture;

	private Texture2D m_RenderableAdContent;

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private UIInputTrigger m_Button;

	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	public string m_Placement;

	private string m_overridePlacement;

	public UISprite m_Grayout;

	private bool m_valid;

	private bool m_Showing;

	public bool m_AdAvailable;

	public static Texture2D m_CachedTexture;

	public bool IsShowing
	{
		get
		{
			return m_Showing;
		}
	}

	public bool IsAdAvailable
	{
		get
		{
			return IsAdAvailable;
		}
	}

	private void Awake()
	{
		DebugLog.Log(GetType(), "Awake");
		m_AdAvailable = true;
		m_Showing = false;
	}

	private void SetDisplayEnabled(bool isTrue)
	{
		DebugLog.Log(GetType(), "SetDisplayEnabled: " + isTrue);
		m_Showing = isTrue;
		base.gameObject.SetActive(isTrue);
		if (m_Texture != null)
		{
			m_Texture.enabled = isTrue;
		}
		if (m_Grayout != null)
		{
			m_Grayout.enabled = isTrue;
		}
		if (m_Collider != null)
		{
			m_Collider.enabled = isTrue;
		}
		if (string.IsNullOrEmpty(m_overridePlacement))
		{
			m_overridePlacement = m_Placement;
		}
		if (isTrue)
		{
			DIContainerInfrastructure.AdService.TrackNativeAdImpression(m_overridePlacement);
		}
	}

	private void Clicked()
	{
		if (string.IsNullOrEmpty(m_overridePlacement))
		{
			m_overridePlacement = m_Placement;
		}
		DIContainerInfrastructure.AdService.HandleClick(m_overridePlacement);
	}

	public bool RenderAd()
	{
		if (m_RenderableAdContent == null)
		{
			DebugLog.Log(GetType(), "RenderAd: No renderable content available!");
			return false;
		}
		m_valid = true;
		SetDisplayEnabled(true);
		m_Button.Clicked -= Clicked;
		m_Button.Clicked += Clicked;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("XPROMO");
		Invoke("AllowLeaving", 1f);
		m_Texture.mainTexture = m_RenderableAdContent;
		return true;
	}

	private void AllowLeaving()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("XPROMO");
		m_CloseButton.Clicked -= OnRenderableHide;
		m_CloseButton.Clicked += OnRenderableHide;
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(9, OnRenderableHide);
	}

	public bool Hatch2_OnRenderableReady(string placement, string contentType, List<byte> content, bool isNative)
	{
		DebugLog.Log(GetType(), "OnRenderableReadyData: " + placement + ", type: " + contentType + ", length: " + content.Count);
		if (m_CachedTexture != null)
		{
			m_overridePlacement = "InGameNative.MapPromo";
			m_RenderableAdContent = m_CachedTexture;
			return RenderAd();
		}
		if (contentType.StartsWith("image/"))
		{
			Texture2D texture2D = new Texture2D(1, 1);
			byte[] array = new byte[content.Count];
			content.CopyTo(array);
			if (texture2D.LoadImage(array))
			{
				if (!isNative)
				{
					m_overridePlacement = m_Placement;
					m_AdAvailable = true;
					m_RenderableAdContent = texture2D;
					return RenderAd();
				}
				m_overridePlacement = "InGameNative.MapPromo";
				Color[] pixels = texture2D.GetPixels(0, 180, 1024, 576);
				Texture2D texture2D2 = new Texture2D(1024, 576);
				texture2D2.SetPixels(pixels);
				texture2D2.Apply();
				m_CachedTexture = texture2D2;
				m_RenderableAdContent = m_CachedTexture;
				return RenderAd();
			}
			DebugLog.Error(GetType(), "Hatch2_OnRenderableReady: adTexture loading failed!");
			m_AdAvailable = false;
			return false;
		}
		DebugLog.Warn(GetType(), "Hatch2_OnRenderableReady: contentType is no image. No implementation available to render contentType: " + contentType);
		m_AdAvailable = false;
		return false;
	}

	public void OnRenderableShow(string placement)
	{
		if ((bool)m_Texture && (bool)m_Texture.mainTexture)
		{
			DebugLog.Log("[AdCanvas] OnRenderableShow: Texture is: " + m_Texture.mainTexture.width + ", " + m_Texture.mainTexture.height);
		}
		SetDisplayEnabled(true);
	}

	public void OnRenderableHide()
	{
		DebugLog.Log("[AdCanvas] OnRenderableHide");
		m_CloseButton.Clicked -= OnRenderableHide;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(9);
		m_AdAvailable = false;
		SetDisplayEnabled(false);
		(DIContainerInfrastructure.LocationStateMgr.WorldMenuUI as WorldMapMenuUI).ComebackFromCrossPromoAd();
	}

	public void OnDestroy()
	{
		if (m_Texture != null)
		{
			m_Texture.mainTexture = null;
		}
		m_Placement = string.Empty;
		if ((bool)m_Grayout)
		{
			m_Grayout.enabled = false;
		}
		if ((bool)m_Button)
		{
			m_Button.Clicked -= Clicked;
		}
	}

	public bool OnRenderableReadyData(string placement, string contentType, byte[] data)
	{
		return true;
	}
}
