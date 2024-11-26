using System;

[Serializable]
public class VisualEffect
{
	public VisualEffectSpawnTiming SpawnTiming;

	public float DelayInSeconds;

	public VisualEffectType Type;

	public VisualEffectTargetCombatant TargetCombatant;

	public VisualEffectTargetAnchor TargetAnchor;

	public string SoundId;

	public bool DoNotParentOnBone;

	public string PrefabName;

	public bool OnHitEffect;

	public string OnHitPrefix;

	public bool ShowEffectInvokerIcon;

	public float InterpolationTime;

	public float SlowMotionDuration;

	public float SlowMotionTimeScale;

	public bool UseTargetBoneScale;

	public bool UseTargetBoneRotation;

	public bool IgnoreScaling;

	public string EffectTextIdent;

	public bool HasText;

	public bool DoNotMirror;
}
