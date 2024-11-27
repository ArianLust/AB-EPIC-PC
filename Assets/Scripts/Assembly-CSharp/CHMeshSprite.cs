using SmoothMoves;
using UnityEngine;

[AddComponentMenu("Chimera/CHMeshSprite")]
public class CHMeshSprite : MonoBehaviour
{
	public TextureAtlas m_SmoothMovesAtlas;

	public UIAtlas m_NguiAtlas;

	public AtlasTypes m_AtlasType = AtlasTypes.Ngui;

	public string m_SpriteName;

	public int m_Width;

	public int m_Height;

	public Color m_Color = Color.white;

	public XAlignmentTypes m_XAlignment = XAlignmentTypes.Center;

	public YAlignmentTypes m_YAlignment = YAlignmentTypes.Center;

	private void Start()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if ((component == null || component.sharedMesh == null) && !string.IsNullOrEmpty(m_SpriteName))
		{
			UpdateSprite();
		}
	}

	public void UpdateSprite(bool forceSetMaterialFromAtlas = false, bool forceSetSizeFromSprite = false, Material replacementMaterial = null)
	{
		if (m_AtlasType == AtlasTypes.Ngui || m_AtlasType == AtlasTypes.SmoothMoves)
		{
			if ((m_AtlasType == AtlasTypes.Ngui && m_NguiAtlas == null) || (m_AtlasType == AtlasTypes.SmoothMoves && m_SmoothMovesAtlas == null) || string.IsNullOrEmpty(m_SpriteName))
			{
				return;
			}
			Rect rect;
			Rect value;
			if (m_AtlasType == AtlasTypes.Ngui)
			{
				UISpriteData sprite = m_NguiAtlas.GetSprite(m_SpriteName);
				if (sprite != null)
				{
					rect = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
					value = NGUIMath.ConvertToTexCoords(rect, m_NguiAtlas.texture.width, m_NguiAtlas.texture.width);
				}
				else
				{
					rect = default(Rect);
					value = default(Rect);
				}
			}
			else if (m_AtlasType == AtlasTypes.SmoothMoves)
			{
				if (replacementMaterial != null)
				{
					Material material = m_SmoothMovesAtlas.material;
					m_SmoothMovesAtlas.material = replacementMaterial;
				}
				int textureIndex = m_SmoothMovesAtlas.GetTextureIndex(m_SmoothMovesAtlas.GetTextureGUIDFromName(m_SpriteName));
				if (textureIndex != -1)
				{
					value = m_SmoothMovesAtlas.uvs[textureIndex];
					rect = new Rect(0f, 0f, m_SmoothMovesAtlas.textureSizes[textureIndex].x, m_SmoothMovesAtlas.textureSizes[textureIndex].y);
				}
				else
				{
					if (Application.isPlaying)
					{
						DebugLog.Warn("Sprite name " + m_SpriteName + " not found in atlas " + m_SmoothMovesAtlas.name);
					}
					value = default(Rect);
					rect = default(Rect);
				}
			}
			else
			{
				value = default(Rect);
				rect = default(Rect);
			}
			if (forceSetSizeFromSprite)
			{
				m_Height = (int)rect.height;
				m_Width = (int)rect.width;
			}
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			meshFilter.sharedMesh = QuadMeshCreator.BuildQuad(m_Width, m_Height, m_Color, value, m_XAlignment, m_YAlignment);
			Material material2 = null;
			if (m_AtlasType == AtlasTypes.Ngui)
			{
				material2 = m_NguiAtlas.spriteMaterial;
			}
			if (m_AtlasType == AtlasTypes.SmoothMoves)
			{
				material2 = m_SmoothMovesAtlas.material;
			}
			MeshRenderer component = GetComponent<MeshRenderer>();
			if (component == null)
			{
				component = base.gameObject.AddComponent<MeshRenderer>();
				component.material = material2;
			}
			else if (forceSetMaterialFromAtlas)
			{
				component.material = material2;
			}
		}
		else
		{
			MeshFilter meshFilter2 = GetComponent<MeshFilter>();
			if (meshFilter2 == null)
			{
				meshFilter2 = base.gameObject.AddComponent<MeshFilter>();
			}
			meshFilter2.sharedMesh = QuadMeshCreator.BuildQuad(m_Width, m_Height, m_Color, null, m_XAlignment, m_YAlignment);
		}
	}
}
