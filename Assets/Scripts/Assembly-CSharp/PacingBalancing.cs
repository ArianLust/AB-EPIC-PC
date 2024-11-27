using UnityEngine;

public class PacingBalancing : MonoBehaviour
{
	[SerializeField]
	private float m_TimeFromStartPosToBasePosInSec;

	[SerializeField]
	private float m_TimeFromBasePosToFocusPosInSec = 0.125f;

	[SerializeField]
	private float m_TimeToJumpInFrontOfAlly = 0.05f;

	[SerializeField]
	private float m_TimeAfterSheltering = 0.7f;

	[SerializeField]
	private float m_TimeFromFocusPosToAttackPosInSec = 0.2f;

	[SerializeField]
	private float m_TimeFromFocusPosToBasePosInSec = 0.125f;

	[SerializeField]
	private float m_TimeFromAttackPosToBasePosInSec = 0.25f;

	[SerializeField]
	private float m_TimeForPigToChooseTargetAndDoSkillInSec = 0.6f;

	[SerializeField]
	private float m_TimeForShowDamageAndReturnToBasePosInSec = 1f;

	[SerializeField]
	private float m_WaitFactorForReturnToBasePos = 0.5f;

	[SerializeField]
	private float m_TimeForShowHealthBarChangeInSec = 0.125f;

	[SerializeField]
	private float m_TimeForFillHealthBarChangedInSec = 0.4f;

	[SerializeField]
	private float m_AfterEnterBattleGroundDelay = 2f;

	[SerializeField]
	private float m_BaseBattleEnterDelay = 0.25f;

	[SerializeField]
	private float m_PerCombatantEnterDelay = 0.1f;

	[SerializeField]
	private float m_TimeForSetCurrentHealthBarInSec = 0.25f;

	[SerializeField]
	private float m_BaseBattleEndDelay = 4f;

	[SerializeField]
	private float m_FallBackTimeWaitForSkillImpact = 1f;

	[SerializeField]
	private float m_FallBackTimeWaitForBlockedMainLoop = 6f;

	[SerializeField]
	private float m_TimeForFocusInituativeAndDOTS = 0.25f;

	[SerializeField]
	private float m_XPFlyTime = 0.5f;

	[SerializeField]
	private float m_TimeForShowRewardWheel = 1.5f;

	[SerializeField]
	private float m_DelayForAllMultiAttacks = 0.5f;

	[SerializeField]
	private float m_XpAndGoldBarStayDuration = 2f;

	[SerializeField]
	private float m_DelayTillRageBarReact = 0.3f;

	[SerializeField]
	private float m_ToasterTime = 3f;

	[SerializeField]
	private float m_ScenerySwitchTimeWalk = 2.5f;

	[SerializeField]
	private float m_ScenerySwitchTimeRun = 1.75f;

	[SerializeField]
	private float m_LevelUpDelayOnCharacters = 1f;

	[SerializeField]
	private float m_TimeForChestExplode = 1f;

	[SerializeField]
	private float m_CraftingTimeForTillStarAppearance = 1.15f;

	[SerializeField]
	private float m_EquipmentRepositionDuration = 0.5f;

	[SerializeField]
	private float m_PreviewWaitTimeBetweenSkills = 1f;

	[SerializeField]
	private float m_ScoreCountTime = 1f;

	[SerializeField]
	private float m_RevivePopupShowTime = 5f;

	[SerializeField]
	private float m_ScoringAnimationMaxTime = 2f;

	[SerializeField]
	private float m_MissingCurrencyOverlayTime = 3f;

	[SerializeField]
	private float m_AutoPlayWaitAfterSkillTime = 0.5f;

	[SerializeField]
	private float m_TimeForShowNews = 900f;

	[SerializeField]
	private float m_DelayBeforePvPCoinflip;

	public float TimeFromStartPosToBasePosInSec
	{
		get
		{
			return m_TimeFromStartPosToBasePosInSec;
		}
	}

	public float TimeFromBasePosToFocusPosInSec
	{
		get
		{
			return m_TimeFromBasePosToFocusPosInSec;
		}
	}

	public float TimeToJumpInFrontOfAlly
	{
		get
		{
			return m_TimeToJumpInFrontOfAlly;
		}
	}

	public float TimeAfterSheltering
	{
		get
		{
			return m_TimeAfterSheltering;
		}
	}

	public float TimeFromFocusPosToAttackPosInSec
	{
		get
		{
			return m_TimeFromFocusPosToAttackPosInSec;
		}
	}

	public float TimeFromFocusPosToBasePosInSec
	{
		get
		{
			return m_TimeFromFocusPosToBasePosInSec;
		}
	}

	public float TimeFromAttackPosToBasePosInSec
	{
		get
		{
			return m_TimeFromAttackPosToBasePosInSec;
		}
	}

	public float TimeForPigToChooseTargetAndDoSkillInSec
	{
		get
		{
			return m_TimeForPigToChooseTargetAndDoSkillInSec;
		}
	}

	public float TimeForShowDamageAndReturnToBasePosInSec
	{
		get
		{
			return m_TimeForShowDamageAndReturnToBasePosInSec;
		}
	}

	public float WaitFactorForReturnToBasePos
	{
		get
		{
			return m_WaitFactorForReturnToBasePos;
		}
	}

	public float TimeForShowHealthBarChangeInSec
	{
		get
		{
			return m_TimeForShowHealthBarChangeInSec;
		}
	}

	public float TimeForFillHealthBarChangedInSec
	{
		get
		{
			return m_TimeForFillHealthBarChangedInSec;
		}
	}

	public float AfterEnterBattleGroundDelay
	{
		get
		{
			return m_AfterEnterBattleGroundDelay;
		}
	}

	public float BaseBattleEnterDelay
	{
		get
		{
			return m_BaseBattleEnterDelay;
		}
	}

	public float PerCombatantEnterDelay
	{
		get
		{
			return m_PerCombatantEnterDelay;
		}
	}

	public float TimeForSetCurrentHealthBarInSec
	{
		get
		{
			return m_TimeForSetCurrentHealthBarInSec;
		}
	}

	public float TimeForHealthBarUpdate
	{
		get
		{
			return TimeForShowHealthBarChangeInSec + TimeForFillHealthBarChangedInSec + TimeForSetCurrentHealthBarInSec;
		}
	}

	public float BaseBattleEndDelay
	{
		get
		{
			return m_BaseBattleEndDelay;
		}
	}

	public float FallBackTimeWaitForSkillImpact
	{
		get
		{
			return m_FallBackTimeWaitForSkillImpact;
		}
	}

	public float FallBackTimeWaitForBlockedMainLoop
	{
		get
		{
			return m_FallBackTimeWaitForBlockedMainLoop;
		}
	}

	public float TimeForFocusInituativeAndDOTS
	{
		get
		{
			return m_TimeForFocusInituativeAndDOTS;
		}
	}

	public float XPFlyTime
	{
		get
		{
			return m_XPFlyTime;
		}
	}

	public float TimeForShowRewardWheel
	{
		get
		{
			return m_TimeForShowRewardWheel;
		}
	}

	public float DelayForAllMultiAttacks
	{
		get
		{
			return m_DelayForAllMultiAttacks;
		}
	}

	public float XpAndGoldBarStayDuration
	{
		get
		{
			return m_XpAndGoldBarStayDuration;
		}
	}

	public float DelayTillRageBarReact
	{
		get
		{
			return m_DelayTillRageBarReact;
		}
	}

	public float ToasterTime
	{
		get
		{
			return m_ToasterTime;
		}
	}

	public float ScenerySwitchTimeWalk
	{
		get
		{
			return m_ScenerySwitchTimeWalk;
		}
	}

	public float ScenerySwitchTimeRun
	{
		get
		{
			return m_ScenerySwitchTimeRun;
		}
	}

	public float LevelUpDelayOnCharacters
	{
		get
		{
			return m_LevelUpDelayOnCharacters;
		}
	}

	public float TimeForChestExplode
	{
		get
		{
			return m_TimeForChestExplode;
		}
	}

	public float CraftingTimeForTillStarAppearance
	{
		get
		{
			return m_CraftingTimeForTillStarAppearance;
		}
	}

	public float EquipmentRepositionDuration
	{
		get
		{
			return m_EquipmentRepositionDuration;
		}
	}

	public float PreviewWaitTimeBetweenSkills
	{
		get
		{
			return m_PreviewWaitTimeBetweenSkills;
		}
	}

	public float ScoreCountTime
	{
		get
		{
			return m_ScoreCountTime;
		}
	}

	public float RevivePopupShowTime
	{
		get
		{
			return m_RevivePopupShowTime;
		}
	}

	public float ScoringAnimationMaxTime
	{
		get
		{
			return m_ScoringAnimationMaxTime;
		}
	}

	public float MissingCurrencyOverlayTime
	{
		get
		{
			return m_MissingCurrencyOverlayTime;
		}
	}

	public float AutoPlayWaitAfterSkillTime
	{
		get
		{
			return m_AutoPlayWaitAfterSkillTime;
		}
	}

	public float TimeForShowNews
	{
		get
		{
			return m_TimeForShowNews;
		}
	}

	public float DelayBeforePvPCoinflip
	{
		get
		{
			return m_DelayBeforePvPCoinflip;
		}
	}
}
