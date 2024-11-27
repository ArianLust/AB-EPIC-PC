using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class RageSpline : MonoBehaviour, IRageSpline
{
	public enum Outline
	{
		None = 0,
		Loop = 1,
		Free = 2
	}

	public enum OutlineGradient
	{
		None = 0,
		Default = 1,
		Inverse = 2
	}

	public enum Corner
	{
		Default = 0,
		Beak = 1
	}

	public enum Fill
	{
		None = 0,
		Solid = 1,
		Gradient = 2,
		Landscape = 3
	}

	public enum UVMapping
	{
		None = 0,
		Fill = 1,
		Outline = 2
	}

	public enum Emboss
	{
		None = 0,
		Sharp = 1,
		Blurry = 2
	}

	public enum Physics
	{
		None = 0,
		Boxed = 1,
		MeshCollider = 2,
		OutlineMeshCollider = 3,
		Polygon = 4
	}

	public enum ShowCoordinates
	{
		None = 0,
		Local = 1,
		World = 2
	}

	public struct RageVertex
	{
		public Vector3 position;

		public Vector2 uv1;

		public Vector2 uv2;

		public Color color;

		public Vector3 normal;

		public float splinePosition;

		public float splineSegmentPosition;

		public RageSplinePoint curveStart;

		public RageSplinePoint curveEnd;
	}

	public Outline outline = Outline.Loop;

	public Color outlineColor1 = Color.black;

	public Color outlineColor2 = Color.black;

	public float OutlineWidth = 1f;

	public float outlineTexturingScale = 10f;

	public OutlineGradient outlineGradient;

	public float outlineNormalOffset;

	public Corner corners;

	public Fill fill = Fill.Solid;

	public float landscapeBottomDepth = 10f;

	public Color fillColor1 = new Color(0.6f, 0.6f, 0.6f, 1f);

	public Color fillColor2 = new Color(0.4f, 0.4f, 0.4f, 1f);

	public UVMapping UVMapping1;

	public UVMapping UVMapping2;

	public Vector2 gradientOffset = new Vector2(-5f, 5f);

	public float gradientAngle;

	public float gradientScale = 0.1f;

	public Vector2 textureOffset = new Vector2(-5f, -5f);

	public float textureAngle;

	public float textureScale = 0.1f;

	public Vector2 textureOffset2 = new Vector2(5f, 5f);

	public float textureAngle2;

	public float textureScale2 = 0.1f;

	public Emboss emboss;

	public Color embossColor1 = new Color(0.75f, 0.75f, 0.75f, 1f);

	public Color embossColor2 = new Color(0.25f, 0.25f, 0.25f, 1f);

	public float embossAngle = 180f;

	public float embossOffset = 0.5f;

	public float embossSize = 10f;

	public float embossCurveSmoothness = 10f;

	public Physics physics;

	public bool createPhysicsInEditor;

	public bool createConvexMeshCollider;

	public PhysicMaterial physicsMaterial;

	public PhysicsMaterial2D physicsMaterial2D;

	[SerializeField]
	private int _vertexDensity;

	public int vertexCount = 64;

	public int optimizedVertexCount = 64;

	public int physicsColliderCount = 32;

	public bool LockPhysicsToAppearence;

	public float colliderZDepth = 100f;

	public float colliderNormalOffset;

	public float boxColliderDepth = 1f;

	[SerializeField]
	private BoxCollider[] _boxColliders;

	public int lastPhysicsVertsCount;

	public float antiAliasingWidth = 0.5f;

	public float landscapeOutlineAlign = 1f;

	public static bool showSplineGizmos = true;

	public static bool showOtherGizmos = true;

	public float maxBeakLength = 3f;

	public bool optimize;

	public float optimizeAngle = 5f;

	public ShowCoordinates showCoordinates;

	public float gridSize = 5f;

	public Color gridColor = new Color(1f, 1f, 1f, 0.2f);

	public bool showGrid = true;

	public int gridExpansion = 2;

	public bool snapToGrid = true;

	public bool snapHandlesToGrid;

	public bool showNumberInputs;

	public RageSplineStyle style;

	public string defaultStyleName = "Stylename";

	public bool styleLocalGradientPositioning;

	public bool styleLocalTexturePositioning;

	public bool styleLocalEmbossPositioning;

	public bool styleLocalAntialiasing;

	public bool styleLocalVertexCount;

	public bool styleLocalPhysicsColliderCount;

	public static bool showWireFrameInEditor;

	public bool hideHandles;

	public bool FlattenZ = true;

	public bool FixOverlaps = true;

	public bool isDirty;

	public bool PinUvs;

	public bool AutoRefresh;

	public float AutoRefreshDelay = 0.15f;

	public bool AutoRefreshDelayRandomize = true;

	public bool PerspectiveMode;

	public bool CurrentPerspective;

	[SerializeField]
	private MeshFilter _mFilter;

	public RageCurve spline;

	public bool lowQualityRender;

	public bool inverseTriangleDrawOrder;

	public float overLappingVertsShift = 0.01f;

	private ArrayList overLappingVerts;

	public Material CachedFillMaterial;

	public Material Cached3DFillMaterial;

	public Material Cached3DAAMaterial;

	private static Quaternion normalRotationQuat = Quaternion.AngleAxis(90f, Vector3.forward);

	private static Color DefaultSolidColor = new Color(1f, 1f, 1f, 1f);

	private static Color DefaultTransparentColor = new Color(1f, 1f, 1f, 0f);

	public static Vector3 FillAaOffset = new Vector3(0f, 0f, -0.001f);

	public static Vector3 OutlineOffset = new Vector3(0f, 0f, -0.001f);

	public static Vector3 OutlineAaOffset = new Vector3(0f, 0f, -0.001f);

	[SerializeField]
	private static ScriptableObject _triangulator;

	[SerializeField]
	private RageCamera _rageCamera;

	public bool DestroyFarseerNow;

	public bool ShowTriangleCount;

	public bool FarseerPhysicsPointsOnly;

	public bool PhysicsIsTrigger;

	public bool WorldAaWidth;

	public static RageSpline sourceStyleSpline;

	public static bool CopyStylingAlpha;

	public int VertexDensity
	{
		get
		{
			return _vertexDensity;
		}
		set
		{
			_vertexDensity = value;
			SetVertexCount((int)Mathf.Max(1f, GetPointCount() * _vertexDensity));
		}
	}

	public BoxCollider[] BoxColliders
	{
		get
		{
			return _boxColliders;
		}
		set
		{
			_boxColliders = value;
		}
	}

	public MeshFilter Mfilter
	{
		get
		{
			if (_mFilter == null)
			{
				_mFilter = GetComponent<MeshFilter>();
				if (_mFilter == null)
				{
					_mFilter = base.gameObject.AddComponent<MeshFilter>();
				}
			}
			return _mFilter;
		}
		set
		{
			_mFilter = value;
		}
	}

	public MeshRenderer Mrenderer
	{
		get
		{
			if (GetComponent<Renderer>() == null)
			{
				base.gameObject.AddComponent<MeshRenderer>();
			}
			return (MeshRenderer)GetComponent<Renderer>();
		}
	}

	public bool Visible
	{
		get
		{
			return Mrenderer.enabled;
		}
		set
		{
			Mrenderer.enabled = value;
		}
	}

	public IRageTriangulator Triangulator
	{
		get
		{
			if (_triangulator == null)
			{
				_triangulator = ScriptableObject.CreateInstance<RageTriangulator>();
			}
			return (IRageTriangulator)_triangulator;
		}
		set
		{
			_triangulator = (ScriptableObject)value;
		}
	}

	public void OnEnable()
	{
		if (!(Camera.main == null))
		{
			if (_rageCamera == null)
			{
				_rageCamera = Camera.main.GetComponent<RageCamera>();
			}
			if (_rageCamera == null)
			{
				_rageCamera = Camera.main.gameObject.AddComponent<RageCamera>();
			}
		}
	}

	public void Awake()
	{
		if (spline == null)
		{
			CreateDefaultSpline();
		}
		overLappingVerts = new ArrayList();
		MeshCreationCheck();
		if (AutoRefresh)
		{
			StartCoroutine(AutoRefreshScheduler());
		}
	}

	private IEnumerator AutoRefreshScheduler()
	{
		while (true)
		{
			yield return new WaitForSeconds((!AutoRefreshDelayRandomize) ? AutoRefreshDelay : (AutoRefreshDelay + UnityEngine.Random.Range(0.001f, 0.01f)));
			RefreshMesh();
		}
	}

	private void MeshCreationCheck()
	{
		if (!Application.isPlaying)
		{
			if (Mfilter != null)
			{
				Mfilter.sharedMesh = null;
			}
			RefreshMesh();
			return;
		}
		if (Mfilter == null)
		{
			RefreshMesh();
			return;
		}
		if (Mfilter.sharedMesh == null)
		{
			RefreshMesh();
			return;
		}
		if (Mfilter.mesh.vertexCount == 0)
		{
			RefreshMesh();
		}
		if (spline == null)
		{
			CreateDefaultSpline();
		}
		spline.PrecalcNormals(GetVertexCount() + 1);
		if (GetPhysics() == Physics.Boxed && (BoxColliders == null || BoxColliders.Length == 0))
		{
			CreateBoxCollidersCache();
		}
		if (!GetCreatePhysicsInEditor() && GetPhysics() != 0)
		{
			RefreshPhysics();
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (showSplineGizmos)
		{
			DrawSplineGizmos();
		}
		if (showGrid && showCoordinates != 0)
		{
			DrawGrid();
		}
		if (showOtherGizmos)
		{
			if (GetEmboss() != 0)
			{
				DrawEmbossGizmos();
			}
			if (GetFill() == Fill.Gradient)
			{
				DrawGradientGizmos();
			}
			if (GetTexturing1() != 0 && GetTexturing1() == UVMapping.Fill && GetFill() != 0)
			{
				DrawTexturingGizmos();
			}
			if (GetTexturing2() != 0 && GetTexturing2() == UVMapping.Fill && GetFill() != 0)
			{
				DrawTexturingGizmos2();
			}
		}
	}

	public void RefreshMesh()
	{
		SetVertexCount(GetVertexCount());
		if (overLappingVerts == null)
		{
			overLappingVerts = new ArrayList();
		}
		if (!Mathfx.Approximately(base.gameObject.transform.localScale.x, 0f) && !Mathfx.Approximately(base.gameObject.transform.localScale.y, 0f))
		{
			if (spline == null)
			{
				CreateDefaultSpline();
			}
			spline.PrecalcNormals(GetVertexCount() + 1);
			GenerateMesh(true, true);
			RefreshPhysics();
		}
	}

	public void RefreshMesh(bool refreshFillTriangulation, bool refreshNormals, bool refreshPhysics)
	{
		SetVertexCount(GetVertexCount());
		if (Mathf.Abs(base.gameObject.transform.localScale.x) > 0f && Mathf.Abs(base.gameObject.transform.localScale.y) > 0f)
		{
			if (refreshNormals)
			{
				spline.PrecalcNormals(GetVertexCount() + 1);
			}
			GenerateMesh(refreshFillTriangulation, true);
			if (refreshPhysics)
			{
				RefreshPhysics();
			}
		}
	}

	public void RefreshMeshInEditor(bool forceRefresh, bool triangulation, bool precalcNormals)
	{
		SetVertexCount(GetVertexCount());
		if (overLappingVerts == null)
		{
			overLappingVerts = new ArrayList();
		}
		lowQualityRender = !forceRefresh && GetVertexCount() > 128;
		if ((GetVertexCount() <= 128 || forceRefresh) && Mathf.Abs(base.gameObject.transform.localScale.x) > 0f && Mathf.Abs(base.gameObject.transform.localScale.y) > 0f)
		{
			if (forceRefresh && !pointsAreInClockWiseOrder())
			{
				flipPointOrder();
			}
			if (precalcNormals)
			{
				spline.PrecalcNormals(GetVertexCount() + 1);
			}
			GenerateMesh(triangulation, true);
			if (forceRefresh)
			{
				RefreshPhysics();
			}
		}
		MaterialCheck();
	}

	public void MaterialCheck()
	{
		if (GetComponent<Renderer>() == null)
		{
			return;
		}
		if (PerspectiveMode)
		{
			if (Mrenderer.sharedMaterials.Length == 1)
			{
				AssignMaterials();
			}
			if (Mrenderer.sharedMaterials[0] == null && GetComponent<Renderer>().sharedMaterials[1] == null)
			{
				AssignMaterials();
			}
		}
		else if (Mrenderer.sharedMaterials[0] == null)
		{
			AssignMaterials();
		}
	}

	private void AssignMaterials()
	{
		if (PerspectiveMode)
		{
			if (Cached3DFillMaterial == null)
			{
				AssignDefaultMaterials();
			}
			else
			{
				AssignCachedMaterials();
			}
		}
		else if (CachedFillMaterial == null)
		{
			AssignDefaultMaterials();
		}
		else
		{
			AssignCachedMaterials();
		}
	}

	public void AssignCachedMaterials()
	{
		if (PerspectiveMode)
		{
			Material[] sharedMaterials = new Material[2]
			{
				Cached3DFillMaterial,
				Resources.Load("RS3DAA") as Material
			};
			Mrenderer.sharedMaterials = sharedMaterials;
		}
		else
		{
			Material[] sharedMaterials2 = new Material[1] { CachedFillMaterial };
			Mrenderer.sharedMaterials = sharedMaterials2;
		}
	}

	public void AssignDefaultMaterials()
	{
		if (PerspectiveMode)
		{
			Material[] sharedMaterials = new Material[2]
			{
				Resources.Load("RS3DBasicFill") as Material,
				Resources.Load("RS3DAA") as Material
			};
			Mrenderer.sharedMaterials = sharedMaterials;
		}
		else
		{
			Material[] sharedMaterials2 = new Material[1] { Resources.Load("RageSplineMaterial") as Material };
			Mrenderer.sharedMaterials = sharedMaterials2;
		}
	}

	public void SwitchPerspectiveMode()
	{
		Material material = Mrenderer.sharedMaterials[0];
		if (PerspectiveMode)
		{
			CachedFillMaterial = material;
		}
		else
		{
			Cached3DFillMaterial = material;
			if (Mrenderer.sharedMaterials.Length == 2)
			{
				Cached3DAAMaterial = Mrenderer.sharedMaterials[1];
			}
		}
		AssignMaterials();
	}

	public MeshFilter GenerateMesh(bool refreshTriangulation, bool useOwners)
	{
		if (FlattenZ && !PerspectiveMode)
		{
			ForceZeroZ();
		}
		if (GetFill() != 0 && FixOverlaps)
		{
			ShiftOverlappingControlPoints();
		}
		bool flag = false;
		float antialiasingWidth = GetAntialiasingWidth();
		if (antialiasingWidth > 0f)
		{
			if (inverseTriangleDrawOrder)
			{
				flag = true;
			}
			else if (GetOutline() == Outline.None || Mathf.Abs(GetOutlineNormalOffset()) > GetOutlineWidth() + antialiasingWidth)
			{
				flag = true;
			}
		}
		bool flag2 = antialiasingWidth > 0f;
		bool flag3 = antialiasingWidth > 0f;
		bool multipleMaterials = Mrenderer.sharedMaterials.GetLength(0) > 1;
		RageVertex[] array = GenerateOutlineVerts(flag2, multipleMaterials);
		RageVertex[] array2 = GenerateFillVerts(flag, multipleMaterials);
		RageVertex[] array3 = GenerateEmbossVerts(flag3);
		int num = array.Length + array2.Length + array3.Length;
		Vector3[] array4 = new Vector3[num];
		Vector2[] array5 = new Vector2[num];
		Vector2[] array6 = new Vector2[num];
		Color[] array7 = new Color[num];
		int num2 = 0;
		for (int i = 0; i < array2.Length; i++)
		{
			array4[num2] = array2[i].position;
			array5[num2] = array2[i].uv1;
			array6[num2] = array2[i].uv2;
			array7[num2] = array2[i].color;
			num2++;
		}
		for (int j = 0; j < array3.Length; j++)
		{
			array4[num2] = array3[j].position;
			array5[num2] = array3[j].uv1;
			array6[num2] = array3[j].uv2;
			array7[num2] = array3[j].color;
			num2++;
		}
		for (int k = 0; k < array.Length; k++)
		{
			array4[num2] = array[k].position;
			array5[num2] = array[k].uv1;
			array6[num2] = array[k].uv2;
			array7[num2] = array[k].color;
			num2++;
		}
		GenerateTrianglesSetup(Mfilter, refreshTriangulation, array4, array2, array3, array, flag, flag3, flag2, multipleMaterials, array5, array6, array7);
		if (GetFill() != 0 && FixOverlaps)
		{
			UnshiftOverlappingControlPoints();
		}
		return (!useOwners) ? Mfilter : null;
	}

	private void GenerateTrianglesSetup(MeshFilter meshFilter, bool refreshTriangulation, Vector3[] verts, RageVertex[] fillVerts, RageVertex[] embossVerts, RageVertex[] outlineVerts, bool fillAntialiasing, bool embossAntialiasing, bool outlineAntialiasing, bool multipleMaterials, Vector2[] uvs, Vector2[] uvs2, Color[] colors)
	{
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		if (refreshTriangulation)
		{
			mesh.Clear();
		}
		mesh.vertices = verts;
		if (refreshTriangulation)
		{
			GenerateTriangles(mesh, fillVerts, embossVerts, outlineVerts, fillAntialiasing, embossAntialiasing, outlineAntialiasing, multipleMaterials);
		}
		if (!PinUvs)
		{
			mesh.uv = uvs;
			mesh.uv2 = uvs2;
		}
		mesh.colors = colors;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		meshFilter.sharedMesh = mesh;
	}

	public void scalePoints(Vector3 middle, float scale)
	{
		for (int i = 0; i < GetPointCount(); i++)
		{
			Vector3 position = GetPosition(i);
			SetPoint(i, (position - middle) * scale + middle);
		}
	}

	public void scaleHandles(float scale)
	{
		for (int i = 0; i < GetPointCount(); i++)
		{
			Vector3 inControlPositionPointSpace = GetInControlPositionPointSpace(i);
			Vector3 outControlPositionPointSpace = GetOutControlPositionPointSpace(i);
			SetPoint(i, GetPosition(i), inControlPositionPointSpace * scale, outControlPositionPointSpace * scale);
		}
	}

	public void setPivotCenter()
	{
		Vector3 middle = GetMiddle();
		for (int i = 0; i < GetPointCount(); i++)
		{
			SetPoint(i, GetPosition(i) - middle);
		}
		base.transform.position += base.transform.TransformPoint(middle) - base.transform.position;
	}

	public void CreateBoxCollidersCache()
	{
		BoxColliders = new BoxCollider[GetPhysicsColliderCount()];
		int childCount = base.transform.childCount;
		int num = 0;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			if (!gameObject.name.Substring(0, 4).Equals("ZZZ_"))
			{
				continue;
			}
			BoxCollider boxCollider = gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
			if (boxCollider != null)
			{
				if (num < BoxColliders.Length)
				{
					BoxColliders[num++] = boxCollider;
				}
				else
				{
					Debug.Log("Error caching the boxcolliders. Amount of boxcolliders doesn't match the count variable.");
				}
			}
		}
	}

	public void ShiftOverlappingControlPoints()
	{
		if (overLappingVerts == null)
		{
			return;
		}
		overLappingVerts.Clear();
		int pointCount = GetPointCount();
		for (int i = 0; i < pointCount; i++)
		{
			for (int j = i + 1; j < pointCount; j++)
			{
				if (Mathfx.Approximately(GetPosition(i).x, GetPosition(j).x) && Mathfx.Approximately(GetPosition(i).y, GetPosition(j).y))
				{
					SetPoint(i, GetPosition(i) + GetNormal(i) * -1f * overLappingVertsShift);
					SetPoint(j, GetPosition(j) + GetNormal(j) * -1f * overLappingVertsShift);
					overLappingVerts.Add(i);
					overLappingVerts.Add(j);
				}
			}
		}
	}

	public void UnshiftOverlappingControlPoints()
	{
		if (overLappingVerts == null)
		{
			return;
		}
		foreach (int overLappingVert in overLappingVerts)
		{
			SetPoint(overLappingVert, GetPosition(overLappingVert) + GetNormal(overLappingVert) * overLappingVertsShift);
		}
	}

	public RageVertex[] GenerateOutlineVerts(bool antialiasing, bool multipleMaterials)
	{
		RageVertex[] result = new RageVertex[0];
		if (GetOutline() != 0)
		{
			result = OutlineVerts(antialiasing, multipleMaterials);
		}
		return result;
	}

	private RageVertex[] OutlineVerts(bool antialiasing, bool multipleMaterials)
	{
		RageVertex[] array = ((GetOutline() != Outline.Free || GetFill() == Fill.None || GetFill() == Fill.Landscape) ? GetSplits(GetVertexCount(), 0f, 1f) : GetSplits(GetVertexCount() - Mathf.FloorToInt((float)GetVertexCount() * 1f / (float)GetPointCount()), 0f, 1f - 1f / (float)GetPointCount()));
		int num = array.Length;
		float num2 = 0f;
		RageVertex[] array2 = ((!antialiasing) ? new RageVertex[array.Length * 2] : new RageVertex[array.Length * 4]);
		for (int i = 0; i < array.Length; i++)
		{
			float edgeWidth = GetOutlineWidth(array[i].splinePosition * GetLastSplinePosition());
			float antialiasingWidth = GetAntialiasingWidth(array[i].splinePosition * GetLastSplinePosition());
			Vector3 normal = OutlineVertsCheckCorner(array, i, ref edgeWidth);
			if (i > 0)
			{
				num2 += (array[i].position - array[i - 1].position).magnitude;
			}
			Vector3 scaledNormal = ScaleToLocal(normal.normalized);
			Vector3 normalized = normal.normalized;
			float num3 = GetOutlineNormalOffset();
			array2 = OutlineVertsProcessSplit(antialiasing, scaledNormal, num, edgeWidth, num3, normalized, array2, i, normal, array, antialiasingWidth);
			Color defaultSolidColor = DefaultSolidColor;
			Color defaultSolidColor2 = DefaultSolidColor;
			switch (GetOutlineGradient())
			{
			case OutlineGradient.None:
				defaultSolidColor = GetOutlineColor1();
				defaultSolidColor2 = GetOutlineColor1();
				break;
			case OutlineGradient.Default:
				defaultSolidColor = GetOutlineColor1();
				defaultSolidColor2 = GetOutlineColor2();
				break;
			case OutlineGradient.Inverse:
				defaultSolidColor = GetOutlineColor2();
				defaultSolidColor2 = GetOutlineColor1();
				break;
			}
			if (antialiasing)
			{
				array2[i].color = defaultSolidColor2 * DefaultTransparentColor;
				array2[i + 1 * num].color = defaultSolidColor2 * DefaultSolidColor;
				array2[i + 2 * num].color = defaultSolidColor * DefaultSolidColor;
				array2[i + 3 * num].color = defaultSolidColor * DefaultTransparentColor;
			}
			else
			{
				array2[i].color = defaultSolidColor2 * DefaultSolidColor;
				array2[i + 1 * num].color = defaultSolidColor * DefaultSolidColor;
			}
			float num4 = 0f;
			if (antialiasingWidth > 0f && edgeWidth > 0f && antialiasing)
			{
				num4 = antialiasingWidth / edgeWidth;
			}
			if (!multipleMaterials)
			{
				OutlineVertsSingleMaterial(antialiasing, num2, num4, array2, i, num);
			}
			else if (antialiasing)
			{
				array2[i + 0 * num].uv1 = new Vector2(num2 / GetOutlineTexturingScaleInv(), 1f);
				array2[i + 1 * num].uv1 = new Vector2(num2 / GetOutlineTexturingScaleInv(), 1f - num4 * 0.5f);
				array2[i + 2 * num].uv1 = new Vector2(num2 / GetOutlineTexturingScaleInv(), num4 * 0.5f);
				array2[i + 3 * num].uv1 = new Vector2(num2 / GetOutlineTexturingScaleInv(), 0f);
			}
			else
			{
				array2[i + 0 * num].uv1 = new Vector2(num2 / GetOutlineTexturingScaleInv(), 0.99f);
				array2[i + 1 * num].uv1 = new Vector2(num2 / GetOutlineTexturingScaleInv(), 0.01f);
			}
		}
		return array2;
	}

	private Vector3 OutlineVertsCheckCorner(RageVertex[] splits, int v, ref float edgeWidth)
	{
		Vector3 result;
		if (corners != Corner.Beak)
		{
			if (GetFill() != Fill.Landscape)
			{
				result = GetNormal(splits[v].splinePosition);
			}
			else
			{
				result = Vector3.up * GetLandscapeOutlineAlign() + GetNormal(splits[v].splinePosition) * (1f - GetLandscapeOutlineAlign());
				result.Normalize();
			}
		}
		else
		{
			if (outline != Outline.Free || (v < splits.Length - 1 && v > 0))
			{
				result = FindNormal(splits[GetIndex(v - 1, splits.Length)].position, splits[v].position, splits[GetIndex(v + 1, splits.Length)].position, edgeWidth);
				result *= -1f;
				edgeWidth = 1f;
			}
			else if (v < splits.Length - 1 && v > 0)
			{
				result = GetNormal(splits[v].splinePosition * GetLastSplinePosition());
			}
			else if (v == 0)
			{
				result = Vector3.zero;
				try
				{
					result = FindNormal(splits[0].position + (splits[0].position - splits[1].position), splits[0].position, splits[1].position, edgeWidth);
				}
				catch
				{
				}
				result *= -1f;
				edgeWidth = 1f;
			}
			else
			{
				result = FindNormal(splits[splits.Length - 1].position + (splits[splits.Length - 1].position - splits[splits.Length - 2].position), splits[splits.Length - 1].position, splits[splits.Length - 2].position, edgeWidth);
				edgeWidth = 1f;
			}
			if (result.magnitude > maxBeakLength * OutlineWidth)
			{
				result = result.normalized * maxBeakLength * OutlineWidth;
			}
		}
		return result;
	}

	private RageVertex[] OutlineVertsProcessSplit(bool antialiasing, Vector3 scaledNormal, int vertsInBand, float edgeWidth, float outlineNormalOffset, Vector3 normalizedNormal, RageVertex[] outlineVerts, int v, Vector3 normal, RageVertex[] splits, float AAWidth)
	{
		Vector3 v2 = Mathfx.Mult(ref normal, ref edgeWidth, 0.5f);
		Vector3 v3 = ((!PerspectiveMode) ? Vector3.zero : OutlineAaOffset);
		Vector3 v4 = ((!PerspectiveMode) ? Vector3.zero : OutlineOffset);
		if (antialiasing)
		{
			Vector3 v5 = ((v == 0 && GetOutline() == Outline.Free) ? Vector3.Cross(normal, Mathfx.Mult(Vector3.back, AAWidth)) : ((v != splits.Length - 1 || GetOutline() != Outline.Free) ? Vector3.zero : Vector3.Cross(normal, Mathfx.Mult(Vector3.back, 0f - AAWidth))));
			Vector3 v6 = Mathfx.Mult(ref normalizedNormal, ref outlineNormalOffset);
			Vector3 v7 = Mathfx.Mult(ref scaledNormal, ref AAWidth);
			outlineVerts[v + 0 * vertsInBand].position = Mathfx.Add(ref splits[v].position, ref v7, ref v2, ref v6, ref v5, ref v3);
			outlineVerts[v + 1 * vertsInBand].position = Mathfx.Add(ref splits[v].position, ref v2, ref v6, ref v4);
			outlineVerts[v + 2 * vertsInBand].position = v4 + splits[v].position - Mathfx.Add(ref v2, ref v6);
			outlineVerts[v + 3 * vertsInBand].position = splits[v].position - v7 - v2 + Mathfx.Add(ref v6, ref v5, ref v3);
		}
		else
		{
			Vector3 v8 = Mathfx.Mult(ref normal, ref outlineNormalOffset);
			outlineVerts[v + 0 * vertsInBand].position = Mathfx.Add(ref splits[v].position, ref v2, ref v8, ref v4);
			outlineVerts[v + 1 * vertsInBand].position = Mathfx.Add(ref splits[v].position, ref v8, ref v4) - v2;
		}
		return outlineVerts;
	}

	private void OutlineVertsSingleMaterial(bool antialiasing, float uvPos, float AAWidthRelatedToEdgeWidth, RageVertex[] outlineVerts, int v, int vertsInBand)
	{
		switch (GetTexturing1())
		{
		case UVMapping.None:
		case UVMapping.Fill:
			outlineVerts[v + 0 * vertsInBand].uv1 = Vector2.zero;
			outlineVerts[v + 1 * vertsInBand].uv1 = Vector2.zero;
			if (antialiasing)
			{
				outlineVerts[v + 2 * vertsInBand].uv1 = Vector2.zero;
				outlineVerts[v + 3 * vertsInBand].uv1 = Vector2.zero;
			}
			break;
		case UVMapping.Outline:
			if (antialiasing)
			{
				outlineVerts[v + 0 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 1f);
				outlineVerts[v + 1 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 1f - AAWidthRelatedToEdgeWidth * 0.5f);
				outlineVerts[v + 2 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), AAWidthRelatedToEdgeWidth * 0.5f);
				outlineVerts[v + 3 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 0f);
			}
			else
			{
				outlineVerts[v + 0 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 1f);
				outlineVerts[v + 1 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 0f);
			}
			break;
		}
		switch (GetTexturing2())
		{
		case UVMapping.None:
		case UVMapping.Fill:
			if (antialiasing)
			{
				outlineVerts[v + 0 * vertsInBand].uv2 = Vector2.zero;
				outlineVerts[v + 1 * vertsInBand].uv2 = Vector2.zero;
				outlineVerts[v + 2 * vertsInBand].uv2 = Vector2.zero;
				outlineVerts[v + 3 * vertsInBand].uv2 = Vector2.zero;
			}
			else
			{
				outlineVerts[v + 0 * vertsInBand].uv2 = Vector2.zero;
				outlineVerts[v + 1 * vertsInBand].uv2 = Vector2.zero;
			}
			break;
		case UVMapping.Outline:
			if (antialiasing)
			{
				outlineVerts[v + 0 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 1f);
				outlineVerts[v + 1 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 1f - AAWidthRelatedToEdgeWidth * 0.5f);
				outlineVerts[v + 2 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), AAWidthRelatedToEdgeWidth * 0.5f);
				outlineVerts[v + 3 * vertsInBand].uv1 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 0f);
			}
			else
			{
				outlineVerts[v + 0 * vertsInBand].uv2 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 0.99f);
				outlineVerts[v + 1 * vertsInBand].uv2 = new Vector2(uvPos / GetOutlineTexturingScaleInv(), 0.01f);
			}
			break;
		}
	}

	public RageVertex[] GenerateFillVerts(bool antialiasing, bool multipleMaterials)
	{
		RageVertex[] array = new RageVertex[0];
		switch (GetFill())
		{
		case Fill.Solid:
		case Fill.Gradient:
		{
			RageVertex[] splits2 = GetSplits(GetVertexCount() - 1, 0f, 1f - 1f / (float)GetVertexCount());
			array = ((!antialiasing) ? new RageVertex[splits2.Length] : new RageVertex[splits2.Length * 2]);
			for (int j = 0; j < splits2.Length; j++)
			{
				Vector3 normal2 = GetNormal(splits2[j].splinePosition);
				Vector3 vector2 = ScaleToLocal(normal2);
				if (antialiasing)
				{
					array[j].position = splits2[j].position;
					array[j + splits2.Length].position = splits2[j].position + vector2 * GetAntialiasingWidth();
					if (PerspectiveMode)
					{
						array[j + splits2.Length].position += FillAaOffset;
					}
					array[j].color = GetFillColor(array[j].position);
					array[j + splits2.Length].color = GetFillColor(array[j + splits2.Length].position) * DefaultTransparentColor;
				}
				else
				{
					array[j].position = splits2[j].position;
					array[j].color = GetFillColor(array[j].position);
				}
				if (!multipleMaterials)
				{
					switch (GetTexturing1())
					{
					case UVMapping.None:
					case UVMapping.Outline:
						array[j].uv1 = new Vector2(0f, 0f);
						break;
					case UVMapping.Fill:
						array[j].uv1 = GetFillUV(array[j].position);
						if (antialiasing)
						{
							array[j + splits2.Length].uv1 = GetFillUV(array[j + splits2.Length].position);
						}
						break;
					}
					switch (GetTexturing2())
					{
					case UVMapping.None:
					case UVMapping.Outline:
						array[j].uv2 = new Vector2(0f, 0f);
						break;
					case UVMapping.Fill:
						array[j].uv2 = GetFillUV2(array[j].position);
						if (antialiasing)
						{
							array[j + splits2.Length].uv2 = GetFillUV2(array[j + splits2.Length].position);
						}
						break;
					}
				}
				else
				{
					array[j].uv1 = GetFillUV(array[j].position);
					if (antialiasing)
					{
						array[j + splits2.Length].uv1 = GetFillUV(array[j + splits2.Length].position);
					}
				}
			}
			break;
		}
		case Fill.Landscape:
		{
			RageVertex[] splits = GetSplits(GetVertexCount(), 0f, 1f);
			float y = GetBounds().yMin - Mathf.Clamp(GetLandscapeBottomDepth(), 1f, 100000000f);
			array = ((!antialiasing) ? new RageVertex[splits.Length * 2] : new RageVertex[splits.Length * 3]);
			for (int i = 0; i < splits.Length; i++)
			{
				Vector3 normal = GetNormal(splits[i].splinePosition);
				Vector3 vector = ScaleToLocal(normal);
				if (antialiasing)
				{
					array[i].position = new Vector3(splits[i].position.x, y);
					array[i + splits.Length].position = splits[i].position;
					array[i + splits.Length * 2].position = splits[i].position + vector * GetAntialiasingWidth();
					if (PerspectiveMode)
					{
						array[i + splits.Length * 2].position += FillAaOffset;
					}
					array[i].color = GetFillColor2();
					array[i + splits.Length].color = GetFillColor1();
					array[i + splits.Length * 2].color = GetFillColor1() * new Color(1f, 1f, 1f, 0f);
				}
				else
				{
					array[i].position = new Vector3(splits[i].position.x, y);
					array[i + splits.Length].position = splits[i].position;
					array[i].color = GetFillColor2();
					array[i + splits.Length].color = GetFillColor1();
				}
				if (!multipleMaterials)
				{
					switch (GetTexturing1())
					{
					case UVMapping.None:
					case UVMapping.Outline:
						array[i].uv1 = new Vector2(0f, 0f);
						array[i + splits.Length].uv1 = new Vector2(0f, 0f);
						if (antialiasing)
						{
							array[i + splits.Length * 2].uv1 = new Vector2(0f, 0f);
						}
						break;
					case UVMapping.Fill:
						array[i].uv1 = GetFillUV(array[i].position);
						array[i + splits.Length].uv1 = GetFillUV(array[i + splits.Length].position);
						if (antialiasing)
						{
							array[i + splits.Length * 2].uv1 = GetFillUV(array[i + splits.Length * 2].position);
						}
						break;
					}
					switch (GetTexturing2())
					{
					case UVMapping.None:
					case UVMapping.Outline:
						array[i].uv2 = new Vector2(0f, 0f);
						array[i + splits.Length].uv2 = new Vector2(0f, 0f);
						if (antialiasing)
						{
							array[i + splits.Length * 2].uv2 = new Vector2(0f, 0f);
						}
						break;
					case UVMapping.Fill:
						array[i].uv2 = GetFillUV(array[i].position);
						array[i + splits.Length].uv2 = GetFillUV(array[i + splits.Length].position);
						if (antialiasing)
						{
							array[i + splits.Length * 2].uv2 = GetFillUV(array[i + splits.Length * 2].position);
						}
						break;
					}
				}
				else
				{
					array[i].uv1 = GetFillUV(array[i].position);
					array[i + splits.Length].uv1 = GetFillUV(array[i + splits.Length].position);
					if (antialiasing)
					{
						array[i + splits.Length * 2].uv1 = GetFillUV(array[i + splits.Length * 2].position);
					}
				}
			}
			break;
		}
		}
		return array;
	}

	public RageVertex[] GenerateEmbossVerts(bool antialiasing)
	{
		RageVertex[] array = new RageVertex[0];
		if (GetEmboss() != 0)
		{
			RageVertex[] splits = GetSplits(GetVertexCount(), 0f, 1f);
			int num = splits.Length;
			array = ((!antialiasing) ? new RageVertex[splits.Length * 2] : new RageVertex[splits.Length * 4]);
			Vector3 vector = RotatePoint2D_CCW(new Vector3(0f, -1f, 0f), GetEmbossAngleDeg() / (180f / (float)Math.PI));
			Vector3[] array2 = new Vector3[splits.Length];
			Vector3[] array3 = new Vector3[splits.Length];
			float[] array4 = new float[splits.Length];
			float[] array5 = new float[splits.Length];
			for (int i = 0; i < splits.Length; i++)
			{
				float num2 = (float)i / (float)splits.Length;
				array3[i] = spline.GetAvgNormal(num2 * GetLastSplinePosition(), 0.05f, 3);
				if (i == splits.Length - 1)
				{
					array3[i] = array3[0];
				}
				array4[i] = Vector3.Dot(vector, array3[i]);
				array5[i] = Mathf.Clamp01(Mathf.Abs(array4[i]) - GetEmbossOffset());
				if (array4[i] > 0f)
				{
					array2[i] = (vector - array3[i] * 2f).normalized * GetEmbossSize() * array5[i];
				}
				else
				{
					array2[i] = (vector + array3[i] * 2f).normalized * GetEmbossSize() * array5[i] * -1f;
				}
			}
			for (int j = 0; j < splits.Length; j++)
			{
				Vector3 vector2 = default(Vector3);
				int num3 = j;
				if (j == splits.Length - 1)
				{
					num3 = 0;
				}
				for (int k = -Mathf.FloorToInt(GetEmbossSmoothness()); k <= Mathf.FloorToInt(GetEmbossSmoothness()) + 1; k++)
				{
					if (k != 0)
					{
						vector2 += array2[mod(num3 - k, splits.Length)] * (1f - (float)Mathf.Abs(k) / (GetEmbossSmoothness() + 1f));
					}
					else
					{
						vector2 += array2[mod(num3 - k, splits.Length)];
					}
				}
				vector2 *= 1f / (float)(Mathf.FloorToInt(GetEmbossSmoothness()) * 2 + 1);
				if (antialiasing)
				{
					array[j + 0 * num].position = splits[j].position - vector2.normalized * GetAntialiasingWidth() * 1f;
					array[j + 1 * num].position = splits[j].position;
					array[j + 2 * num].position = splits[j].position + vector2;
					array[j + 3 * num].position = splits[j].position + vector2 + vector2.normalized * GetAntialiasingWidth();
				}
				else
				{
					array[j + 0 * num].position = splits[j].position;
					array[j + 1 * num].position = splits[j].position + vector2;
				}
				if (vector2.sqrMagnitude > 0.0001f)
				{
					if (array4[j] < 0f)
					{
						if (GetEmboss() == Emboss.Sharp)
						{
							if (antialiasing)
							{
								array[j + 0 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
								array[j + 1 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
								array[j + 2 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
								array[j + 3 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
							}
							else
							{
								array[j + 0 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
								array[j + 1 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
							}
						}
						else if (antialiasing)
						{
							array[j + 0 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
							array[j + 1 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
							array[j + 2 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
							array[j + 3 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
						}
						else
						{
							array[j + 0 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
							array[j + 1 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
						}
					}
					else if (GetEmboss() == Emboss.Sharp)
					{
						if (antialiasing)
						{
							array[j + 0 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, 0f);
							array[j + 1 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
							array[j + 2 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
							array[j + 3 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, 0f);
						}
						else
						{
							array[j + 0 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
							array[j + 1 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
						}
					}
					else if (antialiasing)
					{
						array[j + 0 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, 0f);
						array[j + 1 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
						array[j + 2 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, 0f);
						array[j + 3 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, 0f);
					}
					else
					{
						array[j + 0 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, Mathf.Clamp01(array5[j] * 4f));
						array[j + 1 * num].color = GetEmbossColor2() * new Color(1f, 1f, 1f, 0f);
					}
				}
				else
				{
					if (antialiasing)
					{
						array[j + 0 * num].position = splits[j].position - vector2.normalized * GetAntialiasingWidth();
						array[j + 1 * num].position = splits[j].position;
						array[j + 2 * num].position = splits[j].position;
						array[j + 3 * num].position = splits[j].position;
					}
					else
					{
						array[j + 0 * num].position = splits[j].position;
						array[j + 1 * num].position = splits[j].position;
					}
					if (antialiasing)
					{
						array[j + 0 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
						array[j + 1 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
						array[j + 2 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
						array[j + 3 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
					}
					else
					{
						array[j + 0 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
						array[j + 1 * num].color = GetEmbossColor1() * new Color(1f, 1f, 1f, 0f);
					}
				}
				if (antialiasing)
				{
					array[j + 0 * num].uv1 = new Vector2(0f, 0f);
					array[j + 1 * num].uv1 = new Vector2(0f, 0f);
					array[j + 2 * num].uv1 = new Vector2(0f, 0f);
					array[j + 3 * num].uv1 = new Vector2(0f, 0f);
				}
				else
				{
					array[j + 0 * num].uv1 = new Vector2(0f, 0f);
					array[j + 1 * num].uv1 = new Vector2(0f, 0f);
				}
			}
		}
		else
		{
			array = new RageVertex[0];
		}
		return array;
	}

	public RageVertex[] GetSplits(int vertCount, float start, float end)
	{
		RageVertex[] array = new RageVertex[vertCount + 1];
		float lastSplinePosition = GetLastSplinePosition();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].splinePosition = Mathf.Clamp01((float)i / (float)vertCount * (end - start) + start);
			if (Mathfx.Approximately(array[i].splinePosition, 1f) && !SplineIsOpenEnded())
			{
				array[i].splinePosition = 0f;
			}
			array[i].splineSegmentPosition = spline.GetSegmentPosition(array[i].splinePosition);
			array[i].position = spline.GetPoint(array[i].splinePosition * lastSplinePosition);
			array[i].curveStart = spline.points[spline.GetFloorIndex(array[i].splinePosition)];
			array[i].curveEnd = spline.points[spline.GetCeilIndex(array[i].splinePosition)];
			array[i].color = new Color(1f, 1f, 1f, 1f);
		}
		if (GetOptimize())
		{
			array = Optimize(array);
		}
		return array;
	}

	private RageVertex[] Optimize(RageVertex[] array)
	{
		bool[] array2 = new bool[array.Length];
		int num = 0;
		int num2 = 0;
		for (int i = 1; i < array.Length - 1; i++)
		{
			Vector3 position = array[i].position;
			if (Vector3.SqrMagnitude(position - spline.points[GetNearestPointIndex(position)].point) < 0.1f)
			{
				num2 = i;
				continue;
			}
			Vector3 position2 = array[num2].position;
			Vector3 position3 = array[i + 1].position;
			Vector3 lhs = position2 - position;
			Vector3 rhs = position3 - position;
			float f = Vector3.Dot(lhs, rhs) / (lhs.magnitude * rhs.magnitude);
			float num3 = 180f - 57.29578f * Mathf.Acos(f);
			if (num3 < GetOptimizeAngle())
			{
				array2[i] = true;
				num++;
			}
			else
			{
				num2 = i;
			}
		}
		if (num == 0)
		{
			return array;
		}
		RageVertex[] array3 = new RageVertex[array.Length - num];
		int j = 0;
		for (int k = 0; k < array3.Length; k++)
		{
			for (; array2[j]; j++)
			{
			}
			array3[k] = array[j];
			j++;
		}
		optimizedVertexCount = array3.Length;
		return array3;
	}

	public int GetOutlineTriangleCount(RageVertex[] outlineVerts, bool AAoutline)
	{
		if (GetOutline() != 0)
		{
			if (AAoutline)
			{
				if (GetOutline() == Outline.Free)
				{
					return (outlineVerts.Length / 4 - 1) * 6 + 4;
				}
				return (outlineVerts.Length / 4 - 1) * 6;
			}
			return outlineVerts.Length - 2;
		}
		return 0;
	}

	public int GetFillTriangleCount(RageVertex[] fillVerts, bool AAfill)
	{
		switch (GetFill())
		{
		case Fill.None:
			return 0;
		case Fill.Solid:
		case Fill.Gradient:
			if (AAfill)
			{
				return fillVerts.Length / 2 - 2 + fillVerts.Length;
			}
			return fillVerts.Length - 2;
		case Fill.Landscape:
			if (AAfill)
			{
				return (fillVerts.Length / 3 - 1) * 4;
			}
			return (fillVerts.Length / 2 - 1) * 2;
		default:
			return 0;
		}
	}

	public int GetEmbossTriangleCount(RageVertex[] embossVerts, bool AAemboss)
	{
		if (GetEmboss() != 0)
		{
			if (AAemboss)
			{
				return (embossVerts.Length / 4 - 1) * 6;
			}
			return embossVerts.Length - 2;
		}
		return 0;
	}

	public void GenerateTriangles(Mesh mesh, RageVertex[] fillVerts, RageVertex[] embossVerts, RageVertex[] outlineVerts, bool AAfill, bool AAemboss, bool AAoutline, bool multipleMaterials)
	{
		Vector2[] array = new Vector2[0];
		if (GetFill() != Fill.Landscape)
		{
			array = ((!AAfill) ? new Vector2[fillVerts.Length] : new Vector2[fillVerts.Length / 2]);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Vector2(fillVerts[i].position.x, fillVerts[i].position.y);
			}
		}
		int[] array2 = new int[GetOutlineTriangleCount(outlineVerts, AAoutline) * 3 + GetFillTriangleCount(fillVerts, AAfill) * 3 + GetEmbossTriangleCount(embossVerts, AAemboss) * 3];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		switch (GetFill())
		{
		case Fill.Solid:
		case Fill.Gradient:
		{
			int[] array3 = Triangulator.Triangulate(array);
			for (int l = 0; l < array3.Length; l++)
			{
				array2[num++] = array3[l];
			}
			break;
		}
		case Fill.Landscape:
		{
			if (AAfill)
			{
				num3 = 2;
				num2 = fillVerts.Length / 3;
			}
			else
			{
				num3 = 1;
				num2 = fillVerts.Length / 2;
			}
			for (int j = 0; j < num2 - 1; j++)
			{
				for (int k = 0; k < num3; k++)
				{
					array2[num++] = j + k * num2;
					array2[num++] = j + (k + 1) * num2;
					array2[num++] = j + (k + 1) * num2 + 1;
					array2[num++] = j + k * num2;
					array2[num++] = j + (k + 1) * num2 + 1;
					array2[num++] = j + k * num2 + 1;
				}
			}
			break;
		}
		}
		int num4 = 0;
		if (AAfill)
		{
			num4 += array.Length * 6;
		}
		if (AAoutline && outlineVerts.Length != 0)
		{
			num4 += 2 * (outlineVerts.Length / 4 - 1) * 6;
		}
		if (GetOutline() == Outline.Free && AAoutline)
		{
			num4 += 12;
		}
		int[] array4 = new int[0];
		int num5 = 0;
		array4 = ((num4 <= 0) ? new int[0] : new int[num4]);
		if (AAfill)
		{
			num2 = array.Length - 1;
			if (PerspectiveMode)
			{
				for (int m = 0; m <= num2; m++)
				{
					for (int n = 0; n < 1; n++)
					{
						array4[num5++] = m + n * num2;
						array4[num5++] = m + (n + 1) * num2;
						array4[num5++] = m + (n + 1) * num2 + 1;
						array4[num5++] = m + n * num2;
						array4[num5++] = m + (n + 1) * num2 + 1;
						array4[num5++] = m + n * num2 + 1;
					}
				}
			}
			else
			{
				for (int num6 = 0; num6 <= num2; num6++)
				{
					for (int num7 = 0; num7 < 1; num7++)
					{
						array2[num++] = num6 + num7 * num2;
						array2[num++] = num6 + (num7 + 1) * num2;
						array2[num++] = num6 + (num7 + 1) * num2 + 1;
						array2[num++] = num6 + num7 * num2;
						array2[num++] = num6 + (num7 + 1) * num2 + 1;
						array2[num++] = num6 + num7 * num2 + 1;
					}
				}
			}
		}
		if (AAemboss)
		{
			num2 = embossVerts.Length / 4;
			num3 = 3;
		}
		else
		{
			num2 = embossVerts.Length / 2;
			num3 = 1;
		}
		for (int num8 = 0; num8 < num2 - 1; num8++)
		{
			for (int num9 = 0; num9 < num3; num9++)
			{
				if (num8 < num2 - 1)
				{
					array2[num++] = num8 + num9 * num2 + fillVerts.Length;
					array2[num++] = num8 + (num9 + 1) * num2 + 1 + fillVerts.Length;
					array2[num++] = num8 + (num9 + 1) * num2 + fillVerts.Length;
					array2[num++] = num8 + num9 * num2 + fillVerts.Length;
					array2[num++] = num8 + num9 * num2 + 1 + fillVerts.Length;
					array2[num++] = num8 + (num9 + 1) * num2 + 1 + fillVerts.Length;
				}
			}
		}
		if (AAoutline)
		{
			num2 = outlineVerts.Length / 4;
			num3 = 3;
		}
		else
		{
			num2 = outlineVerts.Length / 2;
			num3 = 1;
		}
		for (int num10 = 0; num10 < num2 - 1; num10++)
		{
			for (int num11 = 0; num11 < num3; num11++)
			{
				if (num11 == 1 || !PerspectiveMode || !AAoutline)
				{
					array2[num++] = num10 + num11 * num2 + embossVerts.Length + fillVerts.Length;
					array2[num++] = num10 + (num11 + 1) * num2 + 1 + embossVerts.Length + fillVerts.Length;
					array2[num++] = num10 + (num11 + 1) * num2 + embossVerts.Length + fillVerts.Length;
					array2[num++] = num10 + num11 * num2 + embossVerts.Length + fillVerts.Length;
					array2[num++] = num10 + num11 * num2 + 1 + embossVerts.Length + fillVerts.Length;
					array2[num++] = num10 + (num11 + 1) * num2 + 1 + embossVerts.Length + fillVerts.Length;
					continue;
				}
				if (array4.Length != 0)
				{
					array2[num++] = 0;
					array2[num++] = 0;
					array2[num++] = 0;
					array2[num++] = 0;
					array2[num++] = 0;
					array2[num++] = 0;
				}
				if (num2 > 0 && array4.Length != 0)
				{
					array4[num5++] = num10 + num11 * num2 + embossVerts.Length + fillVerts.Length;
					array4[num5++] = num10 + (num11 + 1) * num2 + 1 + embossVerts.Length + fillVerts.Length;
					array4[num5++] = num10 + (num11 + 1) * num2 + embossVerts.Length + fillVerts.Length;
					array4[num5++] = num10 + num11 * num2 + embossVerts.Length + fillVerts.Length;
					array4[num5++] = num10 + num11 * num2 + 1 + embossVerts.Length + fillVerts.Length;
					array4[num5++] = num10 + (num11 + 1) * num2 + 1 + embossVerts.Length + fillVerts.Length;
				}
			}
		}
		if (GetOutline() == Outline.Free && AAoutline)
		{
			int num12 = outlineVerts.Length / 4;
			array2[num++] = 0 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 2 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 + embossVerts.Length + fillVerts.Length;
			array2[num++] = 0 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 3 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 2 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 1 - 1 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 3 - 1 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 2 - 1 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 1 - 1 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 4 - 1 + embossVerts.Length + fillVerts.Length;
			array2[num++] = num12 * 3 - 1 + embossVerts.Length + fillVerts.Length;
		}
		if (!PerspectiveMode && multipleMaterials)
		{
			int num13 = 0;
			int[] array5 = new int[GetOutlineTriangleCount(outlineVerts, AAoutline) * 3];
			int[] array6 = new int[array2.Length - GetOutlineTriangleCount(outlineVerts, AAoutline) * 3];
			mesh.subMeshCount = 2;
			for (; num13 < array6.Length; num13++)
			{
				array6[num13] = array2[num13];
			}
			if (GetTexturing1() == UVMapping.Fill)
			{
				mesh.SetTriangles(array6, 0);
			}
			if (GetTexturing2() == UVMapping.Fill)
			{
				mesh.SetTriangles(array6, 1);
			}
			for (; num13 < array2.Length; num13++)
			{
				array5[num13 - array6.Length] = array2[num13];
			}
			if (GetTexturing1() == UVMapping.Outline)
			{
				mesh.SetTriangles(array5, 0);
			}
			if (GetTexturing2() == UVMapping.Outline)
			{
				mesh.SetTriangles(array5, 1);
			}
			return;
		}
		if (inverseTriangleDrawOrder)
		{
			int num14 = array2.Length;
			int[] array7 = new int[array2.Length];
			for (int num15 = 0; num15 < num14; num15 += 3)
			{
				array7[num14 - num15 - 3] = array2[num15];
				array7[num14 - num15 - 3 + 1] = array2[num15 + 1];
				array7[num14 - num15 - 3 + 2] = array2[num15 + 2];
			}
			array2 = array7;
		}
		mesh.triangles = array2;
		mesh.subMeshCount = 2;
		mesh.SetTriangles(array2, 0);
		mesh.SetTriangles(array4, 1);
	}

	public Color GetFillColor(Vector3 position)
	{
		switch (GetFill())
		{
		case Fill.Solid:
			return GetFillColor1();
		case Fill.Gradient:
		{
			Vector3 vector = GetGradientOffset();
			float value = (RotatePoint2D_CCW(position - vector, (0f - GetGradientAngleDeg()) / (180f / (float)Math.PI)) * GetGradientScaleInv() * 0.5f).y + 0.5f;
			return Mathf.Clamp(value, 0f, 1f) * GetFillColor1() + (1f - Mathf.Clamp(value, 0f, 1f)) * GetFillColor2();
		}
		default:
			return GetFillColor1();
		}
	}

	public Vector2 GetFillUV(Vector3 position)
	{
		Vector3 vector = GetTextureOffset();
		Vector2 vector2 = RotatePoint2D_CCW(position - vector, (0f - GetTextureAngleDeg()) / (180f / (float)Math.PI)) * GetTextureScaleInv();
		return vector2 + new Vector2(0.5f, 0.5f);
	}

	public Vector2 GetFillUV2(Vector3 position)
	{
		Vector3 vector = GetTextureOffset2();
		Vector2 vector2 = RotatePoint2D_CCW(position - vector, (0f - GetTextureAngle2Deg()) / (180f / (float)Math.PI)) * GetTextureScale2Inv();
		return vector2 + new Vector2(0.5f, 0.5f);
	}

	public Vector2 RotatePoint2D_CCW(Vector3 p, float angle)
	{
		return new Vector2(p.x * Mathf.Cos(0f - angle) - p.y * Mathf.Sin(0f - angle), p.y * Mathf.Cos(0f - angle) + p.x * Mathf.Sin(0f - angle));
	}

	public float Vector2Angle_CCW(Vector2 normal)
	{
		Vector3 vector = new Vector3(0f, 1f, 0f);
		if (normal.x < 0f)
		{
			return Mathf.Acos(vector.x * normal.x + vector.y * normal.y) * 57.29578f * -1f + 360f;
		}
		return (Mathf.Acos(vector.x * normal.x + vector.y * normal.y) * 57.29578f * -1f + 360f) * -1f + 360f;
	}

	public int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	private float mod(float x, float m)
	{
		return (x % m + m) % m;
	}

	public float SnapToGrid(float val, float gridsize)
	{
		if (mod(val, gridsize) < gridsize * 0.5f)
		{
			return val - mod(val, gridsize);
		}
		return val + (gridsize - mod(val, gridsize));
	}

	public Vector3 SnapToGrid(Vector3 val, float gridsize)
	{
		return new Vector3(SnapToGrid(val.x, gridsize), SnapToGrid(val.y, gridsize));
	}

	public void RefreshPhysics()
	{
		if (GetPhysics() == Physics.Boxed && BoxColliders == null)
		{
			BoxColliders = new BoxCollider[1];
		}
		if (!Application.isPlaying && !GetCreatePhysicsInEditor())
		{
			DestroyPhysicsChildren();
		}
		if (!Application.isPlaying && !GetCreatePhysicsInEditor())
		{
			return;
		}
		switch (GetPhysics())
		{
		case Physics.None:
			DestroyPhysicsChildren();
			break;
		case Physics.Boxed:
		{
			DestroyMeshCollider();
			DestroyPolygonCollider();
			RageVertex[] splits2 = GetSplits(GetPhysicsColliderCount(), 0f, 1f);
			int num4 = splits2.Length - 1;
			if (lastPhysicsVertsCount != num4 || BoxColliders == null || BoxColliders[0] == null)
			{
				DestroyPhysicsChildren();
				lastPhysicsVertsCount = num4;
				RageVertex[] splits3 = GetSplits(GetPhysicsColliderCount(), 0f, 1f);
				BoxColliders = new BoxCollider[splits3.Length - 1];
				int num5 = splits3.Length - 1;
				for (int m = 0; m < num5; m++)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "ZZZ_" + base.gameObject.name + "_BoxCollider";
					GameObject gameObject2 = gameObject;
					gameObject2.transform.parent = base.transform;
					BoxCollider boxCollider = gameObject2.AddComponent(typeof(BoxCollider)) as BoxCollider;
					boxCollider.material = GetPhysicsMaterial();
					int num6 = m + 1;
					Vector3 normal2 = GetNormal(splits3[m].splinePosition);
					Vector3 normal3 = GetNormal(splits3[num6].splinePosition);
					Vector3 vector = splits3[m].position - normal2 * (GetBoxColliderDepth() * 0.5f - GetPhysicsNormalOffset());
					Vector3 vector2 = splits3[num6].position - normal3 * (GetBoxColliderDepth() * 0.5f - GetPhysicsNormalOffset());
					gameObject2.layer = base.transform.gameObject.layer;
					gameObject2.tag = base.transform.gameObject.tag;
					gameObject2.gameObject.transform.localPosition = (vector + vector2) * 0.5f;
					gameObject2.gameObject.transform.LookAt(base.transform.TransformPoint(gameObject2.gameObject.transform.localPosition + Vector3.Cross((vector - vector2).normalized, new Vector3(0f, 0f, -1f))), new Vector3(1f, 0f, 0f));
					gameObject2.gameObject.transform.localScale = new Vector3(GetPhysicsZDepth(), (vector + normal2 * GetBoxColliderDepth() * 0.5f - (vector2 + normal3 * GetBoxColliderDepth() * 0.5f)).magnitude, 1f * GetBoxColliderDepth());
					BoxColliders[m] = boxCollider;
				}
				break;
			}
			PhysicMaterial material = GetPhysicsMaterial();
			int num7 = 0;
			RageVertex[] splits4 = GetSplits(GetPhysicsColliderCount(), 0f, 1f);
			float num8 = GetBoxColliderDepth();
			float physicsNormalOffset2 = GetPhysicsNormalOffset();
			float num9 = num8 * 0.5f - physicsNormalOffset2;
			BoxCollider[] boxColliders = BoxColliders;
			foreach (BoxCollider boxCollider2 in boxColliders)
			{
				if (!(boxCollider2 == null))
				{
					boxCollider2.material = material;
					boxCollider2.isTrigger = PhysicsIsTrigger;
					int num10 = num7 + 1;
					Vector3 normal4 = GetNormal(splits4[num7].splinePosition);
					Vector3 normal5 = GetNormal(splits4[num10].splinePosition);
					Vector3 vector3 = splits4[num7].position - normal4 * num9;
					Vector3 vector4 = splits4[num10].position - normal5 * num9;
					boxCollider2.gameObject.transform.localPosition = (vector3 + vector4) * 0.5f;
					boxCollider2.gameObject.transform.LookAt(base.transform.TransformPoint(boxCollider2.gameObject.transform.localPosition + Vector3.Cross((vector3 - vector4).normalized, new Vector3(0f, 0f, -1f))), new Vector3(1f, 0f, 0f));
					boxCollider2.gameObject.transform.localScale = new Vector3(GetPhysicsZDepth(), (vector3 + normal4 * num8 * 0.5f - (vector4 + normal5 * num8 * 0.5f)).magnitude, 1f * num8);
					num7++;
				}
			}
			lastPhysicsVertsCount = GetPhysicsColliderCount();
			break;
		}
		case Physics.Polygon:
		{
			DestroyBoxColliders();
			DestroyMeshCollider();
			if (GetPhysicsColliderCount() <= 2 || (!GetCreatePhysicsInEditor() && !Application.isPlaying))
			{
				break;
			}
			RageVertex[] splits = GetSplits(GetPhysicsColliderCount(), 0f, 1f);
			float physicsNormalOffset = GetPhysicsNormalOffset();
			PolygonCollider2D polygonCollider2D = base.gameObject.GetComponent<PolygonCollider2D>();
			if (polygonCollider2D == null)
			{
				polygonCollider2D = base.gameObject.AddComponent<PolygonCollider2D>();
			}
			if (GetFill() != Fill.Landscape)
			{
				Vector2[] array3 = new Vector2[splits.Length];
				for (int k = 0; k < array3.Length; k++)
				{
					array3[k] = splits[k].position + GetNormal(splits[k].splinePosition) * physicsNormalOffset;
				}
				polygonCollider2D.SetPath(0, array3);
			}
			else
			{
				Vector2[] array4 = new Vector2[splits.Length + 2];
				float y = GetBounds().yMin - Mathf.Clamp(GetLandscapeBottomDepth(), 1f, 100000000f);
				array4[0] = new Vector2(splits[0].position.x, y);
				for (int l = 1; l < array4.Length - 1; l++)
				{
					array4[l] = splits[l - 1].position + GetNormal(splits[l - 1].splinePosition) * physicsNormalOffset;
				}
				array4[array4.Length - 1] = new Vector2(splits[splits.Length - 1].position.x, y);
				polygonCollider2D.SetPath(0, array4);
			}
			polygonCollider2D.isTrigger = PhysicsIsTrigger;
			polygonCollider2D.sharedMaterial = physicsMaterial2D;
			break;
		}
		case Physics.MeshCollider:
		{
			DestroyBoxColliders();
			DestroyPolygonCollider();
			if (GetPhysicsColliderCount() <= 2 || (!GetCreatePhysicsInEditor() && !Application.isPlaying))
			{
				break;
			}
			int tt = 0;
			float physicsNormalOffset3 = GetPhysicsNormalOffset();
			float physicsZDepth = GetPhysicsZDepth();
			Vector3[] verts;
			int[] array5;
			if (GetFill() != Fill.Landscape)
			{
				RageVertex[] splits5 = GetSplits(GetPhysicsColliderCount(), 0f, 1f);
				verts = new Vector3[splits5.Length * 2];
				array5 = new int[verts.Length * 3 + (verts.Length - 2) * 6];
				for (int num11 = 0; num11 < verts.Length; num11 += 2)
				{
					verts[num11] = splits5[num11 / 2].position + new Vector3(0f, 0f, physicsZDepth * 0.5f) + GetNormal(splits5[num11 / 2].splinePosition) * physicsNormalOffset3;
					verts[num11 + 1] = splits5[num11 / 2].position + new Vector3(0f, 0f, physicsZDepth * -0.5f) + GetNormal(splits5[num11 / 2].splinePosition) * physicsNormalOffset3;
					tt = FeedMeshPhysicsTris(num11, ref verts, array5, tt);
				}
			}
			else
			{
				RageVertex[] splits5 = GetSplits(GetPhysicsColliderCount(), 0f, 1f);
				verts = new Vector3[splits5.Length * 2 + 4];
				array5 = new int[verts.Length * 3 + (verts.Length - 2) * 6];
				float y2 = GetBounds().yMin - Mathf.Clamp(GetLandscapeBottomDepth(), 1f, 100000000f);
				verts[0] = new Vector3(splits5[0].position.x, y2, physicsZDepth * 0.5f);
				verts[1] = new Vector3(splits5[0].position.x, y2, physicsZDepth * -0.5f);
				for (int num12 = 2; num12 < verts.Length - 2; num12 += 2)
				{
					verts[num12] = splits5[(num12 - 2) / 2].position + new Vector3(0f, 0f, physicsZDepth * 0.5f) + GetNormal(splits5[(num12 - 2) / 2].splinePosition) * physicsNormalOffset3;
					verts[num12 + 1] = splits5[(num12 - 2) / 2].position + new Vector3(0f, 0f, physicsZDepth * -0.5f) + GetNormal(splits5[(num12 - 2) / 2].splinePosition) * physicsNormalOffset3;
				}
				for (int num13 = 0; num13 < verts.Length; num13 += 2)
				{
					tt = FeedMeshPhysicsTris(num13, ref verts, array5, tt);
				}
				verts[verts.Length - 2] = new Vector3(splits5[splits5.Length - 1].position.x, y2, physicsZDepth * 0.5f);
				verts[verts.Length - 1] = new Vector3(splits5[splits5.Length - 1].position.x, y2, physicsZDepth * -0.5f);
			}
			Vector2[] array6 = new Vector2[verts.Length / 2];
			for (int num14 = 0; num14 < array6.Length; num14++)
			{
				array6[num14] = new Vector2(verts[num14 * 2].x, verts[num14 * 2].y);
			}
			Vector2[] array7 = new Vector2[verts.Length / 2];
			for (int num15 = 0; num15 < array7.Length; num15++)
			{
				array7[num15] = new Vector2(verts[num15 * 2 + 1].x, verts[num15 * 2 + 1].y);
			}
			int[] array8 = Triangulator.Triangulate(array6);
			for (int num16 = 0; num16 < array8.Length; num16++)
			{
				array5[tt++] = array8[num16] * 2;
			}
			int[] array9 = Triangulator.Triangulate(array7);
			for (int num17 = array9.Length - 1; num17 >= 0; num17--)
			{
				array5[tt++] = array9[num17] * 2 + 1;
			}
			MeshCollider meshCollider2 = base.gameObject.GetComponent(typeof(MeshCollider)) as MeshCollider;
			bool flag2 = meshCollider2 == null;
			Mesh mesh2 = ((!flag2) ? ((!(meshCollider2.sharedMesh == null)) ? meshCollider2.sharedMesh : new Mesh()) : new Mesh());
			mesh2.Clear();
			mesh2.vertices = verts;
			mesh2.triangles = array5;
			mesh2.RecalculateBounds();
			mesh2.RecalculateNormals();
			;
			if (flag2)
			{
				meshCollider2 = base.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
			}
			meshCollider2.sharedMesh = null;
			meshCollider2.sharedMesh = mesh2;
			meshCollider2.sharedMaterial = physicsMaterial;
			if (PhysicsIsTrigger)
			{
				meshCollider2.isTrigger = PhysicsIsTrigger;
			}
			meshCollider2.convex = GetCreateConvexMeshCollider();
			break;
		}
		case Physics.OutlineMeshCollider:
			DestroyBoxColliders();
			DestroyPolygonCollider();
			if (GetPhysicsColliderCount() > 2 && (GetCreatePhysicsInEditor() || Application.isPlaying))
			{
				int num = GetPhysicsColliderCount();
				Vector3[] array = new Vector3[(num + 1) * 4];
				int[] array2 = new int[num * 24];
				int num2 = 0;
				for (int i = 0; i <= num; i++)
				{
					float splinePosition = (float)i / (float)num;
					Vector3 normal = GetNormal(splinePosition);
					array[num2++] = GetPosition(splinePosition) + normal * GetOutlineWidth() * 0.5f + normal * GetOutlineNormalOffset() + new Vector3(0f, 0f, GetPhysicsZDepth() * 0.5f);
					array[num2++] = GetPosition(splinePosition) + normal * GetOutlineWidth() * -0.5f + normal * GetOutlineNormalOffset() + new Vector3(0f, 0f, GetPhysicsZDepth() * 0.5f);
					array[num2++] = GetPosition(splinePosition) + normal * GetOutlineWidth() * -0.5f + normal * GetOutlineNormalOffset() - new Vector3(0f, 0f, GetPhysicsZDepth() * 0.5f);
					array[num2++] = GetPosition(splinePosition) + normal * GetOutlineWidth() * 0.5f + normal * GetOutlineNormalOffset() - new Vector3(0f, 0f, GetPhysicsZDepth() * 0.5f);
				}
				int num3 = 0;
				for (int j = 0; j < num; j++)
				{
					array2[num3++] = j * 4;
					array2[num3++] = j * 4 + 4 + 1;
					array2[num3++] = j * 4 + 4;
					array2[num3++] = j * 4;
					array2[num3++] = j * 4 + 1;
					array2[num3++] = j * 4 + 4 + 1;
					array2[num3++] = j * 4 + 1;
					array2[num3++] = j * 4 + 1 + 4 + 1;
					array2[num3++] = j * 4 + 1 + 4;
					array2[num3++] = j * 4 + 1;
					array2[num3++] = j * 4 + 1 + 1;
					array2[num3++] = j * 4 + 1 + 4 + 1;
					array2[num3++] = j * 4 + 2;
					array2[num3++] = j * 4 + 2 + 4 + 1;
					array2[num3++] = j * 4 + 2 + 4;
					array2[num3++] = j * 4 + 2;
					array2[num3++] = j * 4 + 2 + 1;
					array2[num3++] = j * 4 + 2 + 4 + 1;
					array2[num3++] = j * 4 + 3;
					array2[num3++] = j * 4 + 3 + 1;
					array2[num3++] = j * 4 + 3 + 4;
					array2[num3++] = j * 4 + 3;
					array2[num3++] = j * 4;
					array2[num3++] = j * 4 + 3 + 1;
				}
				MeshCollider meshCollider = base.gameObject.GetComponent<MeshCollider>();
				bool flag = meshCollider == null;
				Mesh mesh = ((!flag) ? ((!(meshCollider.sharedMesh == null)) ? meshCollider.sharedMesh : new Mesh()) : new Mesh());
				mesh.Clear();
				mesh.vertices = array;
				mesh.triangles = array2;
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				;
				if (flag)
				{
					meshCollider = base.gameObject.AddComponent<MeshCollider>();
				}
				meshCollider.sharedMesh = null;
				meshCollider.sharedMesh = mesh;
				meshCollider.sharedMaterial = physicsMaterial;
				meshCollider.isTrigger = PhysicsIsTrigger;
				meshCollider.convex = GetCreateConvexMeshCollider();
			}
			break;
		}
	}

	private int FeedMeshPhysicsTris(int v, ref Vector3[] verts, int[] tris, int tt)
	{
		if (v < verts.Length - 2)
		{
			tris[tt] = v;
			tris[tt + 1] = v + 2;
			tris[tt + 2] = v + 1;
			tris[tt + 3] = v + 1;
			tris[tt + 4] = v + 2;
			tris[tt + 5] = v + 3;
		}
		else
		{
			tris[tt] = v;
			tris[tt + 1] = 0;
			tris[tt + 2] = v + 1;
			tris[tt + 3] = v + 1;
			tris[tt + 4] = 0;
			tris[tt + 5] = 1;
		}
		tt += 6;
		return tt;
	}

	public void ForceZeroZ()
	{
		spline.ForceZeroZ();
	}

	public void DestroyBoxColliders()
	{
		int num = 0;
		int num2 = base.transform.childCount + 1;
		while (base.transform.childCount > 0 && num < base.transform.childCount && num2 > 0)
		{
			num2--;
			if (base.transform.GetChild(num).GetComponent<BoxCollider>() != null)
			{
				if (base.transform.GetChild(num).name.Substring(0, 3).Equals("ZZZ"))
				{
					UnityEngine.Object.DestroyImmediate(base.transform.GetChild(num).gameObject);
				}
			}
			else
			{
				num++;
			}
		}
	}

	public void DestroyMeshCollider()
	{
		MeshCollider component = base.gameObject.GetComponent<MeshCollider>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component.sharedMesh);
			UnityEngine.Object.DestroyImmediate(component);
		}
	}

	public void DestroyPolygonCollider()
	{
		PolygonCollider2D component = base.gameObject.GetComponent<PolygonCollider2D>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
	}

	public void DestroyPhysicsChildren()
	{
		DestroyMeshCollider();
		DestroyBoxColliders();
		DestroyPolygonCollider();
	}

	public Vector3 ScaleToGlobal(Vector3 vec)
	{
		return new Vector3(vec.x * base.transform.lossyScale.x, vec.y * base.transform.lossyScale.y, vec.z * base.transform.lossyScale.z);
	}

	public Vector3 ScaleToLocal(Vector3 vec)
	{
		return (!WorldAaWidth) ? vec : new Vector3(vec.x * (1f / base.transform.lossyScale.x), vec.y * (1f / base.transform.lossyScale.y), vec.z * (1f / base.transform.lossyScale.z));
	}

	public int GetNearestPointIndex(float splinePosition)
	{
		return spline.GetNearestSplinePointIndex(splinePosition);
	}

	public int GetNearestPointIndex(Vector3 pos)
	{
		if (!Mathfx.Approximately(pos.z, 0f))
		{
			pos = new Vector3(pos.x, pos.y, 0f);
		}
		float sqrMagnitude = (pos - spline.points[0].point).sqrMagnitude;
		int result = 0;
		for (int i = 1; i < spline.points.Length; i++)
		{
			if ((pos - spline.points[i].point).sqrMagnitude < sqrMagnitude)
			{
				sqrMagnitude = (pos - spline.points[i].point).sqrMagnitude;
				result = i;
			}
		}
		return result;
	}

	public void CreateDefaultSpline()
	{
		Vector3[] array = new Vector3[2];
		Vector3[] array2 = new Vector3[4];
		float[] array3 = new float[2];
		bool[] array4 = new bool[2];
		array[0] = new Vector3(0f, (0f - Camera.main.orthographicSize) * 0.4f, 0f);
		array3[0] = 1f;
		array2[0] = new Vector3(Camera.main.orthographicSize * 0.3f, 0f, 0f);
		array2[1] = new Vector3(Camera.main.orthographicSize * -0.3f, 0f, 0f);
		array4[0] = true;
		array[1] = new Vector3(0f, Camera.main.orthographicSize * 0.4f, 0f);
		array3[1] = 1f;
		array2[2] = new Vector3(Camera.main.orthographicSize * -0.3f, 0f, 0f);
		array2[3] = new Vector3(Camera.main.orthographicSize * 0.3f, 0f, 0f);
		array4[1] = true;
		spline = new RageCurve(array, array2, array4, array3);
	}

	public bool pointsAreInClockWiseOrder()
	{
		float num = 0f;
		int pointCount = GetPointCount();
		if (pointCount < 3 || GetFill() == Fill.Landscape)
		{
			return true;
		}
		for (int i = 0; i < pointCount; i++)
		{
			Vector3 position = GetPosition(i);
			Vector3 vector = ((i + 1 <= pointCount - 1) ? GetPosition(i + 1) : GetPosition(0));
			num += position.x * vector.y - vector.x * position.y;
		}
		return num < 0f || GetFill() == Fill.Landscape || GetOutline() == Outline.Free;
	}

	public void flipPointOrder()
	{
		RageSplinePoint[] array = new RageSplinePoint[GetPointCount()];
		for (int i = 0; i < GetPointCount(); i++)
		{
			Vector3 inCtrl = spline.points[GetPointCount() - i - 1].inCtrl;
			Vector3 outCtrl = spline.points[GetPointCount() - i - 1].outCtrl;
			array[i] = spline.points[GetPointCount() - i - 1];
			array[i].inCtrl = outCtrl;
			array[i].outCtrl = inCtrl;
		}
		spline.points = array;
	}

	public void DrawSplineGizmos()
	{
		Vector3 position = GetPosition(0f);
		Gizmos.color = new Color(1f, 1f, 1f, 1f);
		for (int i = 1; i <= GetVertexCount(); i++)
		{
			Vector3 position2 = GetPosition(Mathf.Clamp01((float)i / (float)GetVertexCount()));
			Gizmos.DrawLine(base.transform.TransformPoint(position), base.transform.TransformPoint(position2));
			position = position2;
		}
		if (!hideHandles)
		{
			Gizmos.color = Color.red;
			for (int j = 0; j < GetPointCount(); j++)
			{
				Gizmos.DrawLine(GetPositionWorldSpace(j), GetInControlPositionWorldSpace(j));
				Gizmos.DrawLine(GetPositionWorldSpace(j), GetOutControlPositionWorldSpace(j));
			}
		}
	}

	public void DrawEmbossGizmos()
	{
		Vector3 p = new Vector3(0f, 1f, 0f);
		Vector3 middle = spline.GetMiddle(10);
		Vector3 vector = RotatePoint2D_CCW(p, GetEmbossAngleDeg() * ((float)Math.PI / 180f)) * (GetEmbossSize() * 4f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.TransformPoint(middle), base.transform.TransformPoint(middle + vector));
	}

	public void DrawGradientGizmos()
	{
		Vector3 vector = GetGradientOffset();
		Vector3 vector2 = RotatePoint2D_CCW(Vector3.up, GetGradientAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetGradientScaleInv());
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.TransformPoint(vector), base.transform.TransformPoint(vector + vector2));
		Gizmos.color = Color.green * new Color(1f, 1f, 1f, 0.2f);
		Vector3 vector3 = base.transform.TransformPoint(vector);
		for (int i = 4; i <= 360; i += 4)
		{
			Gizmos.DrawLine(vector3 + ScaleToGlobal(RotatePoint2D_CCW(Vector3.up, (float)(i - 4) * ((float)Math.PI / 180f)) * (1f / GetGradientScaleInv())), vector3 + ScaleToGlobal(RotatePoint2D_CCW(Vector3.up, (float)i * ((float)Math.PI / 180f)) * (1f / GetGradientScaleInv())));
		}
	}

	public void DrawTexturingGizmos()
	{
		Vector3 p = new Vector3(0f, 1f, 0f);
		Vector3 vector = GetTextureOffset();
		Vector3 vector2 = RotatePoint2D_CCW(p, GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv());
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(base.transform.TransformPoint(vector), base.transform.TransformPoint(vector + vector2));
		Vector2 vector3 = vector;
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, 0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, 0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())));
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, 0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, -0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())));
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, -0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, -0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())));
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, -0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, 0.5f, 0f), GetTextureAngleDeg() * ((float)Math.PI / 180f)) * (1f / GetTextureScaleInv())));
	}

	public void DrawTexturingGizmos2()
	{
		Vector3 p = new Vector3(0f, 1f, 0f);
		Vector3 vector = GetTextureOffset2();
		Vector3 vector2 = RotatePoint2D_CCW(p, GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv());
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(base.transform.TransformPoint(vector), base.transform.TransformPoint(vector + vector2));
		Vector2 vector3 = vector;
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, 0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, 0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())));
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, 0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, -0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())));
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(-0.5f, -0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, -0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())));
		Gizmos.DrawLine(base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, -0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())), base.transform.TransformPoint(vector3 + RotatePoint2D_CCW(new Vector3(0.5f, 0.5f, 0f), GetTextureAngle2Deg() * ((float)Math.PI / 180f)) * (1f / GetTextureScale2Inv())));
	}

	public void DrawGrid()
	{
		Rect bounds = GetBounds();
		Gizmos.color = gridColor;
		if (showCoordinates == ShowCoordinates.World)
		{
			Vector3 vector = base.transform.TransformPoint(new Vector3(bounds.xMin, bounds.yMin));
			Vector3 vector2 = base.transform.TransformPoint(new Vector3(bounds.xMax, bounds.yMax));
			float num = SnapToGrid(vector.x - gridSize * (float)gridExpansion, gridSize);
			float num2 = SnapToGrid(vector2.x + gridSize * (float)gridExpansion, gridSize);
			float num3 = SnapToGrid(vector.y - gridSize * (float)gridExpansion, gridSize);
			float num4 = SnapToGrid(vector2.y + gridSize * (float)gridExpansion, gridSize);
			if ((num2 - num) / gridSize < 500f && (num4 - num3) / gridSize < 500f)
			{
				for (float num5 = num; num5 < num2 + gridSize * 0.5f; num5 += gridSize)
				{
					Gizmos.DrawLine(new Vector2(num5, num4), new Vector2(num5, num3));
				}
				for (float num6 = num3; num6 < num4 + gridSize * 0.5f; num6 += gridSize)
				{
					Gizmos.DrawLine(new Vector2(num2, num6), new Vector2(num, num6));
				}
			}
			return;
		}
		float num7 = SnapToGrid(bounds.xMin, gridSize) - gridSize * (float)gridExpansion;
		float num8 = SnapToGrid(bounds.xMax, gridSize) + gridSize * (float)gridExpansion;
		float num9 = SnapToGrid(bounds.yMin, gridSize) - gridSize * (float)gridExpansion;
		float num10 = SnapToGrid(bounds.yMax, gridSize) + gridSize * (float)gridExpansion;
		if ((num8 - num7) / gridSize < 500f && (num10 - num9) / gridSize < 500f)
		{
			for (float num11 = num7; num11 < num8 + gridSize * 0.5f; num11 += gridSize)
			{
				Gizmos.DrawLine(base.transform.TransformPoint(new Vector2(num11, num10)), base.transform.TransformPoint(new Vector2(num11, num9)));
			}
			for (float num12 = num9; num12 < num10 + gridSize * 0.5f; num12 += gridSize)
			{
				Gizmos.DrawLine(base.transform.TransformPoint(new Vector2(num8, num12)), base.transform.TransformPoint(new Vector2(num7, num12)));
			}
		}
	}

	public bool IsSharpCorner(int index)
	{
		if (!GetNatural(index))
		{
			Vector3 position = GetPosition(index);
			float splinePosition = (float)index / (float)GetPointCount() - 1f / (float)GetVertexCount();
			float splinePosition2 = (float)index / (float)GetPointCount() + 1f / (float)GetVertexCount();
			Vector3 position2 = GetPosition(splinePosition);
			Vector3 position3 = GetPosition(splinePosition2);
			Vector3 lhs = position - position2;
			Vector3 rhs = position3 - position;
			if (Vector3.Cross(lhs, rhs).z < 0f)
			{
				return true;
			}
		}
		return false;
	}

	public float GetLastSplinePosition()
	{
		if (SplineIsOpenEnded())
		{
			return (float)(GetPointCount() - 1) / (float)GetPointCount();
		}
		return 1f;
	}

	public int GetLastIndex()
	{
		return GetPointCount() - 1;
	}

	public bool SplineIsOpenEnded()
	{
		return (GetOutline() == Outline.Free && GetFill() == Fill.None) || GetFill() == Fill.Landscape;
	}

	public Vector3 GetNormal(float splinePosition)
	{
		return spline.GetNormal(splinePosition * GetLastSplinePosition());
	}

	public Vector3 GetNormalInterpolated(float splinePosition)
	{
		return spline.GetNormalInterpolated(splinePosition * GetLastSplinePosition(), SplineIsOpenEnded());
	}

	public Vector3 GetNormal(int index)
	{
		return spline.GetNormal((float)index / (float)GetPointCount());
	}

	public void SetOutControlPosition(int index, Vector3 position)
	{
		spline.GetRageSplinePoint(index).outCtrl = position - spline.GetRageSplinePoint(index).point;
		if (spline.GetRageSplinePoint(index).natural)
		{
			spline.GetRageSplinePoint(index).inCtrl = spline.GetRageSplinePoint(index).outCtrl * -1f;
		}
	}

	public void SetOutControlPositionPointSpace(int index, Vector3 position)
	{
		spline.GetRageSplinePoint(index).outCtrl = position;
		if (spline.GetRageSplinePoint(index).natural)
		{
			spline.GetRageSplinePoint(index).inCtrl = spline.GetRageSplinePoint(index).outCtrl * -1f;
		}
	}

	public void SetOutControlPositionWorldSpace(int index, Vector3 position)
	{
		spline.GetRageSplinePoint(index).outCtrl = base.transform.InverseTransformPoint(position) - spline.GetRageSplinePoint(index).point;
		if (spline.GetRageSplinePoint(index).natural)
		{
			spline.GetRageSplinePoint(index).inCtrl = spline.GetRageSplinePoint(index).outCtrl * -1f;
		}
	}

	public void SetInControlPosition(int index, Vector3 position)
	{
		spline.GetRageSplinePoint(index).inCtrl = position - spline.GetRageSplinePoint(index).point;
		if (spline.GetRageSplinePoint(index).natural)
		{
			spline.GetRageSplinePoint(index).outCtrl = spline.GetRageSplinePoint(index).inCtrl * -1f;
		}
	}

	public void SetInControlPositionPointSpace(int index, Vector3 position)
	{
		spline.GetRageSplinePoint(index).inCtrl = position;
		if (spline.GetRageSplinePoint(index).natural)
		{
			spline.GetRageSplinePoint(index).outCtrl = spline.GetRageSplinePoint(index).inCtrl * -1f;
		}
	}

	public void SetInControlPositionWorldSpace(int index, Vector3 position)
	{
		spline.GetRageSplinePoint(index).inCtrl = base.transform.InverseTransformPoint(position) - spline.GetRageSplinePoint(index).point;
		if (spline.GetRageSplinePoint(index).natural)
		{
			spline.GetRageSplinePoint(index).outCtrl = spline.GetRageSplinePoint(index).inCtrl * -1f;
		}
	}

	public Vector3 GetOutControlPosition(int index)
	{
		return spline.GetRageSplinePoint(index).point + spline.GetRageSplinePoint(index).outCtrl;
	}

	public Vector3 GetInControlPosition(int index)
	{
		return spline.GetRageSplinePoint(index).point + spline.GetRageSplinePoint(index).inCtrl;
	}

	public Vector3 GetOutControlPositionPointSpace(int index)
	{
		return spline.GetRageSplinePoint(index).outCtrl;
	}

	public Vector3 GetInControlPositionPointSpace(int index)
	{
		return spline.GetRageSplinePoint(index).inCtrl;
	}

	public Vector3 GetOutControlPositionWorldSpace(int index)
	{
		return base.transform.TransformPoint(spline.GetRageSplinePoint(index).point + spline.GetRageSplinePoint(index).outCtrl);
	}

	public Vector3 GetInControlPositionWorldSpace(int index)
	{
		return base.transform.TransformPoint(spline.GetRageSplinePoint(index).point + spline.GetRageSplinePoint(index).inCtrl);
	}

	public Vector3 GetPosition(int index)
	{
		return spline.GetRageSplinePoint(index).point;
	}

	public int GetPointCount()
	{
		return spline.points.Length;
	}

	public Vector3 GetPositionWorldSpace(int index)
	{
		return base.transform.TransformPoint(spline.GetRageSplinePoint(index).point);
	}

	public Vector3 GetPosition(float splinePosition)
	{
		return spline.GetPoint(splinePosition * GetLastSplinePosition());
	}

	public Vector3 GetPositionWorldSpace(float splinePosition)
	{
		return base.transform.TransformPoint(spline.GetPoint(splinePosition * GetLastSplinePosition()));
	}

	public Vector3 GetMiddle()
	{
		return spline.GetMiddle(100);
	}

	public Rect GetBounds()
	{
		Vector3 max = spline.GetMax(100, 0f, GetLastSplinePosition());
		Vector3 min = spline.GetMin(100, 0f, GetLastSplinePosition());
		return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
	}

	public float GetLength()
	{
		return spline.GetLength(128, GetLastSplinePosition());
	}

	public float GetNearestSplinePosition(Vector3 target, int accuracy)
	{
		float num = 1E+11f;
		float result = 0f;
		for (int i = 0; i < accuracy; i++)
		{
			Vector3 point = spline.GetPoint((float)i / (float)accuracy * GetLastSplinePosition());
			float sqrMagnitude = (target - point).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = (float)i / (float)accuracy;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	public float GetNearestSplinePositionWorldSpace(Vector3 position, int accuracy)
	{
		return GetNearestSplinePosition(base.transform.InverseTransformPoint(position), accuracy);
	}

	public Vector3 GetNearestPosition(Vector3 position)
	{
		return spline.GetPoint(GetNearestSplinePosition(position, 100));
	}

	public Vector3 GetNearestPositionWorldSpace(Vector3 position)
	{
		return base.transform.TransformPoint(spline.GetPoint(spline.GetNearestSplinePoint(base.transform.InverseTransformPoint(position), 100)));
	}

	public void ClearPoints()
	{
		spline.ClearPoints();
	}

	public void AddPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
	{
		spline.AddRageSplinePoint(index, position, inCtrl, outCtrl, width, natural);
	}

	public void AddPoint(int index, Vector3 position, Vector3 outCtrl)
	{
		spline.AddRageSplinePoint(index, position, outCtrl * -1f, outCtrl, 1f, true);
	}

	public void AddPoint(int index, Vector3 position)
	{
		if (GetPointCount() >= 2)
		{
			spline.AddRageSplinePoint(index, position);
		}
		else
		{
			Debug.Log("ERROR: You can only call AddPoint(index, position), when there are 2 or more points in the RageSpline already");
		}
	}

	public int AddPoint(float splinePosition)
	{
		return spline.AddRageSplinePoint(splinePosition * GetLastSplinePosition());
	}

	public void AddPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
	{
		spline.AddRageSplinePoint(index, base.transform.InverseTransformPoint(position), inCtrl, outCtrl, width, natural);
	}

	public void AddPointWorldSpace(int index, Vector3 position, Vector3 outCtrl, float width)
	{
		spline.AddRageSplinePoint(index, base.transform.InverseTransformPoint(position), outCtrl * -1f, outCtrl, width, true);
	}

	public void AddPointWorldSpace(int index, Vector3 position, Vector3 outCtrl)
	{
		spline.AddRageSplinePoint(index, base.transform.InverseTransformPoint(position), outCtrl * -1f, outCtrl, 1f, true);
	}

	public void AddPointWorldSpace(int index, Vector3 position)
	{
		if (GetPointCount() >= 2)
		{
			spline.AddRageSplinePoint(index, base.transform.InverseTransformPoint(position));
		}
		else
		{
			Debug.Log("ERROR: You can only call AddPoint(index, position), when there are 2 or more points in the RageSpline already");
		}
	}

	public void SetPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
	{
		spline.points[index].point = position;
		spline.points[index].inCtrl = inCtrl;
		spline.points[index].outCtrl = outCtrl;
		spline.points[index].widthMultiplier = width;
		spline.points[index].natural = natural;
	}

	public void SetPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, bool natural)
	{
		spline.points[index].point = position;
		spline.points[index].inCtrl = inCtrl;
		spline.points[index].outCtrl = outCtrl;
		spline.points[index].natural = natural;
	}

	public void SetPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl)
	{
		spline.points[index].point = position;
		spline.points[index].inCtrl = inCtrl;
		spline.points[index].outCtrl = outCtrl;
	}

	public void SetPoint(int index, Vector3 position, Vector3 outCtrl)
	{
		spline.points[index].point = position;
		spline.points[index].inCtrl = outCtrl * -1f;
		spline.points[index].outCtrl = outCtrl;
		spline.points[index].natural = true;
	}

	public void SetPoint(int index, Vector3 position)
	{
		spline.points[index].point = position;
	}

	public void SetPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
	{
		spline.points[index].point = base.transform.InverseTransformPoint(position);
		spline.points[index].inCtrl = outCtrl * -1f;
		spline.points[index].outCtrl = outCtrl;
		spline.points[index].widthMultiplier = width;
		spline.points[index].natural = natural;
	}

	public void SetPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width)
	{
		spline.points[index].point = base.transform.InverseTransformPoint(position);
		spline.points[index].inCtrl = outCtrl * -1f;
		spline.points[index].outCtrl = outCtrl;
		spline.points[index].widthMultiplier = width;
	}

	public void SetPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl)
	{
		spline.points[index].point = base.transform.InverseTransformPoint(position);
		spline.points[index].inCtrl = outCtrl * -1f;
		spline.points[index].outCtrl = outCtrl;
	}

	public void SetPointWorldSpace(int index, Vector3 position, Vector3 outCtrl)
	{
		spline.points[index].point = base.transform.InverseTransformPoint(position);
		spline.points[index].inCtrl = outCtrl * -1f;
		spline.points[index].outCtrl = outCtrl;
		spline.points[index].natural = true;
	}

	public void SetPointWorldSpace(int index, Vector3 position)
	{
		spline.points[index].point = base.transform.InverseTransformPoint(position);
	}

	public void SmartRemovePoint(int index)
	{
		spline.SmartDelPoint(index);
	}

	public void RemovePoint(int index)
	{
		spline.DelPoint(index);
	}

	public bool GetNatural(int index)
	{
		return spline.GetRageSplinePoint(index).natural;
	}

	public void SetNatural(int index, bool natural)
	{
		spline.GetRageSplinePoint(index).natural = natural;
		if (natural)
		{
			spline.GetRageSplinePoint(index).outCtrl = spline.GetRageSplinePoint(index).inCtrl * -1f;
		}
	}

	public float GetOutlineWidth(float splinePosition)
	{
		return spline.GetWidth(splinePosition) * GetOutlineWidth();
	}

	public float GetAntialiasingWidth(float splinePosition)
	{
		return GetAntialiasingWidth();
	}

	public Vector3 FindNormal(Vector3 v1, Vector3 v2, Vector3 v3, float outlineWidth)
	{
		Vector3 vector = normalRotationQuat * (v1 - v2).normalized * outlineWidth;
		Vector3 vector2 = normalRotationQuat * (v2 - v3).normalized * outlineWidth;
		return Crossing(v1 + vector, v2 + vector, v2 + vector2, v3 + vector2) - v2;
	}

	public Vector3 Crossing(Vector3 p11, Vector3 p12, Vector3 p21, Vector3 p22)
	{
		float num = (p12.y - p11.y) * (p21.x - p22.x) - (p21.y - p22.y) * (p12.x - p11.x);
		float num2 = (p21.y - p11.y) * (p21.x - p22.x) - (p21.y - p22.y) * (p21.x - p11.x);
		if ((num > -0.001f && num < 0.001f) || (num2 > -0.001f && num2 < 0.001f))
		{
			return p12;
		}
		return new Vector3(p11.x + (p12.x - p11.x) * num2 / num, p11.y + (p12.y - p11.y) * num2 / num);
	}

	public int GetIndex(int index, int length)
	{
		if (index >= length)
		{
			return length - index + 1;
		}
		if (index < 0)
		{
			return length + index - 1;
		}
		return index;
	}

	public float GetOutlineWidth(int index)
	{
		return GetOutlineWidth((float)index / (float)GetPointCount());
	}

	public float GetOutlineWidthMultiplier(int index)
	{
		return spline.GetRageSplinePoint(index).widthMultiplier;
	}

	public void SetOutlineWidthMultiplier(int index, float width)
	{
		spline.GetRageSplinePoint(index).widthMultiplier = width;
	}

	public void SetOutline(Outline outline)
	{
		this.outline = outline;
		if (style != null)
		{
			style.SetOutline(outline, this);
		}
	}

	public Outline GetOutline()
	{
		if (style == null)
		{
			return outline;
		}
		return style.GetOutline();
	}

	public void SetOutlineColor1(Color color)
	{
		outlineColor1 = color;
		if (style != null)
		{
			style.SetOutlineColor1(color, this);
		}
	}

	public Color GetOutlineColor1()
	{
		if (style == null)
		{
			return outlineColor1;
		}
		return style.GetOutlineColor1();
	}

	public Color GetOutlineColor2()
	{
		if (style == null)
		{
			return outlineColor2;
		}
		return style.GetOutlineColor2();
	}

	public void SetOutlineColor2(Color color)
	{
		outlineColor2 = color;
		if (style != null)
		{
			style.SetOutlineColor2(color, this);
		}
	}

	public OutlineGradient GetOutlineGradient()
	{
		if (style == null)
		{
			return outlineGradient;
		}
		return style.GetOutlineGradient();
	}

	public void SetOutlineGradient(OutlineGradient outlineGradient)
	{
		this.outlineGradient = outlineGradient;
		if (style != null)
		{
			style.SetOutlineGradient(outlineGradient, this);
		}
	}

	public float GetOutlineNormalOffset()
	{
		if (style == null)
		{
			return outlineNormalOffset;
		}
		return style.GetOutlineNormalOffset();
	}

	public void SetOutlineNormalOffset(float outlineNormalOffset)
	{
		this.outlineNormalOffset = outlineNormalOffset;
		if (style != null)
		{
			style.SetOutlineNormalOffset(outlineNormalOffset, this);
		}
	}

	public void SetCorners(Corner corners)
	{
		this.corners = corners;
		if (style != null)
		{
			style.SetCorners(corners, this);
		}
	}

	public Corner GetCorners()
	{
		if (style == null)
		{
			return corners;
		}
		return style.GetCorners();
	}

	public void SetFill(Fill fill)
	{
		this.fill = fill;
		if (style != null)
		{
			style.SetFill(fill, this);
		}
	}

	public Fill GetFill()
	{
		if (style == null)
		{
			return fill;
		}
		return style.GetFill();
	}

	public void SetFillColor1(Color color)
	{
		fillColor1 = color;
		if (style != null)
		{
			style.SetFillColor1(color, this);
		}
	}

	public Color GetFillColor1()
	{
		if (style == null)
		{
			return fillColor1;
		}
		return style.GetFillColor1();
	}

	public void SetFillColor2(Color color)
	{
		fillColor2 = color;
		if (style != null)
		{
			style.SetFillColor2(color, this);
		}
	}

	public Color GetFillColor2()
	{
		if (style == null)
		{
			return fillColor2;
		}
		return style.GetFillColor2();
	}

	public void SetLandscapeBottomDepth(float landscapeBottomDepth)
	{
		this.landscapeBottomDepth = landscapeBottomDepth;
		if (style != null)
		{
			style.SetLandscapeBottomDepth(landscapeBottomDepth, this);
		}
	}

	public float GetLandscapeBottomDepth()
	{
		if (style == null)
		{
			return landscapeBottomDepth;
		}
		return style.GetLandscapeBottomDepth();
	}

	public void SetLandscapeOutlineAlign(float landscapeOutlineAlign)
	{
		landscapeOutlineAlign = Mathf.Clamp01(landscapeOutlineAlign);
		this.landscapeOutlineAlign = landscapeOutlineAlign;
		if (style != null)
		{
			style.SetLandscapeOutlineAlign(landscapeOutlineAlign, this);
		}
	}

	public float GetLandscapeOutlineAlign()
	{
		if (style == null)
		{
			return landscapeOutlineAlign;
		}
		return style.GetLandscapeOutlineAlign();
	}

	public void SetTexturing1(UVMapping texturing)
	{
		UVMapping1 = texturing;
		if (style != null)
		{
			style.SetTexturing1(texturing, this);
		}
	}

	public UVMapping GetTexturing1()
	{
		if (style == null)
		{
			return UVMapping1;
		}
		return style.GetTexturing1();
	}

	public void SetTexturing2(UVMapping texturing)
	{
		UVMapping2 = texturing;
		if (style != null)
		{
			style.SetTexturing2(texturing, this);
		}
	}

	public UVMapping GetTexturing2()
	{
		if (style == null)
		{
			return UVMapping2;
		}
		return style.GetTexturing2();
	}

	public void SetGradientOffset(Vector2 offset)
	{
		gradientOffset = offset;
		if (style != null && !styleLocalGradientPositioning)
		{
			style.SetGradientOffset(offset, this);
		}
	}

	public Vector2 GetGradientOffset()
	{
		if (style == null || styleLocalGradientPositioning)
		{
			return gradientOffset;
		}
		return style.GetGradientOffset();
	}

	public void SetGradientAngleDeg(float angle)
	{
		gradientAngle = Mathf.Clamp(angle, 0f, 360f);
		if (style != null && !styleLocalGradientPositioning)
		{
			style.SetGradientAngleDeg(angle, this);
		}
	}

	public float GetGradientAngleDeg()
	{
		if (style == null || styleLocalGradientPositioning)
		{
			return gradientAngle;
		}
		return style.GetGradientAngleDeg();
	}

	public void SetGradientScaleInv(float scale)
	{
		gradientScale = Mathf.Clamp(scale, 1E-05f, 100f);
		if (style != null && !styleLocalGradientPositioning)
		{
			style.SetGradientScaleInv(scale, this);
		}
	}

	public float GetGradientScaleInv()
	{
		if (style == null || styleLocalGradientPositioning)
		{
			return gradientScale;
		}
		return style.GetGradientScaleInv();
	}

	public void SetTextureOffset(Vector2 offset)
	{
		textureOffset = offset;
		if (style != null && !styleLocalTexturePositioning)
		{
			style.SetTextureOffset(offset, this);
		}
	}

	public Vector2 GetTextureOffset()
	{
		if (style == null || styleLocalTexturePositioning)
		{
			return textureOffset;
		}
		return style.GetTextureOffset();
	}

	public void SetTextureAngleDeg(float angle)
	{
		textureAngle = Mathf.Clamp(angle, 0f, 360f);
		if (style != null && !styleLocalTexturePositioning)
		{
			style.SetTextureAngleDeg(angle, this);
		}
	}

	public float GetTextureAngleDeg()
	{
		if (style == null || styleLocalTexturePositioning)
		{
			return textureAngle;
		}
		return style.GetTextureAngleDeg();
	}

	public void SetTextureScaleInv(float scale)
	{
		textureScale = Mathf.Clamp(scale, 1E-05f, 100f);
		if (style != null && !styleLocalTexturePositioning)
		{
			style.SetTextureScaleInv(scale, this);
		}
	}

	public float GetTextureScaleInv()
	{
		if (style == null || styleLocalTexturePositioning)
		{
			return textureScale;
		}
		return style.GetTextureScaleInv();
	}

	public void SetTextureOffset2(Vector2 offset)
	{
		textureOffset2 = offset;
		if (style != null && !styleLocalTexturePositioning)
		{
			style.SetTextureOffset2(offset, this);
		}
	}

	public Vector2 GetTextureOffset2()
	{
		if (style == null || styleLocalTexturePositioning)
		{
			return textureOffset2;
		}
		return style.GetTextureOffset2();
	}

	public void SetTextureAngle2Deg(float angle)
	{
		textureAngle2 = Mathf.Clamp(angle, 0f, 360f);
		if (style != null && !styleLocalTexturePositioning)
		{
			style.SetTextureAngle2Deg(angle, this);
		}
	}

	public float GetTextureAngle2Deg()
	{
		if (style == null || styleLocalTexturePositioning)
		{
			return textureAngle2;
		}
		return style.GetTextureAngle2Deg();
	}

	public void SetTextureScale2Inv(float scale)
	{
		textureScale2 = Mathf.Clamp(scale, 1E-05f, 100f);
		if (style != null && !styleLocalTexturePositioning)
		{
			style.SetTextureScale2Inv(scale, this);
		}
	}

	public float GetTextureScale2Inv()
	{
		if (style == null || styleLocalTexturePositioning)
		{
			return textureScale2;
		}
		return style.GetTextureScale2Inv();
	}

	public void SetEmboss(Emboss emboss)
	{
		this.emboss = emboss;
		if (style != null)
		{
			style.SetEmboss(emboss, this);
		}
	}

	public Emboss GetEmboss()
	{
		if (style == null)
		{
			return emboss;
		}
		return style.GetEmboss();
	}

	public void SetEmbossColor1(Color color)
	{
		embossColor1 = color;
		if (style != null)
		{
			style.SetEmbossColor1(color, this);
		}
	}

	public Color GetEmbossColor1()
	{
		if (style == null)
		{
			return embossColor1;
		}
		return style.GetEmbossColor1();
	}

	public void SetEmbossColor2(Color color)
	{
		embossColor2 = color;
		if (style != null)
		{
			style.SetEmbossColor2(color, this);
		}
	}

	public Color GetEmbossColor2()
	{
		if (style == null)
		{
			return embossColor2;
		}
		return style.GetEmbossColor2();
	}

	public void SetEmbossAngleDeg(float angle)
	{
		embossAngle = Mathf.Clamp(angle, 0f, 360f);
		if (style != null && !styleLocalEmbossPositioning)
		{
			style.SetEmbossAngle(angle, this);
		}
	}

	public float GetEmbossAngleDeg()
	{
		if (style == null || styleLocalEmbossPositioning)
		{
			return embossAngle;
		}
		return style.GetEmbossAngle();
	}

	public void SetEmbossOffset(float offset)
	{
		embossOffset = offset;
		if (style != null && !styleLocalEmbossPositioning)
		{
			style.SetEmbossOffset(offset, this);
		}
	}

	public float GetEmbossOffset()
	{
		if (style == null || styleLocalEmbossPositioning)
		{
			return embossOffset;
		}
		return style.GetEmbossOffset();
	}

	public void SetEmbossSize(float size)
	{
		embossSize = Mathf.Clamp(size, 0.00061f, 1000f);
		if (style != null && !styleLocalEmbossPositioning)
		{
			style.SetEmbossSize(size, this);
		}
	}

	public float GetEmbossSize()
	{
		if (style == null)
		{
			return embossSize;
		}
		return style.GetEmbossSize();
	}

	public void SetEmbossSmoothness(float smoothness)
	{
		embossCurveSmoothness = Mathf.Clamp(smoothness, 0f, 100f);
		if (style != null)
		{
			style.SetEmbossSmoothness(smoothness, this);
		}
	}

	public float GetEmbossSmoothness()
	{
		if (style == null)
		{
			return embossCurveSmoothness;
		}
		return style.GetEmbossSmoothness();
	}

	public void SetPhysics(Physics physicsValue)
	{
		physics = physicsValue;
		if (style != null)
		{
			style.SetPhysics(physicsValue, this);
		}
	}

	public Physics GetPhysics()
	{
		if (style == null)
		{
			return physics;
		}
		return style.GetPhysics();
	}

	public void SetCreatePhysicsInEditor(bool createInEditor)
	{
		createPhysicsInEditor = createInEditor;
		if (style != null)
		{
			style.SetCreatePhysicsInEditor(createInEditor, this);
		}
	}

	public bool GetCreatePhysicsInEditor()
	{
		if (style == null)
		{
			return createPhysicsInEditor;
		}
		return style.GetCreatePhysicsInEditor();
	}

	public void SetPhysicsMaterial(PhysicMaterial physicsMaterial)
	{
		this.physicsMaterial = physicsMaterial;
		if (style != null)
		{
			style.SetPhysicsMaterial(physicsMaterial, this);
		}
	}

	public PhysicMaterial GetPhysicsMaterial()
	{
		if (style == null)
		{
			return physicsMaterial;
		}
		return style.GetPhysicsMaterial();
	}

	public void SetVertexCount(int count)
	{
		if (lowQualityRender)
		{
			return;
		}
		int pointCount = GetPointCount();
		int num = mod(count, pointCount);
		int num2 = ((num == 0) ? count : ((num < pointCount / 2) ? (count - num) : (count + (pointCount - num))));
		if (num2 < pointCount)
		{
			num2 = pointCount;
		}
		vertexCount = num2;
		if (!optimize)
		{
			_vertexDensity = num2 / pointCount;
		}
		if (style != null && !styleLocalVertexCount)
		{
			if (count <= 0)
			{
				count = 1;
			}
			style.SetVertexCount(count, this);
		}
	}

	public int GetVertexCount()
	{
		if (style == null || lowQualityRender || styleLocalVertexCount)
		{
			if (lowQualityRender)
			{
				return 128;
			}
			if (vertexCount >= GetPointCount() || (outline == Outline.Free && fill == Fill.None))
			{
				return vertexCount;
			}
			return GetPointCount();
		}
		int num = mod(style.GetVertexCount(), GetPointCount());
		if (num == 0)
		{
			return style.GetVertexCount();
		}
		return style.GetVertexCount() + (GetPointCount() - num);
	}

	public void SetPhysicsColliderCount(int count)
	{
		physicsColliderCount = ((count < 1) ? 1 : count);
		if (style != null && !styleLocalPhysicsColliderCount)
		{
			style.SetPhysicsColliderCount(count, this);
		}
	}

	public int GetPhysicsColliderCount()
	{
		if (style == null || styleLocalPhysicsColliderCount)
		{
			if (LockPhysicsToAppearence)
			{
				return vertexCount;
			}
			return physicsColliderCount;
		}
		return style.GetPhysicsColliderCount();
	}

	public void SetCreateConvexMeshCollider(bool createConvexCollider)
	{
		createConvexMeshCollider = createConvexCollider;
		if (style != null)
		{
			style.SetCreateConvexMeshCollider(createConvexCollider, this);
		}
	}

	public bool GetCreateConvexMeshCollider()
	{
		if (style == null)
		{
			return createConvexMeshCollider;
		}
		return style.GetCreateConvexMeshCollider();
	}

	public void SetPhysicsZDepth(float depth)
	{
		colliderZDepth = Mathf.Clamp(depth, 0.0001f, 10000f);
		if (style != null)
		{
			style.SetPhysicsZDepth(depth, this);
		}
	}

	public float GetPhysicsZDepth()
	{
		if (style == null)
		{
			return colliderZDepth;
		}
		return style.GetPhysicsZDepth();
	}

	public void SetPhysicsNormalOffset(float offset)
	{
		colliderNormalOffset = Mathf.Clamp(offset, -1000f, 1000f);
		if (style != null)
		{
			style.SetPhysicsNormalOffset(offset, this);
		}
	}

	public float GetPhysicsNormalOffset()
	{
		if (style == null)
		{
			return colliderNormalOffset;
		}
		return style.GetPhysicsNormalOffset();
	}

	public void SetBoxColliderDepth(float depth)
	{
		boxColliderDepth = Mathf.Clamp(depth, -1000f, 1000f);
		if (style != null)
		{
			style.SetBoxColliderDepth(depth, this);
		}
	}

	public float GetBoxColliderDepth()
	{
		if (style == null)
		{
			return boxColliderDepth;
		}
		return style.GetBoxColliderDepth();
	}

	public void SetAntialiasingWidth(float width)
	{
		antiAliasingWidth = Mathf.Clamp(width, 0f, 1000f);
		if (style != null && !styleLocalAntialiasing)
		{
			style.SetAntialiasingWidth(width, this);
		}
	}

	public float GetAntialiasingWidth()
	{
		if (style == null || styleLocalAntialiasing)
		{
			return antiAliasingWidth;
		}
		return style.GetAntialiasingWidth();
	}

	public void SetOutlineWidth(float width)
	{
		OutlineWidth = Mathf.Clamp(width, 0.0001f, 1000f);
		if (style != null)
		{
			style.SetOutlineWidth(width, this);
		}
	}

	public float GetOutlineWidth()
	{
		if (style == null)
		{
			return OutlineWidth;
		}
		return style.GetOutlineWidth();
	}

	public void SetOutlineTexturingScaleInv(float scale)
	{
		outlineTexturingScale = Mathf.Clamp(scale, 0.001f, 1000f);
		if (style != null)
		{
			style.SetOutlineTexturingScaleInv(scale, this);
		}
	}

	public float GetOutlineTexturingScaleInv()
	{
		if (style == null)
		{
			return outlineTexturingScale;
		}
		return style.GetOutlineTexturingScaleInv();
	}

	public void SetOptimizeAngle(float angle)
	{
		optimizeAngle = angle;
		if (style != null)
		{
			style.SetOptimizeAngle(angle, this);
		}
	}

	public float GetOptimizeAngle()
	{
		if (style == null)
		{
			return optimizeAngle;
		}
		return style.GetOptimizeAngle();
	}

	public void SetOptimize(bool optimize)
	{
		this.optimize = optimize;
		if (style != null)
		{
			style.SetOptimize(optimize, this);
		}
	}

	public bool GetOptimize()
	{
		if (style == null)
		{
			return optimize;
		}
		return style.GetOptimize();
	}

	public void SetStyle(RageSplineStyle style)
	{
		this.style = style;
	}

	public RageSplineStyle GetStyle()
	{
		return style;
	}

	[ContextMenu("Refresh all RageSplines")]
	public void RefreshAllRageSplines()
	{
		RageSpline[] array = UnityEngine.Object.FindObjectsOfType(typeof(RageSpline)) as RageSpline[];
		RageSpline[] array2 = array;
		foreach (RageSpline rageSpline in array2)
		{
			rageSpline.RefreshMeshInEditor(true, true, true);
		}
	}

	[ContextMenu("All RageSplines to 3D")]
	public void AllRageSplinesTo3D()
	{
		RageSpline[] array = UnityEngine.Object.FindObjectsOfType(typeof(RageSpline)) as RageSpline[];
		RageSpline[] array2 = array;
		foreach (RageSpline rageSpline in array2)
		{
			rageSpline.PerspectiveMode = true;
			rageSpline.AssignDefaultMaterials();
		}
	}

	[ContextMenu("All RageSplines to 2D")]
	public void AllRageSplinesTo2D()
	{
		RageSpline[] array = UnityEngine.Object.FindObjectsOfType(typeof(RageSpline)) as RageSpline[];
		RageSpline[] array2 = array;
		foreach (RageSpline rageSpline in array2)
		{
			rageSpline.PerspectiveMode = false;
			rageSpline.AssignDefaultMaterials();
		}
	}

	public float GetTriangleCount()
	{
		return (float)Mfilter.sharedMesh.triangles.Length / 3f;
	}

	public static void CopyStyling(ref RageSpline refSpline, RageSpline target)
	{
		if (!(refSpline == null))
		{
			target.emboss = refSpline.emboss;
			target.embossAngle = refSpline.embossAngle;
			target.embossColor1 = refSpline.embossColor1;
			target.embossColor2 = refSpline.embossColor2;
			target.embossCurveSmoothness = refSpline.embossCurveSmoothness;
			target.embossOffset = refSpline.embossOffset;
			target.embossSize = refSpline.embossSize;
			target.SetFill(refSpline.fill);
			target.fillColor1 = refSpline.fillColor1;
			target.fillColor2 = refSpline.fillColor2;
			target.gradientAngle = refSpline.gradientAngle;
			target.gradientOffset = refSpline.gradientOffset;
			target.gradientScale = refSpline.gradientScale;
			target.outline = refSpline.outline;
			target.outlineColor1 = refSpline.outlineColor1;
			target.outlineColor2 = refSpline.outlineColor2;
			target.outlineTexturingScale = refSpline.outlineTexturingScale;
			target.OutlineWidth = refSpline.OutlineWidth;
			target.outlineGradient = refSpline.outlineGradient;
			target.outlineNormalOffset = refSpline.outlineNormalOffset;
			target.corners = refSpline.corners;
			target.textureAngle = refSpline.textureAngle;
			target.textureAngle2 = refSpline.textureAngle2;
			target.textureOffset = refSpline.textureOffset;
			target.textureOffset2 = refSpline.textureOffset2;
			target.textureScale = refSpline.textureScale;
			target.textureScale2 = refSpline.textureScale2;
			target.UVMapping1 = refSpline.UVMapping1;
			target.UVMapping2 = refSpline.UVMapping2;
		}
	}

	public void CopyPhysics(RageSpline refSpline)
	{
		physics = refSpline.physics;
		physicsColliderCount = refSpline.physicsColliderCount;
		physicsMaterial = refSpline.physicsMaterial;
		colliderZDepth = refSpline.colliderZDepth;
		createPhysicsInEditor = refSpline.createPhysicsInEditor;
	}

	public void CopyMaterial(RageSpline refSpline)
	{
		Material[] sharedMaterials = refSpline.Mrenderer.sharedMaterials;
		if (sharedMaterials.Length > 1)
		{
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Mrenderer.sharedMaterials[i] = refSpline.Mrenderer.sharedMaterials[i];
			}
		}
		else
		{
			Mrenderer.sharedMaterial = refSpline.Mrenderer.sharedMaterial;
		}
	}

	public static void CopyStylingAndMaterial(RageSpline refSpline, RageSpline target, bool copyAlpha)
	{
		if (!(refSpline == null))
		{
			float a = target.fillColor1.a;
			CopyStyling(ref refSpline, target);
			Vector2 vector = refSpline.transform.position - target.transform.position;
			target.gradientOffset = refSpline.gradientOffset + vector;
			target.textureOffset = refSpline.textureOffset + vector;
			target.textureOffset2 = refSpline.textureOffset2 + vector;
			if (!copyAlpha)
			{
				target.fillColor1.a = a;
			}
			target.CopyMaterial(refSpline);
		}
	}
}
