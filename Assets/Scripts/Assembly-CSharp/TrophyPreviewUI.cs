using System.Collections;
using ABH.Shared.Models.Character;
using UnityEngine;

public class TrophyPreviewUI : MonoBehaviour
{
	[SerializeField]
	private CHMeshSprite m_TrophySprite;

	public void SetModel(TrophyData trophy)
	{
		m_TrophySprite.m_SpriteName = trophy.NameId;
		m_TrophySprite.UpdateSprite(false, true);
	}

	public IEnumerator Enter()
	{
		GetComponent<Animation>().Play("CharacterDisplay_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["CharacterDisplay_Enter"].clip.length);
	}

	public IEnumerator Leave()
	{
		GetComponent<Animation>().Play("CharacterDisplay_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["CharacterDisplay_Leave"].clip.length);
	}
}
