using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class DummyBattleScreen : MonoBehaviour
{
	public GameObject m_CharacterButtonPrefab;

	private List<BattlePrepCharacterButton> m_buttonList = new List<BattlePrepCharacterButton>();

	public bool IsActive { get; set; }

	public void Start()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Birds != null)
		{
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().Birds.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(m_CharacterButtonPrefab);
				gameObject.transform.parent = base.transform;
				BattlePrepCharacterButton component = gameObject.GetComponent<BattlePrepCharacterButton>();
				m_buttonList.Add(component);
			}
		}
		SetBirdsPosition();
	}

	private void SetBirdsPosition()
	{
		float num = 200f;
		int count = m_buttonList.Count;
		float num2 = (float)(count - 1) * num / 2f;
		for (int i = 0; i < count; i++)
		{
			m_buttonList[i].transform.localPosition = new Vector3(num * (float)i - num2, 0f, -10f);
		}
	}

	public void Back()
	{
		IsActive = false;
		base.gameObject.SetActive(false);
	}

	public void Battle()
	{
		List<BirdGameData> list = new List<BirdGameData>();
		foreach (BattlePrepCharacterButton button in m_buttonList)
		{
			if (button.IsSelected())
			{
				list.Add(button.GetBirdGameData());
			}
		}
		Back();
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		IsActive = true;
	}
}
