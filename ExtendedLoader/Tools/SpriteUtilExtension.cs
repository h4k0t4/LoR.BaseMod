using System.IO;
using UnityEngine;

namespace ExtendedLoader
{
	public class SpriteUtilExtension
	{
		public static Sprite LoadCustomSizedPivotSprite(string filePath, Vector2 pivot, Vector2Int size, float resolution)
		{
			byte[] data;
			try
			{
				data = File.ReadAllBytes(filePath);
			}
			catch (FileNotFoundException message)
			{
				Debug.LogError(message);
				return null;
			}
			Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
			texture.LoadImage(data);
			if (size.x <= 0)
			{
				size.x = texture.width;
			}
			if (size.y <= 0)
			{
				size.y = texture.height;
			}
			if (size.x != texture.width || size.y != texture.height)
			{
				texture = SpriteUtil.ScaleTexture(texture, size.x, size.y, true);
			}
			texture.Compress(true);
			texture.Apply(false, true);
			return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot, resolution, 0U, SpriteMeshType.Tight);
		}
	}
}