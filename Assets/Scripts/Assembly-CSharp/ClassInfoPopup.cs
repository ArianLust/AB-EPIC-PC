using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class ClassInfoPopup : MonoBehaviour
{
	[SerializeField]
	private UISprite m_AttackIcon;

	[SerializeField]
	private UISprite m_SupportIcon;

	[SerializeField]
	private UISprite m_RageIcon;

	[SerializeField]
	private UILabel m_AttackText;

	[SerializeField]
	private UILabel m_SupportText;

	[SerializeField]
	private UILabel m_RageText;

	[SerializeField]
	private UILabel m_AttackHeaderText;

	[SerializeField]
	private UILabel m_SupportHeaderText;

	[SerializeField]
	private UILabel m_RageHeaderText;

	[SerializeField]
	private UILabel m_ClassTitle;

	[SerializeField]
	private UILabel m_ClassDescription;

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private Animation m_PopupAnimation;

	[SerializeField]
	private Transform m_ClassParent;

	[SerializeField]
	private CharacterControllerCamp m_CharacterControllerPrefab;

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(7, ClosePopup);
		m_CloseButton.Clicked += ClosePopup;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(7);
		m_CloseButton.Clicked -= ClosePopup;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		m_PopupAnimation.Play("Popup_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(6u);
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(false);
	}

	public void Show(ClassItemGameData birdClass, SkinItemGameData birdSkin = null)
	{
		base.gameObject.SetActive(true);
		BirdGameData birdGameData = null;
		foreach (BirdGameData bird in DIContainerInfrastructure.GetCurrentPlayer().Birds)
		{
			if (bird.BalancingData.NameId == birdClass.BalancingData.RestrictedBirdId)
			{
				birdGameData = new BirdGameData(bird);
				break;
			}
		}
		if (birdGameData == null)
		{
			birdGameData = new BirdGameData(birdClass.BalancingData.RestrictedBirdId);
		}
		if (birdSkin != null)
		{
			m_ClassTitle.text = birdSkin.ItemLocalizedName;
			DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { birdSkin }, InventoryItemType.Skin, birdGameData.InventoryGameData);
		}
		else
		{
			m_ClassTitle.text = birdClass.ItemLocalizedName;
		}
		birdGameData.OverrideClassItem = birdClass;
		InitSkills(birdClass, birdGameData);
		SpawnCharacter(birdGameData);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("class_info");
		m_PopupAnimation.Play("Popup_Enter");
		yield return new WaitForSeconds(0.1f);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 6u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		yield return new WaitForSeconds(0.5f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("class_info");
		RegisterEventHandler();
	}

	private void SpawnCharacter(BirdGameData bird)
	{
		if (m_ClassParent.childCount > 0)
		{
			Object.Destroy(m_ClassParent.GetChild(0).gameObject);
		}
		CharacterControllerCamp characterControllerCamp = Object.Instantiate(m_CharacterControllerPrefab);
		characterControllerCamp.transform.parent = m_ClassParent;
		characterControllerCamp.transform.localPosition = Vector3.zero;
		characterControllerCamp.SetModel(bird, false, true, false);
		characterControllerCamp.gameObject.layer = LayerMask.NameToLayer("Interface");
		characterControllerCamp.transform.localScale = Vector3.one;
		characterControllerCamp.m_AssetController.transform.localScale = Vector3.one;
		characterControllerCamp.m_AssetController.gameObject.layer = LayerMask.NameToLayer("Interface");
		if ((bool)characterControllerCamp.GetComponentInChildren<BoxCollider>())
		{
			characterControllerCamp.GetComponentInChildren<BoxCollider>().enabled = false;
		}
	}

	private void InitSkills(ClassItemGameData birdClass, BirdGameData selectedBird)
	{
		m_ClassDescription.text = birdClass.ItemLocalizedDesc;
		m_AttackHeaderText.text = birdClass.PrimarySkill.SkillLocalizedName;
		m_SupportHeaderText.text = birdClass.SecondarySkill.SkillLocalizedName;
		m_RageHeaderText.text = selectedBird.Skills[2].SkillLocalizedName;
		m_AttackText.text = birdClass.PrimarySkill.GetIndepthDescription();
		m_SupportText.text = birdClass.SecondarySkill.GetIndepthDescription();
		m_RageText.text = selectedBird.Skills[2].GetIndepthDescription();
		SetSkillIcon(m_AttackIcon, birdClass.PrimarySkill);
		SetSkillIcon(m_SupportIcon, birdClass.SecondarySkill);
		SetSkillIcon(m_RageIcon, selectedBird.Skills[2]);
	}

	private void SetSkillIcon(UISprite skillIcon, SkillGameData skill)
	{
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(skill.Balancing.IconAtlasId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(skill.Balancing.IconAtlasId) as GameObject;
			skillIcon.atlas = gameObject.GetComponent<UIAtlas>();
		}
		skillIcon.spriteName = skill.m_SkillIconName;
		skillIcon.MakePixelPerfect();
	}
}
