using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoScalingTextBox : MonoBehaviour
{
	[Serializable]
	private struct NineSliceSprite
	{
		public Vector2 m_startSize;

		public XAlignmentTypes m_XAlignment;

		public YAlignmentTypes m_YAlignment;

		public UISprite m_Sprite_TL;

		public UISprite m_Sprite_T;

		public UISprite m_Sprite_TR;

		public UISprite m_Sprite_L;

		public UISprite m_Sprite_C;

		public UISprite m_Sprite_R;

		public UISprite m_Sprite_BL;

		public UISprite m_Sprite_B;

		public UISprite m_Sprite_BR;

		private float m_topHeight;

		private float m_bottomHeight;

		private float m_leftWidth;

		private float m_rightWidth;

		public void Init()
		{
			if ((bool)m_Sprite_TL)
			{
				m_leftWidth = m_Sprite_TL.width;
				m_topHeight = m_Sprite_TL.height;
			}
			if ((bool)m_Sprite_BR)
			{
				m_bottomHeight = m_Sprite_BR.height;
				m_rightWidth = m_Sprite_BR.width;
			}
		}

		public void ApplyNewSize(Rect newRect)
		{
			if (!m_Sprite_TL || !m_Sprite_T || !m_Sprite_TR || !m_Sprite_L || !m_Sprite_C || !m_Sprite_R || !m_Sprite_BL || !m_Sprite_B || !m_Sprite_BR)
			{
				Debug.LogError("AutoScalingTextBox does not work: Either set the single sprite or all of the splicesprites!");
				return;
			}
			float num = newRect.height - m_topHeight - m_bottomHeight;
			float num2 = newRect.width - m_leftWidth - m_rightWidth;
			float yMax = newRect.yMax;
			float y = newRect.yMin + num / 2f + m_bottomHeight;
			float yMin = newRect.yMin;
			float xMin = newRect.xMin;
			float x = newRect.xMin + num2 / 2f + m_leftWidth;
			float xMax = newRect.xMax;
			m_Sprite_TL.cachedTransform.localPosition = new Vector2(xMin, yMax);
			m_Sprite_T.cachedTransform.localPosition = new Vector2(x, yMax);
			m_Sprite_TR.cachedTransform.localPosition = new Vector2(xMax, yMax);
			m_Sprite_L.cachedTransform.localPosition = new Vector2(xMin, y);
			m_Sprite_C.cachedTransform.localPosition = new Vector2(x, y);
			m_Sprite_R.cachedTransform.localPosition = new Vector2(xMax, y);
			m_Sprite_BL.cachedTransform.localPosition = new Vector2(xMin, yMin);
			m_Sprite_B.cachedTransform.localPosition = new Vector2(x, yMin);
			m_Sprite_BR.cachedTransform.localPosition = new Vector2(xMax, yMin);
			m_Sprite_T.width = (int)num2;
			m_Sprite_C.width = (int)num2;
			m_Sprite_B.width = (int)num2;
			m_Sprite_L.height = (int)num;
			m_Sprite_C.height = (int)num;
			m_Sprite_R.height = (int)num;
			m_Sprite_C.enabled = num > 0f && num2 > 0f;
			m_Sprite_L.enabled = num > 0f;
			m_Sprite_R.enabled = num > 0f;
			m_Sprite_T.enabled = num2 > 0f;
			m_Sprite_B.enabled = num2 > 0f;
			Vector3 localScale = new Vector3(1f, 1f, 1f);
			if (num2 <= 0f)
			{
				localScale.x = newRect.width / (m_leftWidth + m_rightWidth);
			}
			if (num <= 0f)
			{
				localScale.y = newRect.height / (m_topHeight + m_bottomHeight);
			}
			m_Sprite_TL.cachedTransform.localScale = localScale;
			m_Sprite_T.cachedTransform.localScale = localScale;
			m_Sprite_TR.cachedTransform.localScale = localScale;
			m_Sprite_L.cachedTransform.localScale = localScale;
			m_Sprite_R.cachedTransform.localScale = localScale;
			m_Sprite_BL.cachedTransform.localScale = localScale;
			m_Sprite_B.cachedTransform.localScale = localScale;
			m_Sprite_BR.cachedTransform.localScale = localScale;
		}
	}

	[Serializable]
	private struct AnchoredObject
	{
		public Transform m_Transform;

		public XAlignmentTypes m_XAlignment;

		public YAlignmentTypes m_YAlignment;

		[NonSerialized]
		public Vector2 m_Offset;
	}

	[SerializeField]
	private XAlignmentTypes m_horizontalAlignment = XAlignmentTypes.Center;

	[SerializeField]
	private YAlignmentTypes m_verticalAlignment;

	[SerializeField]
	[FormerlySerializedAs("Text")]
	private UILabel m_label;

	[SerializeField]
	[FormerlySerializedAs("Sprite")]
	private UISprite m_backgroundSprite;

	[SerializeField]
	private NineSliceSprite m_backgroundSlicedSprite;

	[SerializeField]
	private AnchoredObject[] m_anchoredObjects;

	[Header("Size Properties")]
	[SerializeField]
	[FormerlySerializedAs("maxX")]
	private float m_maxTotalWidth = 600f;

	[SerializeField]
	[FormerlySerializedAs("minX")]
	private float m_minTotalWidth = 200f;

	[SerializeField]
	[FormerlySerializedAs("borderX")]
	private float m_verticalBorders = 20f;

	[SerializeField]
	[FormerlySerializedAs("borderY")]
	private float m_horizontalBorders = 30f;

	[FormerlySerializedAs("offsetX")]
	[SerializeField]
	private float m_horizontalOffset;

	[SerializeField]
	private float m_verticalOffset;

	public Vector3 textPosition
	{
		get
		{
			return m_label.transform.position;
		}
		set
		{
			m_label.transform.position = value;
		}
	}

	private void Awake()
	{
		if ((bool)m_label)
		{
			m_label.width = (int)(m_maxTotalWidth - 2f * m_verticalBorders);
			m_label.text = string.Empty;
		}
		m_backgroundSlicedSprite.Init();
		InitializeAnchoredObjects();
	}

	private void InitializeAnchoredObjects()
	{
		int num;
		int num2;
		Vector2 vector;
		if ((bool)m_backgroundSprite)
		{
			num = m_backgroundSprite.width;
			num2 = m_backgroundSprite.height;
			vector = new Vector2(0.5f, 0.5f) - m_backgroundSprite.pivotOffset;
		}
		else
		{
			num = (int)m_backgroundSlicedSprite.m_startSize.x;
			num2 = (int)m_backgroundSlicedSprite.m_startSize.y;
			vector = GetRelativePivot(m_backgroundSlicedSprite.m_XAlignment, m_backgroundSlicedSprite.m_YAlignment) * -1f;
		}
		vector.x *= num;
		vector.y *= num2;
		for (int i = 0; i < m_anchoredObjects.Length; i++)
		{
			AnchoredObject anchoredObject = m_anchoredObjects[i];
			if (anchoredObject.m_Transform == null)
			{
				anchoredObject.m_Offset = default(Vector2);
				continue;
			}
			Vector2 relativePivot = GetRelativePivot(anchoredObject.m_XAlignment, anchoredObject.m_YAlignment);
			relativePivot.x *= num;
			relativePivot.y *= num2;
			anchoredObject.m_Offset = (Vector2)anchoredObject.m_Transform.localPosition - (vector + relativePivot);
			m_anchoredObjects[i] = anchoredObject;
		}
	}

	private void SetBackgroundSize()
	{
		GetBackgroundSize(true);
	}

	public void SetTextWithAlignment(string newText, XAlignmentTypes horizontalAlignment, YAlignmentTypes verticalAlignment)
	{
		m_horizontalAlignment = horizontalAlignment;
		m_verticalAlignment = verticalAlignment;
		SetText(newText);
	}

	public void SetText(string newText)
	{
		if (!(m_label == null))
		{
			m_label.text = newText;
			SetBackgroundSize();
		}
	}

	public Vector3 SetTextAndGetBackgroundSize(string newText)
	{
		m_label.text = newText;
		return GetBackgroundSize();
	}

	private Vector3 GetBackgroundSize(bool setBackgroundSizeAndTextPos = false)
	{
		float num = m_maxTotalWidth - 2f * m_verticalBorders;
		float num2 = 0f;
		if ((float)m_label.width < num)
		{
			num2 = Mathf.Max(m_minTotalWidth - 2f * m_verticalBorders, m_label.width);
		}
		else
		{
			m_label.width = (int)num;
			num2 = m_label.width;
		}
		m_label.UpdateNGUIText();
		Vector2 printedSize = m_label.printedSize;
		printedSize = new Vector2(Mathf.Max(printedSize.x, m_minTotalWidth - Mathf.Abs(m_horizontalOffset)), printedSize.y);
		Vector2 vector = new Vector2(printedSize.x + 2f * m_verticalBorders + Mathf.Abs(m_horizontalOffset), printedSize.y + 2f * m_horizontalBorders + Mathf.Abs(m_verticalOffset));
		if (setBackgroundSizeAndTextPos)
		{
			SetTextPos(printedSize, vector);
			SetBackgroundPosAndSize(vector);
			SetAnchoredObjects(vector);
		}
		return new Vector3(vector.x, vector.y, 1f);
	}

	private void SetBackgroundPosAndSize(Vector2 size)
	{
		if ((bool)m_backgroundSprite)
		{
			m_backgroundSprite.width = (int)size.x;
			m_backgroundSprite.height = (int)size.y;
			Vector2 vector = m_backgroundSprite.pivotOffset - new Vector2(0.5f, 0.5f) - GetRelativePivot(m_horizontalAlignment, m_verticalAlignment);
			vector.x *= size.x;
			vector.y *= size.y;
			m_backgroundSprite.cachedTransform.localPosition = vector;
		}
		else
		{
			Rect newRect = default(Rect);
			Vector2 vector2 = new Vector2(-0.5f, -0.5f) - GetRelativePivot(m_horizontalAlignment, m_verticalAlignment);
			vector2.x *= size.x;
			vector2.y *= size.y;
			newRect.x = vector2.x;
			newRect.y = vector2.y;
			newRect.width = size.x;
			newRect.height = size.y;
			m_backgroundSlicedSprite.ApplyNewSize(newRect);
		}
	}

	private void SetTextPos(Vector2 labelSize, Vector2 totalSize)
	{
		if ((bool)m_label)
		{
			Vector2 vector = m_label.pivotOffset - new Vector2(0.5f, 0.5f);
			vector.x *= labelSize.x;
			vector.y *= labelSize.y;
			Vector2 relativePivot = GetRelativePivot(m_horizontalAlignment, m_verticalAlignment);
			relativePivot.x *= totalSize.x;
			relativePivot.y *= totalSize.y;
			m_label.cachedTransform.localPosition = vector - relativePivot + new Vector2(m_horizontalOffset * 0.5f, m_verticalOffset * 0.5f);
		}
	}

	private void SetAnchoredObjects(Vector2 targetSize)
	{
		for (int i = 0; i < m_anchoredObjects.Length; i++)
		{
			AnchoredObject anchoredObject = m_anchoredObjects[i];
			Vector2 vector = GetRelativePivot(m_horizontalAlignment, m_verticalAlignment) * -1f + GetRelativePivot(anchoredObject.m_XAlignment, anchoredObject.m_YAlignment);
			vector.x *= targetSize.x;
			vector.y *= targetSize.y;
			anchoredObject.m_Transform.localPosition = vector + anchoredObject.m_Offset;
		}
	}

	private static Vector2 GetRelativePivot(XAlignmentTypes horizontalAlignment, YAlignmentTypes verticalAlignment)
	{
		Vector2 result = default(Vector2);
		switch (horizontalAlignment)
		{
		case XAlignmentTypes.Left:
			result.x = -0.5f;
			break;
		case XAlignmentTypes.Center:
			result.x = 0f;
			break;
		case XAlignmentTypes.Right:
			result.x = 0.5f;
			break;
		}
		switch (verticalAlignment)
		{
		case YAlignmentTypes.Top:
			result.y = 0.5f;
			break;
		case YAlignmentTypes.Center:
			result.y = 0f;
			break;
		case YAlignmentTypes.Bottom:
			result.y = -0.5f;
			break;
		}
		return result;
	}
}
