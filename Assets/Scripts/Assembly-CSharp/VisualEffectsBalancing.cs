using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsBalancing : MonoBehaviour
{
	public List<VisualEffectSetting> VisualEffectSettings = new List<VisualEffectSetting>();

	private Dictionary<string, VisualEffectSetting> VisualEffectSettingsLookUp = new Dictionary<string, VisualEffectSetting>();

	public List<BubbleSetting> BubbleSettings = new List<BubbleSetting>();

	private Dictionary<string, BubbleSetting> BubbleSettingsLookUp = new Dictionary<string, BubbleSetting>();

	public GenericAssetProvider m_EffectAssetProvider;

	public Material m_RecipeItemMaterial;

	public Material m_ClassItemUnavailableMaterial;

	[SerializeField]
	private CharacterSpeechBubble m_LargeBubble;

	[SerializeField]
	private CharacterSpeechBubble m_KoBubble;

	[SerializeField]
	private CharacterSpeechBubble m_OnlyTimerBubble;

	[SerializeField]
	private CharacterSpeechBubble m_OnlyIconBubble;

	[SerializeField]
	private CharacterSpeechBubble m_TargetedBubble;

	[SerializeField]
	private string m_CoinsEffect;

	[SerializeField]
	private string m_LuckyCoinsEffect;

	[SerializeField]
	private string m_ExpEffect;

	[SerializeField]
	private string m_MasteryEffect;

	[SerializeField]
	private string m_BonusEffect;

	[SerializeField]
	private Color m_ColorOffersBuyable = new Color(64f, 32f, 0f);

	[SerializeField]
	private Color m_ColorOffersNotBuyable = Color.red;

	[SerializeField]
	private float m_ZPosGuides = -250f;

	[SerializeField]
	private float m_EffectSizeFactorBoss = 1.5f;

	[SerializeField]
	private float m_EffectSizeFactorLarge = 1f;

	[SerializeField]
	private float m_EffectSizeFactorMedium = 0.85f;

	[SerializeField]
	private float m_EffectSizeFactorSmall = 0.65f;

	[SerializeField]
	private float m_FriendsZOffset = -40f;

	public CharacterSpeechBubble LargeBubble
	{
		get
		{
			return m_LargeBubble;
		}
	}

	public CharacterSpeechBubble KoBubble
	{
		get
		{
			return m_KoBubble;
		}
	}

	public CharacterSpeechBubble OnlyTimerBubble
	{
		get
		{
			return m_OnlyTimerBubble;
		}
	}

	public CharacterSpeechBubble OnlyIconBubble
	{
		get
		{
			return m_OnlyIconBubble;
		}
	}

	public CharacterSpeechBubble TargetedBubble
	{
		get
		{
			return m_TargetedBubble;
		}
	}

	public string CoinsEffect
	{
		get
		{
			return m_CoinsEffect;
		}
	}

	public string LuckyCoinsEffect
	{
		get
		{
			return m_LuckyCoinsEffect;
		}
	}

	public string ExpEffect
	{
		get
		{
			return m_ExpEffect;
		}
	}

	public string MasteryEffect
	{
		get
		{
			return m_MasteryEffect;
		}
	}

	public string BonusEffect
	{
		get
		{
			return m_BonusEffect;
		}
	}

	public Color ColorOffersBuyable
	{
		get
		{
			return m_ColorOffersBuyable;
		}
	}

	public Color ColorOffersNotBuyable
	{
		get
		{
			return m_ColorOffersNotBuyable;
		}
	}

	public float EffectSizeFactorBoss
	{
		get
		{
			return m_EffectSizeFactorBoss;
		}
	}

	public float EffectSizeFactorLarge
	{
		get
		{
			return m_EffectSizeFactorLarge;
		}
	}

	public float EffectSizeFactorMedium
	{
		get
		{
			return m_EffectSizeFactorMedium;
		}
	}

	public float EffectSizeFactorSmall
	{
		get
		{
			return m_EffectSizeFactorSmall;
		}
	}

	public float FriendsZOffset
	{
		get
		{
			return m_FriendsZOffset;
		}
	}

	public float ZPosGuides
	{
		get
		{
			return m_ZPosGuides;
		}
	}

	private void Awake()
	{
		foreach (VisualEffectSetting visualEffectSetting in VisualEffectSettings)
		{
			VisualEffectSettingsLookUp.Add(visualEffectSetting.BalancingId, visualEffectSetting);
		}
		foreach (BubbleSetting bubbleSetting in BubbleSettings)
		{
			BubbleSettingsLookUp.Add(bubbleSetting.BalancingId, bubbleSetting);
		}
	}

	public bool TryGetVisualEffectSetting(string ident, out VisualEffectSetting setting)
	{
		return VisualEffectSettingsLookUp.TryGetValue(ident, out setting);
	}

	public bool TryGetBubbleSetting(string ident, out BubbleSetting setting)
	{
		return BubbleSettingsLookUp.TryGetValue(ident, out setting);
	}
}
