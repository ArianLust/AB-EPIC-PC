using UnityEngine;

public class CharacterAssetControllerSquire : MonoBehaviour
{
	public float PlaySquireAnimation(string animationName)
	{
		return base.gameObject.PlayAnimationOrAnimatorState(animationName);
	}
}
