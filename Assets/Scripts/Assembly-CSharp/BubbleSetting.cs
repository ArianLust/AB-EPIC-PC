using System;

[Serializable]
public class BubbleSetting
{
	public string BalancingId;

	public BubbleType Type;

	public VisualEffectTargetCombatant TargetCombatant;

	public bool CharacterIcon;

	public string EffectTextIdent;

	public bool ShowDuration;

	public bool AtAll;

	public bool AtPigs;

	public bool HasText;

	public bool ReverseFill;
}
