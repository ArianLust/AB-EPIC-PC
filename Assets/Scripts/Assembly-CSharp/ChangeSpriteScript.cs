using SmoothMoves;
using UnityEngine;

[RequireComponent(typeof(BoneAnimation))]
public class ChangeSpriteScript : MonoBehaviour
{
	private BoneAnimation BoneAnimation;

	public Transform Bone;

	public TextureSearchReplace Replace;

	public bool switchSprite;

	private bool switchedSprite;

	public void Awake()
	{
		BoneAnimation = GetComponent<BoneAnimation>();
	}

	private void Update()
	{
		if (switchSprite && !switchedSprite)
		{
			switchedSprite = true;
			BoneAnimation.ReplaceBoneTexture(Bone.gameObject.name, Replace);
		}
		else if (!switchSprite && switchedSprite)
		{
			switchedSprite = false;
			BoneAnimation.RestoreBoneTexture(Bone.gameObject.name);
		}
	}
}
