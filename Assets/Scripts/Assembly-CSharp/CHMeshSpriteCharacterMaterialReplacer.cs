using UnityEngine;

[RequireComponent(typeof(CHMeshSprite))]
public class CHMeshSpriteCharacterMaterialReplacer : MonoBehaviour
{
	private void Start()
	{
		CHMeshSprite component = GetComponent<CHMeshSprite>();
		if ((!(component != null) || (component.m_AtlasType == AtlasTypes.SmoothMoves && !(component.m_SmoothMovesAtlas == null))) && component != null)
		{
			component.UpdateSprite(true);
		}
	}
}
