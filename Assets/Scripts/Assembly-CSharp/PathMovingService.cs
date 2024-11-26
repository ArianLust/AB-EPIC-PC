using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovingService : MonoBehaviour
{
	public Camera m_Cam;

	private static PathMovingService _instance;

	private Bounds m_CamRect;

	public static PathMovingService Instance
	{
		get
		{
			return _instance;
		}
	}

	public void Awake()
	{
		_instance = this;
	}

	public void WalkAlongPath(List<HotSpotWorldMapViewBase> pathList, GameObject bird, Animation anim, float speed, int index, float delay, MonoBehaviour callbackScript, string callbackFunction, bool mirror = false, string moveAnimation = "Move_Loop", bool ignoreOutOfScreen = false)
	{
		StartCoroutine(WalkToTarget(pathList, bird, anim, speed, index, delay, callbackScript, callbackFunction, mirror, moveAnimation, ignoreOutOfScreen));
	}

	private IEnumerator WalkToTarget(List<HotSpotWorldMapViewBase> pathList, GameObject movingObject, Animation anim, float speed, int index, float delay, MonoBehaviour callbackScript, string callbackFunction, bool mirror = false, string moveAnimation = "Move_Loop", bool ignoreOutOfScreen = false)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		else
		{
			yield return null;
		}
		int listIndex = GetStartIndex(pathList, 0, ignoreOutOfScreen);
		float splinePosition = 0f;
		int lastIndex = 0;
		RageSplineLite currentPath = GetNewPath(ref listIndex, pathList);
		bool reversePath = CalculateSplinePositionIndex(ref splinePosition, listIndex, pathList);
		int currentIndex2 = listIndex;
		if (anim != null)
		{
			anim.CrossFade(moveAnimation);
		}
		Vector3 startingOffset = movingObject.transform.position - pathList[0].transform.position;
		yield return StartCoroutine(WalkToPosition(movingObject, pathList[0].transform.position, speed));
		if (listIndex < pathList.Count)
		{
			HotSpotWorldMapViewBase previousHotspot2 = pathList[0];
			HotSpotWorldMapViewBase hotspot2 = pathList[currentIndex2];
			previousHotspot2.HandleMovingObjectVisibility(movingObject, hotspot2);
		}
		do
		{
			if (currentPath == null)
			{
				currentPath = GetNewPath(ref listIndex, pathList);
				reversePath = CalculateSplinePositionIndex(ref splinePosition, listIndex, pathList);
			}
			else
			{
				Vector3 lastPosition = movingObject.transform.position;
				movingObject.transform.position = currentPath.GetPositionWorldSpace(splinePosition) + new Vector3(0f, 0f, -20f);
				if (!mirror)
				{
					if (lastPosition.x > movingObject.transform.position.x)
					{
						movingObject.transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (lastPosition.x < movingObject.transform.position.x)
					{
						movingObject.transform.localScale = new Vector3(1f, 1f, 1f);
					}
				}
				else if (lastPosition.x > movingObject.transform.position.x)
				{
					movingObject.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				else if (lastPosition.x < movingObject.transform.position.x)
				{
					movingObject.transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				splinePosition = ((!reversePath) ? (splinePosition + speed / currentPath.GetLength() * Time.deltaTime) : (splinePosition - speed / currentPath.GetLength() * Time.deltaTime));
				if (splinePosition > 1f || splinePosition < 0f)
				{
					lastIndex = listIndex + (reversePath ? 1 : 0);
					currentPath = GetNewPath(ref listIndex, pathList);
					currentIndex2 = listIndex + (reversePath ? 1 : 0);
					if (currentIndex2 < pathList.Count)
					{
						if (!IsInFrustrum(pathList[currentIndex2 - 1]) && !ignoreOutOfScreen)
						{
							listIndex = GetStartIndex(pathList, listIndex, ignoreOutOfScreen);
							lastIndex = listIndex + (reversePath ? 1 : 0);
							currentPath = GetNewPath(ref listIndex, pathList);
							currentIndex2 = listIndex + (reversePath ? 1 : 0);
						}
						if (currentIndex2 < pathList.Count)
						{
							HotSpotWorldMapViewBase previousHotspot = pathList[lastIndex];
							HotSpotWorldMapViewBase hotspot = pathList[currentIndex2];
							hotspot.HandleMovingObjectVisibility(movingObject, hotspot);
						}
					}
					reversePath = CalculateSplinePositionIndex(ref splinePosition, listIndex, pathList);
				}
			}
			yield return null;
		}
		while (listIndex < pathList.Count);
		yield return null;
		HotSpotWorldMapViewBase lastHotspot = pathList[pathList.Count - 1];
		lastHotspot.HandleMovingObjectVisibility(movingObject, lastHotspot);
		Vector3 offset = ((lastHotspot.m_HotSpotPositions.Length <= index) ? Vector3.zero : lastHotspot.m_HotSpotPositions[index]);
		yield return StartCoroutine(WalkToPosition(movingObject, pathList[pathList.Count - 1].transform.position + offset, speed));
		if (anim != null)
		{
			anim.CrossFade("Idle");
		}
		if (callbackScript != null && callbackFunction != string.Empty)
		{
			DebugLog.Log("[PathMovingService] Invoking callback with param " + index);
			if (callbackScript is BaseLocationStateManager)
			{
				(callbackScript as BaseLocationStateManager).m_movementTargetIndex = index;
			}
			callbackScript.SendMessage(callbackFunction, index);
		}
	}

	private IEnumerator WalkToPosition(GameObject bird, Vector3 target, float speed)
	{
		Vector3 delta = bird.transform.position - target;
		if (delta.magnitude > 1f)
		{
			bool targetReached = false;
			do
			{
				yield return null;
				Vector3 stepSize = delta.normalized * (speed / 5f * Time.deltaTime);
				if (stepSize.sqrMagnitude > delta.sqrMagnitude)
				{
					targetReached = true;
					bird.transform.position = target;
				}
				else
				{
					delta -= stepSize;
					bird.transform.Translate(-stepSize);
				}
			}
			while (!targetReached);
		}
		else
		{
			bird.transform.position = target;
		}
	}

	private int GetStartIndex(List<HotSpotWorldMapViewBase> pathList, int startAt, bool ignoreOutOfScreen)
	{
		int num = startAt - 1;
		if (startAt < 0 || startAt >= pathList.Count)
		{
			DebugLog.Error("Wrong StartIndex for path search: " + startAt);
			return pathList.Count - 1;
		}
		Vector2 zero = Vector2.zero;
		m_CamRect = default(Bounds);
		if ((bool)m_Cam)
		{
			zero = new Vector2(m_Cam.orthographicSize * ((float)Screen.width / (float)Screen.height), m_Cam.orthographicSize);
			m_CamRect = new Bounds(m_Cam.transform.position, new Vector3(2f * zero.x, 2f * zero.y));
		}
		if (pathList.Count > 2)
		{
			for (int i = startAt; i < pathList.Count; i++)
			{
				HotSpotWorldMapViewBase hotSpotWorldMapViewBase = pathList[i];
				if (RectangleIntersect(m_CamRect, hotSpotWorldMapViewBase.GetBoundingBox()) || ignoreOutOfScreen)
				{
					break;
				}
				num++;
			}
		}
		if (num > 0 && pathList[num] == pathList[num - 1].GetPreviousHotspot())
		{
			num--;
		}
		return num;
	}

	private RageSplineLite GetNewPath(ref int listIndex, List<HotSpotWorldMapViewBase> pathList)
	{
		listIndex++;
		if (listIndex >= pathList.Count)
		{
			return null;
		}
		if (listIndex == 0)
		{
			if (pathList[listIndex].GetPreviousHotspot() != pathList[listIndex + 1])
			{
				return GetNewPath(ref listIndex, pathList);
			}
		}
		else if (listIndex != pathList.Count - 1)
		{
		}
		if (listIndex < pathList.Count - 1 && pathList[listIndex].GetPreviousHotspot() == pathList[listIndex + 1])
		{
			return pathList[listIndex].GetPath();
		}
		if (listIndex > 0 && pathList[listIndex].GetPreviousHotspot() == pathList[listIndex - 1])
		{
			return pathList[listIndex].GetPath();
		}
		return GetNewPath(ref listIndex, pathList);
	}

	private bool CalculateSplinePositionIndex(ref float splinePosition, int listIndex, List<HotSpotWorldMapViewBase> pathList)
	{
		if (listIndex >= pathList.Count)
		{
			return false;
		}
		if (pathList[listIndex].GetPath() == null)
		{
			return false;
		}
		float num = 0f;
		if (listIndex < pathList.Count - 1)
		{
			if (pathList[listIndex].GetPreviousHotspot() == pathList[listIndex + 1])
			{
				num = 1f;
				splinePosition = num - Mathf.Abs(splinePosition) % 1f;
			}
			else
			{
				splinePosition = Mathf.Abs(splinePosition) % 1f;
			}
		}
		else if (listIndex > 0)
		{
			if (pathList[listIndex].GetPreviousHotspot() == pathList[listIndex - 1])
			{
				splinePosition = Mathf.Abs(splinePosition) % 1f;
			}
			else
			{
				num = 1f;
				splinePosition = num - Mathf.Abs(splinePosition) % 1f;
			}
		}
		return num == 1f;
	}

	private bool RectangleIntersect(Bounds rectA, Bounds rectB)
	{
		Bounds bounds = new Bounds(new Vector3(rectA.center.x, rectA.center.y, 100f), new Vector3(rectA.size.x, rectA.size.y, 100f));
		Bounds bounds2 = new Bounds(new Vector3(rectB.center.x, rectB.center.y, 100f), new Vector3(rectB.size.x, rectB.size.y, 100f));
		return bounds.Intersects(bounds2);
	}

	private bool IsInFrustrum(HotSpotWorldMapViewBase spot)
	{
		Vector2 zero = Vector2.zero;
		m_CamRect = default(Bounds);
		if ((bool)m_Cam)
		{
			zero = new Vector2(m_Cam.orthographicSize * ((float)Screen.width / (float)Screen.height), m_Cam.orthographicSize);
			m_CamRect = new Bounds(m_Cam.transform.position, new Vector3(2f * zero.x, 2f * zero.y));
		}
		if (RectangleIntersect(m_CamRect, spot.GetBoundingBox()))
		{
			return true;
		}
		return false;
	}
}
