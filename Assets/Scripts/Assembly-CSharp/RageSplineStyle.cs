using System;
using UnityEngine;

[Serializable]
public class RageSplineStyle : ScriptableObject
{
	public RageSpline.Outline outline = RageSpline.Outline.Loop;

	public Color outlineColor1 = Color.black;

	public Color outlineColor2 = Color.black;

	public RageSpline.OutlineGradient outlineGradient;

	public float outlineNormalOffset = 1f;

	public RageSpline.Corner corners;

	public RageSpline.Fill fill = RageSpline.Fill.Solid;

	public Color fillColor1 = Color.gray;

	public Color fillColor2 = Color.blue;

	public float landscapeBottomDepth;

	public float landscapeOutlineAlign;

	public RageSpline.UVMapping UVMapping1 = RageSpline.UVMapping.Fill;

	public RageSpline.UVMapping UVMapping2;

	public Vector2 gradientOffset = new Vector2(0f, 0f);

	public float gradientAngle;

	public float gradientScale = 10f;

	public Vector2 textureOffset = new Vector2(0f, 0f);

	public float textureAngle;

	public float textureScale = 10f;

	public Vector2 textureOffset2 = new Vector2(0f, 0f);

	public float textureAngle2;

	public float textureScale2 = 10f;

	public RageSpline.Emboss emboss;

	public Color embossColor1 = Color.white;

	public Color embossColor2 = Color.black;

	public float embossAngle = 180f;

	public float embossOffset = 0.5f;

	public float embossSize = 10f;

	public float embossCurveSmoothness = 3f;

	public RageSpline.Physics physics;

	public bool createPhysicsInEditor;

	public PhysicMaterial physicsMaterial;

	public int vertexCount = 64;

	public int physicsColliderCount = 32;

	public float colliderZDepth = 100f;

	public float colliderNormalOffset;

	public float boxColliderDepth = 1f;

	public bool createConvexMeshCollider;

	public float antiAliasingWidth = 0.5f;

	public float outlineWidth = 1f;

	public bool optimize;

	public float optimizeAngle = 5f;

	public float outlineTexturingScale = 0.1f;

	public void GetStyleFromRageSpline(RageSpline rageSpline)
	{
		antiAliasingWidth = rageSpline.antiAliasingWidth;
		colliderZDepth = rageSpline.colliderZDepth;
		createPhysicsInEditor = rageSpline.createPhysicsInEditor;
		emboss = rageSpline.emboss;
		embossAngle = rageSpline.embossAngle;
		embossColor1 = rageSpline.embossColor1;
		embossColor2 = rageSpline.embossColor2;
		embossCurveSmoothness = rageSpline.embossCurveSmoothness;
		embossOffset = rageSpline.embossOffset;
		embossSize = rageSpline.embossSize;
		fill = rageSpline.fill;
		fillColor1 = rageSpline.fillColor1;
		fillColor2 = rageSpline.fillColor2;
		gradientAngle = rageSpline.gradientAngle;
		gradientOffset = rageSpline.gradientOffset;
		gradientScale = rageSpline.gradientScale;
		outline = rageSpline.outline;
		outlineColor1 = rageSpline.outlineColor1;
		outlineColor2 = rageSpline.outlineColor2;
		outlineTexturingScale = rageSpline.outlineTexturingScale;
		outlineWidth = rageSpline.OutlineWidth;
		outlineGradient = rageSpline.outlineGradient;
		outlineNormalOffset = rageSpline.outlineNormalOffset;
		corners = rageSpline.corners;
		physics = rageSpline.physics;
		physicsColliderCount = rageSpline.physicsColliderCount;
		physicsMaterial = rageSpline.physicsMaterial;
		textureAngle = rageSpline.textureAngle;
		textureAngle2 = rageSpline.textureAngle2;
		textureOffset = rageSpline.textureOffset;
		textureOffset2 = rageSpline.textureOffset2;
		textureScale = rageSpline.textureScale;
		textureScale2 = rageSpline.textureScale2;
		UVMapping1 = rageSpline.UVMapping1;
		UVMapping2 = rageSpline.UVMapping2;
		vertexCount = rageSpline.vertexCount;
	}

	public void RefreshAllRageSplinesWithThisStyle(RageSpline caller)
	{
		RageSpline[] array = UnityEngine.Object.FindObjectsOfType(typeof(RageSpline)) as RageSpline[];
		RageSpline[] array2 = array;
		foreach (RageSpline rageSpline in array2)
		{
			if (rageSpline.style != null && rageSpline.style.Equals(this))
			{
				if (Application.isPlaying)
				{
					rageSpline.RefreshMesh();
				}
				else
				{
					rageSpline.RefreshMeshInEditor(true, true, true);
				}
			}
		}
	}

	public void SetOutline(RageSpline.Outline outline, RageSpline caller)
	{
		if (this.outline != outline)
		{
			this.outline = outline;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.Outline GetOutline()
	{
		return outline;
	}

	public void SetOutlineColor1(Color color, RageSpline caller)
	{
		if (outlineColor1 != color)
		{
			outlineColor1 = color;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Color GetOutlineColor1()
	{
		return outlineColor1;
	}

	public void SetOutlineColor2(Color color, RageSpline caller)
	{
		if (outlineColor2 != color)
		{
			outlineColor2 = color;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Color GetOutlineColor2()
	{
		return outlineColor2;
	}

	public RageSpline.OutlineGradient GetOutlineGradient()
	{
		return outlineGradient;
	}

	public void SetOutlineGradient(RageSpline.OutlineGradient outlineGradient, RageSpline caller)
	{
		if (this.outlineGradient != outlineGradient)
		{
			this.outlineGradient = outlineGradient;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetOutlineNormalOffset()
	{
		return outlineNormalOffset;
	}

	public void SetOutlineNormalOffset(float outlineNormalOffset, RageSpline caller)
	{
		if (this.outlineNormalOffset != outlineNormalOffset)
		{
			this.outlineNormalOffset = outlineNormalOffset;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public void SetCorners(RageSpline.Corner corners, RageSpline caller)
	{
		if (this.corners != corners)
		{
			this.corners = corners;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.Corner GetCorners()
	{
		return corners;
	}

	public void SetFill(RageSpline.Fill fill, RageSpline caller)
	{
		if (this.fill != fill)
		{
			this.fill = fill;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.Fill GetFill()
	{
		return fill;
	}

	public void SetFillColor1(Color color, RageSpline caller)
	{
		if (fillColor1 != color)
		{
			fillColor1 = color;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Color GetFillColor1()
	{
		return fillColor1;
	}

	public void SetFillColor2(Color color, RageSpline caller)
	{
		if (fillColor2 != color)
		{
			fillColor2 = color;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Color GetFillColor2()
	{
		return fillColor2;
	}

	public void SetLandscapeBottomDepth(float landscapeBottomDepth, RageSpline caller)
	{
		if (this.landscapeBottomDepth != landscapeBottomDepth)
		{
			this.landscapeBottomDepth = landscapeBottomDepth;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetLandscapeBottomDepth()
	{
		return landscapeBottomDepth;
	}

	public void SetLandscapeOutlineAlign(float landscapeOutlineAlign, RageSpline caller)
	{
		landscapeOutlineAlign = Mathf.Clamp01(landscapeOutlineAlign);
		if (this.landscapeOutlineAlign != landscapeOutlineAlign)
		{
			this.landscapeOutlineAlign = landscapeOutlineAlign;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetLandscapeOutlineAlign()
	{
		return landscapeOutlineAlign;
	}

	public void SetTexturing1(RageSpline.UVMapping texturing, RageSpline caller)
	{
		if (UVMapping1 != texturing)
		{
			UVMapping1 = texturing;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.UVMapping GetTexturing1()
	{
		return UVMapping1;
	}

	public void SetTexturing2(RageSpline.UVMapping texturing, RageSpline caller)
	{
		if (UVMapping2 != texturing)
		{
			UVMapping2 = texturing;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.UVMapping GetTexturing2()
	{
		return UVMapping2;
	}

	public void SetGradientOffset(Vector2 offset, RageSpline caller)
	{
		if (gradientOffset != offset)
		{
			gradientOffset = offset;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Vector2 GetGradientOffset()
	{
		return gradientOffset;
	}

	public void SetGradientAngleDeg(float angle, RageSpline caller)
	{
		if (gradientAngle != angle)
		{
			gradientAngle = Mathf.Clamp(angle, 0f, 360f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetGradientAngleDeg()
	{
		return gradientAngle;
	}

	public void SetGradientScaleInv(float scale, RageSpline caller)
	{
		if (gradientScale != scale)
		{
			gradientScale = Mathf.Clamp(scale, 1E-05f, 100f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetGradientScaleInv()
	{
		return gradientScale;
	}

	public void SetTextureOffset(Vector2 offset, RageSpline caller)
	{
		if (textureOffset != offset)
		{
			textureOffset = offset;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Vector2 GetTextureOffset()
	{
		return textureOffset;
	}

	public void SetTextureAngleDeg(float angle, RageSpline caller)
	{
		if (textureAngle != angle)
		{
			textureAngle = Mathf.Clamp(angle, 0f, 360f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetTextureAngleDeg()
	{
		return textureAngle;
	}

	public void SetTextureScaleInv(float scale, RageSpline caller)
	{
		if (textureScale != scale)
		{
			textureScale = Mathf.Clamp(scale, 1E-05f, 100f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetTextureScaleInv()
	{
		return textureScale;
	}

	public void SetTextureOffset2(Vector2 offset, RageSpline caller)
	{
		if (textureOffset2 != offset)
		{
			textureOffset2 = offset;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Vector2 GetTextureOffset2()
	{
		return textureOffset2;
	}

	public void SetTextureAngle2Deg(float angle, RageSpline caller)
	{
		if (textureAngle2 != angle)
		{
			textureAngle2 = Mathf.Clamp(angle, 0f, 360f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetTextureAngle2Deg()
	{
		return textureAngle2;
	}

	public void SetTextureScale2Inv(float scale, RageSpline caller)
	{
		if (textureScale2 != scale)
		{
			textureScale2 = Mathf.Clamp(scale, 1E-05f, 100f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetTextureScale2Inv()
	{
		return textureScale2;
	}

	public void SetEmboss(RageSpline.Emboss emboss, RageSpline caller)
	{
		if (this.emboss != emboss)
		{
			this.emboss = emboss;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.Emboss GetEmboss()
	{
		return emboss;
	}

	public void SetEmbossColor1(Color color, RageSpline caller)
	{
		if (embossColor1 != color)
		{
			embossColor1 = color;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Color GetEmbossColor1()
	{
		return embossColor1;
	}

	public void SetEmbossColor2(Color color, RageSpline caller)
	{
		if (embossColor2 != color)
		{
			embossColor2 = color;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public Color GetEmbossColor2()
	{
		return embossColor2;
	}

	public void SetEmbossAngle(float angle, RageSpline caller)
	{
		if (embossAngle != angle)
		{
			embossAngle = Mathf.Clamp(angle, 0f, 360f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetEmbossAngle()
	{
		return embossAngle;
	}

	public void SetEmbossOffset(float offset, RageSpline caller)
	{
		if (embossOffset != offset)
		{
			embossOffset = offset;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetEmbossOffset()
	{
		return embossOffset;
	}

	public void SetEmbossSize(float size, RageSpline caller)
	{
		if (embossSize != size)
		{
			embossSize = Mathf.Clamp(size, 0.00061f, 1000f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetEmbossSize()
	{
		return embossSize;
	}

	public void SetEmbossSmoothness(float smoothness, RageSpline caller)
	{
		if (embossCurveSmoothness != smoothness)
		{
			embossCurveSmoothness = Mathf.Clamp(smoothness, 0f, 100f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetEmbossSmoothness()
	{
		return embossCurveSmoothness;
	}

	public void SetPhysics(RageSpline.Physics physics, RageSpline caller)
	{
		if (this.physics != physics)
		{
			this.physics = physics;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public RageSpline.Physics GetPhysics()
	{
		return physics;
	}

	public void SetCreatePhysicsInEditor(bool createInEditor, RageSpline caller)
	{
		if (createPhysicsInEditor != createInEditor)
		{
			createPhysicsInEditor = createInEditor;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public bool GetCreatePhysicsInEditor()
	{
		return createPhysicsInEditor;
	}

	public void SetPhysicsMaterial(PhysicMaterial physicsMaterial, RageSpline caller)
	{
		if (this.physicsMaterial != physicsMaterial)
		{
			this.physicsMaterial = physicsMaterial;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public PhysicMaterial GetPhysicsMaterial()
	{
		return physicsMaterial;
	}

	public void SetVertexCount(int count, RageSpline caller)
	{
		if (vertexCount != count)
		{
			vertexCount = count;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public int GetVertexCount()
	{
		return vertexCount;
	}

	public void SetPhysicsColliderCount(int count, RageSpline caller)
	{
		if (physicsColliderCount != count)
		{
			physicsColliderCount = count;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public int GetPhysicsColliderCount()
	{
		return physicsColliderCount;
	}

	public void SetCreateConvexMeshCollider(bool createConvexMeshCollider, RageSpline caller)
	{
		if (this.createConvexMeshCollider != createConvexMeshCollider)
		{
			this.createConvexMeshCollider = createConvexMeshCollider;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public bool GetCreateConvexMeshCollider()
	{
		return createConvexMeshCollider;
	}

	public void SetPhysicsZDepth(float depth, RageSpline caller)
	{
		if (colliderZDepth != depth)
		{
			colliderZDepth = depth;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetPhysicsZDepth()
	{
		return colliderZDepth;
	}

	public void SetPhysicsNormalOffset(float offset, RageSpline caller)
	{
		colliderNormalOffset = offset;
	}

	public float GetPhysicsNormalOffset()
	{
		return colliderNormalOffset;
	}

	public void SetBoxColliderDepth(float depth, RageSpline caller)
	{
		if (boxColliderDepth != depth)
		{
			boxColliderDepth = Mathf.Clamp(depth, 0.1f, 1000f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetBoxColliderDepth()
	{
		return boxColliderDepth;
	}

	public void SetAntialiasingWidth(float width, RageSpline caller)
	{
		if (antiAliasingWidth != width)
		{
			antiAliasingWidth = Mathf.Clamp(width, 0f, 1000f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetAntialiasingWidth()
	{
		return antiAliasingWidth;
	}

	public void SetOutlineWidth(float width, RageSpline caller)
	{
		if (outlineWidth != width)
		{
			outlineWidth = Mathf.Clamp(width, 0.0001f, 1000f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetOutlineWidth()
	{
		return outlineWidth;
	}

	public void SetOutlineTexturingScaleInv(float scale, RageSpline caller)
	{
		if (outlineTexturingScale != scale)
		{
			outlineTexturingScale = Mathf.Clamp(scale, 0.001f, 1000f);
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetOutlineTexturingScaleInv()
	{
		return outlineTexturingScale;
	}

	public void SetOptimizeAngle(float angle, RageSpline caller)
	{
		if (optimizeAngle != angle)
		{
			optimizeAngle = angle;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public float GetOptimizeAngle()
	{
		return optimizeAngle;
	}

	public void SetOptimize(bool optimize, RageSpline caller)
	{
		if (this.optimize != optimize)
		{
			this.optimize = optimize;
			RefreshAllRageSplinesWithThisStyle(caller);
		}
	}

	public bool GetOptimize()
	{
		return optimize;
	}
}
