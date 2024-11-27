using System.Collections;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using UnityEngine;

public class BattleProgressBar : MonoBehaviour
{
	[SerializeField]
	private UIGrid m_CenterTracks;

	[SerializeField]
	private GameObject m_CenterTrackPrefab;

	[SerializeField]
	private GameObject m_CenterTrackPrefabWheelBronze;

	[SerializeField]
	private GameObject m_CenterTrackPrefabWheelSilver;

	[SerializeField]
	private GameObject m_CenterTrackPrefabWheelGold;

	[SerializeField]
	private GameObject m_EndTrack;

	[SerializeField]
	private GameObject m_StartTrack;

	[SerializeField]
	private CHMotionTween m_Bird;

	[SerializeField]
	private Animation m_BirdAnimation;

	[SerializeField]
	private GameObject m_ChestRoot;

	[SerializeField]
	private GameObject m_WheelRoot;

	[SerializeField]
	private GameObject m_WheelRootBronze;

	[SerializeField]
	private GameObject m_WheelRootSilver;

	[SerializeField]
	private GameObject m_WheelRootGold;

	[SerializeField]
	private Vector2 m_ProgressRange = new Vector2(-100f, 100f);

	private BattleMgrBase m_BattleMgr;

	private float m_Progress;

	private float m_ProgressX;

	private float m_OffsetZ;

	private List<GameObject> m_centerTrackList = new List<GameObject>();

	private Dictionary<int, GameObject> m_centerWheels = new Dictionary<int, GameObject>();

	public bool m_Updating;

	public bool m_Entered { get; set; }

	private void Awake()
	{
		m_ProgressX = m_ProgressRange.x;
		base.gameObject.SetActive(false);
	}

	public void SetBattleMgr(BattleMgrBase battleMgr)
	{
		m_BattleMgr = battleMgr;
		if (battleMgr.Model != null)
		{
			Initialize();
		}
	}

	public GameObject GetWheelPrefab(int index)
	{
		switch (index)
		{
		case 0:
			return m_CenterTrackPrefabWheelBronze;
		case 1:
			return m_CenterTrackPrefabWheelSilver;
		default:
			return m_CenterTrackPrefabWheelGold;
		}
	}

	private void Initialize()
	{
		int count = m_BattleMgr.Model.Balancing.BattleParticipantsIds.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(m_CenterTrackPrefab, Vector3.zero, Quaternion.identity);
			m_centerTrackList.Add(gameObject);
			gameObject.transform.parent = m_CenterTracks.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			m_CenterTracks.Reposition();
			Vector3 localPosition = m_StartTrack.transform.localPosition;
			localPosition.x -= 32f;
			m_StartTrack.transform.localPosition = localPosition;
			Vector3 localPosition2 = m_EndTrack.transform.localPosition;
			localPosition2.x += 32f;
			m_EndTrack.transform.localPosition = localPosition2;
			if (m_BattleMgr.Model.Balancing.LootTableWheelAfterWave == null)
			{
				DebugLog.Log("[BattleProgressBar] No additional loot wheels availiable");
			}
			else if (m_BattleMgr.Model.Balancing.LootTableWheelAfterWave.ContainsKey(i + 1))
			{
				GameObject gameObject2 = (GameObject)Object.Instantiate(GetWheelPrefab(num), Vector3.zero, Quaternion.identity);
				m_centerWheels.Add(i + 1, gameObject2);
				gameObject2.transform.parent = m_CenterTracks.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localScale = Vector3.one;
				m_CenterTracks.Reposition();
				localPosition = m_StartTrack.transform.localPosition;
				localPosition.x -= 32f;
				m_StartTrack.transform.localPosition = localPosition;
				localPosition2 = m_EndTrack.transform.localPosition;
				localPosition2.x += 32f;
				m_EndTrack.transform.localPosition = localPosition2;
				num++;
			}
		}
		Vector3 localPosition3 = m_CenterTracks.gameObject.transform.localPosition;
		localPosition3.x = m_StartTrack.transform.localPosition.x + 32f;
		m_CenterTracks.gameObject.transform.localPosition = localPosition3;
		LootTableBalancingData balancing = null;
		if (m_BattleMgr.Model.Balancing.LootTableWheel == null)
		{
			return;
		}
		int num2 = 0;
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		ExperienceLevelBalancingData balancing2;
		if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + level.ToString("00"), out balancing2) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (level - 1).ToString("00"), out balancing2))
		{
			num2 = balancing2.MatchmakingRangeIndex;
		}
		string nameId = m_BattleMgr.Model.Balancing.LootTableWheel.FirstOrDefault().Key.Replace("{levelrange}", num2.ToString("00"));
		DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(nameId, out balancing);
		LootTableBalancingData balancing3 = null;
		if (num > 0)
		{
			m_ChestRoot.SetActive(false);
			m_WheelRoot.SetActive(false);
			if (num == 1)
			{
				m_WheelRootSilver.gameObject.SetActive(true);
			}
			else
			{
				m_WheelRootGold.gameObject.SetActive(true);
			}
		}
		else if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(balancing.LootTableEntries[0].NameId, out balancing3))
		{
			m_ChestRoot.SetActive(true);
			m_WheelRoot.SetActive(false);
		}
		else
		{
			m_ChestRoot.SetActive(false);
			m_WheelRoot.SetActive(true);
		}
	}

	public IEnumerator UpdateProgress()
	{
		m_Updating = true;
		GetComponent<Animation>().Play("BattleProgress_Update");
		yield return new WaitForSeconds(0.2f);
		m_centerTrackList[m_BattleMgr.Model.CurrentWaveIndex - 1].GetComponent<Animation>().Play("BattleProgress_Pig_Defeated");
		yield return new WaitForSeconds(0.2f);
		m_BirdAnimation.Play("BattleProgress_Bird_Move");
		if (m_centerWheels != null && m_centerWheels.ContainsKey(m_BattleMgr.Model.CurrentWaveIndex))
		{
			m_centerWheels[m_BattleMgr.Model.CurrentWaveIndex].PlayAnimationOrAnimatorState("BattleProgress_WheelOfLoot_Accomplished");
		}
		else if (m_centerWheels != null && m_centerWheels.ContainsKey(m_BattleMgr.Model.CurrentWaveIndex - 1))
		{
			m_Bird.Play();
			yield return new WaitForSeconds(m_Bird.m_DurationInSeconds);
		}
		m_Bird.Play();
		yield return new WaitForSeconds(m_Bird.m_DurationInSeconds);
		m_BirdAnimation.Play("BattleProgress_Bird_Idle");
		yield return new WaitForEndOfFrame();
		foreach (Transform root in m_centerTrackList[m_BattleMgr.Model.CurrentWaveIndex - 1].transform)
		{
			if (root.name != "Sprite")
			{
				Object.Destroy(root.gameObject, 1f);
			}
		}
		m_Updating = false;
	}

	public void Enter()
	{
		if (!m_Updating && !m_Entered)
		{
			m_Entered = true;
			base.gameObject.SetActive(true);
			StartCoroutine(EnterCoroutine());
		}
	}

	public IEnumerator EnterCoroutine()
	{
		GetComponent<Animation>().Play("BattleProgress_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["BattleProgress_Enter"].length);
	}

	public void Leave()
	{
		if (!m_Updating && m_Entered)
		{
			base.gameObject.SetActive(true);
			StartCoroutine(LeaveCoroutine());
		}
	}

	public IEnumerator LeaveCoroutine()
	{
		GetComponent<Animation>().Play("BattleProgress_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["BattleProgress_Leave"].length);
		base.gameObject.SetActive(false);
		m_Entered = false;
	}
}
