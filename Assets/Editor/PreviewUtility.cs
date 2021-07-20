using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PreviewUtility {
	public static Texture2D GetTilePreview(TileBase tile) {
		if(tile == null) return Texture2D.whiteTexture;

		Sprite sprite = GetTileSprite(tile);
		if(sprite == null) return Texture2D.whiteTexture;
		
		Texture2D texture = AssetPreview.GetAssetPreview(sprite);
		if(texture != null) {
			texture.filterMode = FilterMode.Point;
			return texture;
		}

		return Texture2D.whiteTexture;
	}

	private static Sprite GetTileSprite(TileBase tile) {
		if(tile is Tile tile_tile) {
			return tile_tile.sprite;
		} else if(tile is RuleTile ruleTile_tile) {
			return ruleTile_tile.m_DefaultSprite;
		} else if(tile is AnimatedTile animatedTile_tile) {
			return animatedTile_tile.m_AnimatedSprites.Length > 0 ? animatedTile_tile.m_AnimatedSprites[0]:null;
		} else {
			Debug.Log("Unknown tile type: " + tile.GetType());
		}

		return null;
	}

	public static Texture2D GetSpritePreview(Sprite sprite) {
		if(sprite == null) return Texture2D.whiteTexture;
		
		Texture2D texture = AssetPreview.GetAssetPreview(sprite);
		if(texture != null) {
			texture.filterMode = FilterMode.Point;
			return texture;
		}

		return Texture2D.whiteTexture;
	}
}
