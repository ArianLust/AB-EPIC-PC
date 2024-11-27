using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	public UILabel m_FPSText;

	public UILabel m_LowestFPSInLast10Sec;

	public UILabel m_LowestFPSEver;

	private float _LowestFPSInLast10Sec = 9999f;

	private float _LowestFPSEver = 9999f;

	public float m_UpdateInterval = 0.5f;

	private float m_TenSecInterval = 10f;

	private float m_AccumulatedTimePassed;

	private float m_AccumulatedTimePassedForTenSec;

	private int m_FramesPassed;

	private int m_FramesPassedForTenSec;

	private float m_UpdateTimer;

	private float m_TenSecUpdateTimer;

	private void Start()
	{
		#if UNITY_ANDROID || UNITY_IOS
			Application.targetFrameRate = 60;
		#endif
		base.gameObject.SetActive(false);
	}

	public void Awake()
	{
		base.useGUILayout = false;
		base.enabled = (bool)DIContainerInfrastructure.GetCoreStateMgr() && DIContainerInfrastructure.GetCoreStateMgr().UseDebug;
	}

	private void OnDisable()
	{
		m_FPSText.gameObject.SetActive(false);
		m_LowestFPSInLast10Sec.gameObject.SetActive(false);
		m_LowestFPSEver.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		m_FPSText.gameObject.SetActive(true);
		m_LowestFPSInLast10Sec.gameObject.SetActive(true);
		m_LowestFPSEver.gameObject.SetActive(true);
	}

	private void Update()
	{
		m_UpdateTimer -= Time.deltaTime;
		m_TenSecUpdateTimer -= Time.deltaTime;
		m_AccumulatedTimePassed += Time.timeScale / Time.deltaTime;
		m_AccumulatedTimePassedForTenSec += Time.timeScale / Time.deltaTime;
		m_FramesPassed++;
		m_FramesPassedForTenSec++;
		if ((double)m_UpdateTimer <= 0.0)
		{
			UpdateFPSDisplay();
		}
		if ((double)m_TenSecUpdateTimer <= 0.0)
		{
			float fps = m_AccumulatedTimePassedForTenSec / (float)m_FramesPassedForTenSec;
			UpdateLowestFPSTenSec(fps);
		}
	}

	private void ResetTenSecCounters()
	{
		m_TenSecUpdateTimer = m_TenSecInterval;
		m_AccumulatedTimePassedForTenSec = 0f;
		m_FramesPassedForTenSec = 0;
		_LowestFPSInLast10Sec = 9999f;
	}

	private void ResetNormalCounters()
	{
		m_UpdateTimer = m_UpdateInterval;
		m_AccumulatedTimePassed = 0f;
		m_FramesPassed = 0;
	}

	private void UpdateFPSDisplay()
	{
		float num = m_AccumulatedTimePassed / (float)m_FramesPassed;
		m_FPSText.text = "FPS: " + string.Format("{0:F2} FPS", num);
		if (num < 30f)
		{
			m_FPSText.color = Color.blue;
		}
		else if (num < 10f)
		{
			m_FPSText.color = Color.red;
		}
		else
		{
			m_FPSText.color = Color.green;
		}
		if (num < _LowestFPSEver)
		{
			UpdateLowestFPSEverTime(num);
		}
		if (num < _LowestFPSInLast10Sec)
		{
			_LowestFPSInLast10Sec = num;
		}
		ResetNormalCounters();
	}

	private void UpdateLowestFPSEverTime(float fps)
	{
		_LowestFPSEver = fps;
		m_LowestFPSEver.text = "Low: " + string.Format("{0:F2} FPS", _LowestFPSEver);
		if (_LowestFPSEver < 30f)
		{
			m_LowestFPSEver.color = Color.blue;
		}
		else if (_LowestFPSEver < 10f)
		{
			m_LowestFPSEver.color = Color.red;
		}
		else
		{
			m_LowestFPSEver.color = Color.green;
		}
	}

	private void UpdateLowestFPSTenSec(float fps)
	{
		m_LowestFPSInLast10Sec.text = "10s: " + string.Format("{0:F2} FPS", _LowestFPSInLast10Sec);
		if (_LowestFPSInLast10Sec < 30f)
		{
			m_LowestFPSInLast10Sec.color = Color.blue;
		}
		else if (_LowestFPSInLast10Sec < 10f)
		{
			m_LowestFPSInLast10Sec.color = Color.red;
		}
		else
		{
			m_LowestFPSInLast10Sec.color = Color.green;
		}
		ResetTenSecCounters();
	}
}
