using UnityEngine;

public interface IRageSpline
{
	Vector3 GetNormal(float splinePosition);

	Vector3 GetNormalInterpolated(float splinePosition);

	Vector3 GetNormal(int index);

	void SetOutControlPosition(int index, Vector3 position);

	void SetOutControlPositionPointSpace(int index, Vector3 position);

	void SetOutControlPositionWorldSpace(int index, Vector3 position);

	void SetInControlPosition(int index, Vector3 position);

	void SetInControlPositionPointSpace(int index, Vector3 position);

	void SetInControlPositionWorldSpace(int index, Vector3 position);

	Vector3 GetOutControlPosition(int index);

	Vector3 GetInControlPosition(int index);

	Vector3 GetOutControlPositionPointSpace(int index);

	Vector3 GetInControlPositionPointSpace(int index);

	Vector3 GetOutControlPositionWorldSpace(int index);

	Vector3 GetInControlPositionWorldSpace(int index);

	Vector3 GetPosition(int index);

	int GetPointCount();

	Vector3 GetPositionWorldSpace(int index);

	Vector3 GetPosition(float splinePosition);

	Vector3 GetPositionWorldSpace(float splinePosition);

	Vector3 GetMiddle();

	Rect GetBounds();

	float GetLength();

	float GetNearestSplinePosition(Vector3 target, int accuracy);

	float GetNearestSplinePositionWorldSpace(Vector3 position, int accuracy);

	Vector3 GetNearestPositionWorldSpace(Vector3 position);

	int GetNearestPointIndex(float splinePosition);

	int GetNearestPointIndex(Vector3 position);

	Vector3 GetNearestPosition(Vector3 position);

	void ClearPoints();

	void RemovePoint(int index);

	void AddPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural);

	void AddPoint(int index, Vector3 position, Vector3 outCtrl);

	void AddPoint(int index, Vector3 position);

	int AddPoint(float splinePosition);

	void AddPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural);

	void AddPointWorldSpace(int index, Vector3 position, Vector3 outCtrl, float width);

	void AddPointWorldSpace(int index, Vector3 position, Vector3 outCtrl);

	void AddPointWorldSpace(int index, Vector3 position);

	void SetPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural);

	void SetPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, bool natural);

	void SetPoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl);

	void SetPoint(int index, Vector3 position, Vector3 outCtrl);

	void SetPoint(int index, Vector3 position);

	void SetPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural);

	void SetPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width);

	void SetPointWorldSpace(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl);

	void SetPointWorldSpace(int index, Vector3 position, Vector3 outCtrl);

	void SetPointWorldSpace(int index, Vector3 position);

	bool GetNatural(int index);

	void SetNatural(int index, bool natural);

	float GetOutlineWidth(float splinePosition);

	float GetOutlineWidth(int index);

	float GetOutlineWidthMultiplier(int index);

	void SetOutlineWidthMultiplier(int index, float width);

	void RefreshMesh();

	void RefreshMesh(bool refreshFillTriangulation, bool refreshNormals, bool refreshPhysics);

	void SetOutline(RageSpline.Outline outline);

	RageSpline.Outline GetOutline();

	void SetOutlineColor1(Color color);

	Color GetOutlineColor1();

	void SetOutlineColor2(Color color);

	Color GetOutlineColor2();

	RageSpline.OutlineGradient GetOutlineGradient();

	void SetOutlineGradient(RageSpline.OutlineGradient outlineGradient);

	float GetOutlineNormalOffset();

	void SetOutlineNormalOffset(float outlineNormalOffset);

	RageSpline.Corner GetCorners();

	void SetCorners(RageSpline.Corner corners);

	void SetFill(RageSpline.Fill fill);

	RageSpline.Fill GetFill();

	void SetFillColor1(Color color);

	Color GetFillColor1();

	void SetFillColor2(Color color);

	Color GetFillColor2();

	void SetLandscapeBottomDepth(float landscapeBottomDepth);

	float GetLandscapeBottomDepth();

	void SetLandscapeOutlineAlign(float landscapeOutlineAlign);

	float GetLandscapeOutlineAlign();

	void SetTexturing1(RageSpline.UVMapping texturing);

	RageSpline.UVMapping GetTexturing1();

	void SetTexturing2(RageSpline.UVMapping texturing);

	RageSpline.UVMapping GetTexturing2();

	void SetGradientOffset(Vector2 offset);

	Vector2 GetGradientOffset();

	void SetGradientAngleDeg(float angle);

	float GetGradientAngleDeg();

	void SetGradientScaleInv(float scale);

	float GetGradientScaleInv();

	void SetTextureOffset(Vector2 offset);

	Vector2 GetTextureOffset();

	void SetTextureAngleDeg(float angle);

	float GetTextureAngleDeg();

	void SetTextureScaleInv(float scale);

	float GetTextureScaleInv();

	void SetTextureOffset2(Vector2 offset);

	Vector2 GetTextureOffset2();

	void SetTextureAngle2Deg(float angle);

	float GetTextureAngle2Deg();

	void SetTextureScale2Inv(float scale);

	float GetTextureScale2Inv();

	void SetEmboss(RageSpline.Emboss emboss);

	RageSpline.Emboss GetEmboss();

	void SetEmbossColor1(Color color);

	Color GetEmbossColor1();

	void SetEmbossColor2(Color color);

	Color GetEmbossColor2();

	void SetEmbossAngleDeg(float angle);

	float GetEmbossAngleDeg();

	void SetEmbossOffset(float offset);

	float GetEmbossOffset();

	void SetEmbossSize(float size);

	float GetEmbossSize();

	void SetEmbossSmoothness(float smoothness);

	float GetEmbossSmoothness();

	void SetPhysics(RageSpline.Physics physicsValue);

	RageSpline.Physics GetPhysics();

	void SetCreatePhysicsInEditor(bool createInEditor);

	bool GetCreatePhysicsInEditor();

	void SetPhysicsMaterial(PhysicMaterial physicsMaterial);

	PhysicMaterial GetPhysicsMaterial();

	void SetVertexCount(int count);

	int GetVertexCount();

	void SetPhysicsColliderCount(int count);

	int GetPhysicsColliderCount();

	void SetCreateConvexMeshCollider(bool createConvexCollider);

	bool GetCreateConvexMeshCollider();

	void SetPhysicsZDepth(float depth);

	float GetPhysicsZDepth();

	void SetPhysicsNormalOffset(float offset);

	float GetPhysicsNormalOffset();

	void SetBoxColliderDepth(float depth);

	float GetBoxColliderDepth();

	void SetAntialiasingWidth(float width);

	float GetAntialiasingWidth();

	void SetOutlineWidth(float width);

	float GetOutlineWidth();

	void SetOutlineTexturingScaleInv(float scale);

	float GetOutlineTexturingScaleInv();

	void SetOptimizeAngle(float angle);

	float GetOptimizeAngle();

	void SetOptimize(bool optimize);

	bool GetOptimize();

	void SetStyle(RageSplineStyle style);

	RageSplineStyle GetStyle();
}
