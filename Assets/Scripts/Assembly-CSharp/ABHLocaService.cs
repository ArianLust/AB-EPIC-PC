using System.Collections;
using System.Collections.Generic;
using ABH.Shared.Generic;
using ChBgLib.Text;
using UnityEngine;

public class ABHLocaService
{
	private readonly LocaHelper locaHelper;

	private Dictionary<string, string> IsoCodeToLanguageMap;

	private Dictionary<string, string> LanguageToIsoCodeMap;

	private readonly ILocaLoader m_locaLoader;

	private List<string> m_supportedLanguages = new List<string>(new string[11]
	{
		"Chinese (Simplified)", "Chinese (Traditional)", "English", "French", "German", "Italian", "Japanese", "Korean", "Portuguese (Brazil)", "Russian",
		"Spanish"
	});

	private string m_currentLanguageRegion;

	private string m_currentLanguageAlpha2Code;

	public static string DefaultLanguageName
	{
		get
		{
			return "English";
		}
	}

	public LocaConfig LocaConfig { get; set; }

	public bool Initialized { get; private set; }

	public string CurrentLanguageKey { get; private set; }

	public List<string> SupportedLanguages
	{
		get
		{
			return m_supportedLanguages;
		}
	}

	public bool IsAsynch
	{
		get
		{
			return m_locaLoader != null && m_locaLoader.IsAsynch;
		}
	}

	public ABHLocaService(ILocaLoader locaLoader, LocaHelper ilocaHelper)
	{
		m_locaLoader = locaLoader;
		LocaConfig = new LocaConfig();
		locaHelper = ilocaHelper.SetLocaConfig(LocaConfig);
	}

	private string GetLanguageCodeWithRegion() // FIX
	{
		string text = null;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
			{
				text = androidJavaObject.Call<string>("toString", new object[0]);
			}
		}
		DebugLog.Log(GetType(), "Found system language code " + text);
		return text;
	}

	private void ExtractLanguageCodeAndRegion(string languageCodeWithRegion)
	{
		if (!string.IsNullOrEmpty(languageCodeWithRegion))
		{
			languageCodeWithRegion = languageCodeWithRegion.Replace("-", "_");
			string[] array = languageCodeWithRegion.Split('_');
			m_currentLanguageAlpha2Code = ((!languageCodeWithRegion.Contains("_")) ? languageCodeWithRegion : array[0]);
			m_currentLanguageRegion = ((!languageCodeWithRegion.Contains("_") || array.Length <= 1) ? languageCodeWithRegion : array[1]);
		}
	}

	private string CleanupLanguageName(string languageName)
	{
		// since it seems some languages just completely break the protobuf reader, im just gonna make them use english
		if (languageName.Contains("Chinese") || languageName.Contains("Japanese") || languageName.Contains("Korean"))
			languageName = "English";
		
		return languageName.Replace("(", "_").Replace(")", "_").Replace(" ", "_");
	}

	public ABHLocaService InjectLocaAsset(List<TextAsset> textAsset)
	{
		DebugLog.Log(GetType(), "InjectLocaAsset");
		m_locaLoader.InjectLocaAsset(textAsset);
		return this;
	}

	private IEnumerator LoadLocaAsynch(MonoBehaviour synchObject, string languageName)
	{
		DebugLog.Log(GetType(), "Asynch loading loca for: " + languageName);
		yield return synchObject.StartCoroutine(m_locaLoader.LoadAsync(languageName, delegate(Dictionary<string, string> loca)
		{
			LocaConfig.LocaDictionary = loca;
			DebugLog.Log(GetType(), "Loca fully initialized. Loaded " + LocaConfig.LocaDictionary.Count + " entries.");
			Initialized = true;
		}));
	}

	private void LoadLocaSync(string languageName)
	{
		DebugLog.Log(GetType(), "Sync loading loca for: " + languageName);
		LocaConfig.LocaDictionary = m_locaLoader.Load(languageName);
		DebugLog.Log(GetType(), "Loca fully initialized. Loaded " + LocaConfig.LocaDictionary.Count + " entries.");
		Initialized = true;
	}

	public void InitDefaultLoca(MonoBehaviour synchMonoBehaviour = null, string overrideLanguage = null)
	{
		if (string.IsNullOrEmpty(overrideLanguage))
		{
			string text = Application.systemLanguage.ToString();
			DebugLog.Log(GetType(), "System language is: " + text);
			if (IsLanguageSupported(text))
			{
				if (text == "Chinese")
				{
					DebugLog.Log(GetType(), "Checking for region settings...");
					string languageCodeWithRegion = GetLanguageCodeWithRegion();
					DebugLog.Log(GetType(), "found language with region: " + languageCodeWithRegion);
					ExtractLanguageCodeAndRegion(languageCodeWithRegion);
					string languageName = "Chinese (Simplified)";
					if (!string.IsNullOrEmpty(m_currentLanguageRegion))
					{
						if (m_currentLanguageRegion.ToLower().Equals("hant") || m_currentLanguageRegion.ToLower().Equals("tw"))
						{
							languageName = "Chinese (Traditional)";
						}
						else if (m_currentLanguageRegion.ToLower().Equals("hans"))
						{
							languageName = "Chinese (Simplified)";
						}
					}
					CurrentLanguageKey = CleanupLanguageName(languageName);
				}
				else if (text == "Portuguese")
				{
					CurrentLanguageKey = CleanupLanguageName("Portuguese (Brazil)");
				}
				else
				{
					CurrentLanguageKey = CleanupLanguageName(text);
				}
			}
			else
			{
				CurrentLanguageKey = CleanupLanguageName(DefaultLanguageName);
			}
		}
		else
		{
			CurrentLanguageKey = CleanupLanguageName(overrideLanguage);
		}
		DebugLog.Log(GetType(), "CurrentLanguageKey is: " + CurrentLanguageKey);
		FontInitializer fontInitializer = ((!ContentLoader.Instance) ? null : ContentLoader.Instance.GetComponent<FontInitializer>());
		if ((bool)fontInitializer)
		{
			fontInitializer.ReplaceFontsIfNeeded(CurrentLanguageKey);
		}
		if (IsAsynch && synchMonoBehaviour != null)
		{
			synchMonoBehaviour.StartCoroutine(LoadLocaAsynch(synchMonoBehaviour, CurrentLanguageKey));
		}
		else
		{
			LoadLocaSync(CurrentLanguageKey);
		}
	}

	private bool IsLanguageSupported(string systemLanguage)
	{
		foreach (string supportedLanguage in SupportedLanguages)
		{
			if (supportedLanguage.Contains(systemLanguage))
			{
				return true;
			}
		}
		return false;
	}

	public string Tr(string ident, string dummyText)
	{
		return locaHelper.Tr(ident, dummyText);
	}

	public string Tr(string ident)
	{
		return locaHelper.Tr(ident);
	}

	public string Tr(string ident, Dictionary<string, string> replacementStrings)
	{
		return locaHelper.Tr(ident, replacementStrings);
	}

	public string GetSkillName(string ident, Dictionary<string, string> replacementStrings)
	{
		return locaHelper.Tr(ident + "_name", replacementStrings);
	}

	public string GetSkillDescriptions(string ident, Dictionary<string, string> replacementStrings)
	{
		return locaHelper.Tr(ident + "_desc", replacementStrings);
	}

	public string GetExtendedSkillDescriptions(string ident)
	{
		return locaHelper.Tr(ident + "_detail");
	}

	public string GetCharacterName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	public string ReplaceUnmappableCharacters(string localizedString)
	{
		if (string.IsNullOrEmpty(localizedString))
		{
			return localizedString;
		}
		localizedString = localizedString
			.Replace("æ", "e")
			.Replace("ø", "oe")
			.Replace("å", "aa")
			.Replace("æ", "ae")
			.Replace("Æ", "Ae")
			.Replace("Ø", "Oe")
			.Replace("Ǿ", "Oe")
			.Replace("ǿ", "oe")
			.Replace("ǽ", "ae")
			.Replace("Ǽ", "Ae")
			.Replace("Å", "Aa")
			.Replace("ı", "i");
		return localizedString;
	}

	internal string GetEquipmentName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetClassName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetBannerItemName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetZoneName(string ident)
	{
		return locaHelper.Tr(ident);
	}

	internal string GetCraftingResourceName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetCraftingResourceDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetMasteryItemName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetMasteryItemDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetRecipeName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetConsumableName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetConsumableDesc(string ident, Dictionary<string, string> replacementTag)
	{
		return locaHelper.Tr(ident + "_desc", replacementTag);
	}

	internal string GetInventoryItemTypeName(InventoryItemType m_selectedCategory)
	{
		switch (m_selectedCategory)
		{
		default:
			return locaHelper.Tr(m_selectedCategory.ToString().ToLower() + "_name");
		}
	}

	internal string GetCraftingMenuName(CraftingMenuType m_MenuType)
	{
		switch (m_MenuType)
		{
		default:
			return locaHelper.Tr(m_MenuType.ToString().ToLower() + "_name");
		}
	}

	public string GetShopTypeName(ShopMenuType m_MenuType)
	{
		return locaHelper.Tr(m_MenuType.ToString().ToLower() + "_name");
	}

	public string GetShopTypeContentName(ShopMenuType m_MenuType)
	{
		return locaHelper.Tr(m_MenuType.ToString().ToLower() + "_content");
	}

	internal string GetItemName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetItemDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetClassDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetRecipeDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetEquipmentDesc(string ident, Dictionary<string, string> replacementTag)
	{
		return locaHelper.Tr(ident + "_desc", replacementTag);
	}

	internal string GetShopOfferName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetShopOfferDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetGlobalShopCategoryName(string ident)
	{
		return locaHelper.Tr(ident + "_name");
	}

	internal string GetGlobalShopCategoryDesc(string ident)
	{
		return locaHelper.Tr(ident + "_desc");
	}

	internal string GetItemTooltipDesc(string ident, Dictionary<string, string> replacementTag)
	{
		return locaHelper.Tr(ident + "_tt", replacementTag);
	}

	internal string GetPerkDesc(PerkType perkType, Dictionary<string, string> replacementTags)
	{
		return locaHelper.Tr("perk_" + perkType.ToString().ToLower() + "_desc", replacementTags);
	}

	internal string GetPerkName(PerkType perkType)
	{
		return locaHelper.Tr("perk_" + perkType.ToString().ToLower() + "_name");
	}

	internal string GetClassNamePrefix(int level)
	{
		return locaHelper.Tr("gen_class_prefix_" + level.ToString("00"));
	}

	internal string GetMailboxMessageName(MessageType messageType)
	{
		return locaHelper.Tr("msg_" + messageType.ToString().ToLower() + "_name");
	}

	internal string GetInventoryItemTypeCraftingDesc(InventoryItemType selectedCategory)
	{
		return locaHelper.Tr(selectedCategory.ToString().ToLower() + "_craft_desc");
	}

	internal string GetLeagueName(int league)
	{
		return Tr("pvp_league_" + league.ToString("00") + "_name");
	}
}
