using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using UnityEngine;

public class BattlePrepCharacterButton : MonoBehaviour
{
	[SerializeField]
	private GameObject m_CampViewController;

	[SerializeField]
	private Animation m_ButtonAnim;

	[SerializeField]
	private Transform m_BirdRoot;

	private bool m_selected;

	private BirdGameData m_bird;

	private bool m_IsSelectable = true;

	private CharacterControllerCamp m_Controller;

	private BattlePreperationUI m_PreperationUi;

	private ArenaBattlePreperationUI m_ArenaPreperationUi;

	private bool m_PlayAnimationOnNextSelect;

	[method: MethodImpl(32)]
	public event Action Selected;

	public void Init(BirdGameData bird, BattlePreperationUI preperationUi)
	{
		m_PreperationUi = preperationUi;
		m_bird = bird;
		CreateBird();
		RemoveScaleControllerComponent();
		m_selected = false;
		m_PlayAnimationOnNextSelect = true;
		m_IsSelectable = true;
		RegisterEventHandler();
	}

	public void Init(BirdGameData bird, ArenaBattlePreperationUI preperationUi)
	{
		m_ArenaPreperationUi = preperationUi;
		m_bird = bird;
		CreateBird();
		RemoveScaleControllerComponent();
		m_selected = false;
		m_PlayAnimationOnNextSelect = true;
		m_IsSelectable = true;
		RegisterEventHandler();
	}

	public void PlayCharacterIdle()
	{
		if (m_Controller != null)
		{
			m_Controller.PlayIdleAnimation();
		}
	}

	public void Refresh()
	{
		Select(m_selected, true);
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		if ((bool)m_Controller)
		{
			m_Controller.BirdClicked += OnButtonClicked;
		}
	}

	public void Selectable(bool isSelectable)
	{
		m_IsSelectable = isSelectable;
	}

	public bool IsSelectable()
	{
		return m_IsSelectable;
	}

	public void Select(bool select, bool forced = false)
	{
		if ((m_IsSelectable || forced) && (select != m_selected || forced))
		{
			if (base.gameObject.activeInHierarchy)
			{
				m_ButtonAnim.Play((!select) ? "CharacterSlot_Deselected" : "CharacterSlot_Selected");
			}
			else
			{
				m_PlayAnimationOnNextSelect = true;
			}
			m_selected = select;
			if (this.Selected != null)
			{
				this.Selected();
			}
		}
	}

	private void OnEnable()
	{
		if (m_PlayAnimationOnNextSelect)
		{
			m_ButtonAnim.Play((!m_selected) ? "CharacterSlot_Deselected" : "CharacterSlot_Selected");
			m_PlayAnimationOnNextSelect = false;
		}
	}

	private void OnButtonClicked(ICharacter obj)
	{
		if (!m_IsSelectable)
		{
			return;
		}
		if (m_PreperationUi != null)
		{
			if (m_PreperationUi.m_OneBirdLeft && m_selected)
			{
				return;
			}
		}
		else if (m_ArenaPreperationUi.m_OneBirdLeft && m_selected)
		{
			return;
		}
		m_selected = !m_selected;
		if (m_selected)
		{
			m_ButtonAnim.Play("CharacterSlot_Selected");
		}
		else
		{
			m_ButtonAnim.Play("CharacterSlot_Deselected");
		}
		if (m_selected)
		{
			StartCoroutine(ToggleCheerAnim());
		}
		else
		{
			StartCoroutine(ToggleMourneAnimation());
		}
		if (this.Selected != null)
		{
			this.Selected();
		}
	}

	private IEnumerator ToggleMourneAnimation()
	{
		m_Controller.m_AssetController.PlayAnimation("Move_Once");
		yield return new WaitForSeconds(m_Controller.m_AssetController.GetAnimationLength("Move_Once"));
		m_Controller.PlayMourneAnimation();
	}

	private IEnumerator ToggleCheerAnim()
	{
		m_Controller.m_AssetController.PlayAnimation("Move_Once");
		yield return new WaitForSeconds(m_Controller.m_AssetController.GetAnimationLength("Move_Once"));
		m_Controller.PlayCheerCharacter();
	}

	private void DeregisterEventHandler()
	{
		if ((bool)m_Controller)
		{
			m_Controller.BirdClicked -= OnButtonClicked;
		}
	}

	public bool IsSelected()
	{
		return m_selected;
	}

	public BirdGameData GetBirdGameData()
	{
		return m_bird;
	}

	private void CreateBird()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_CampViewController, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = m_BirdRoot.transform;
		gameObject.transform.localPosition = Vector3.zero;
		m_Controller = gameObject.GetComponent<CharacterControllerCamp>();
		m_Controller.SetModel(m_bird);
		SetLayerRecusively(m_BirdRoot.gameObject, 8);

		var audioSource = m_BirdRoot.GetComponentInChildren<AudioSource>();
		if (audioSource) audioSource.spatialBlend = 0f; // fix sound bug..
	}

	public void RemoveScaleControllerComponent()
	{
		ScaleController[] componentsInChildren = base.gameObject.GetComponentsInChildren<ScaleController>();
		if (componentsInChildren != null && !(ScaleMgr.Instance == null))
		{
			ScaleController[] array = componentsInChildren;
			foreach (ScaleController scaleController in array)
			{
				ScaleMgr.Instance.RemoveScaleController(scaleController);
				UnityEngine.Object.Destroy(scaleController);
			}
		}
	}

	private void SetLayerRecusively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecusively(item.gameObject, layer);
		}
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}
}
