using ABH.Shared.Models.Character;
using UnityEngine;

public class TrophyItemInfo : MonoBehaviour
{
	[SerializeField]
	private UILabel m_SeasonChampLabel;

	[SerializeField]
	private UILabel m_Description;

	public void SetModel(TrophyData Trophy)
	{
		ABHLocaService locaService = DIContainerInfrastructure.GetLocaService();
		m_SeasonChampLabel.text = locaService.Tr("pvp_trophy_s" + Trophy.Seasonid.ToString("00") + "_l" + Trophy.FinishedLeagueId.ToString("00") + "_name");
		m_Description.text = locaService.Tr("pvp_trophy_s" + Trophy.Seasonid.ToString("00") + "_l" + Trophy.FinishedLeagueId.ToString("00") + "_desc");
	}
}
