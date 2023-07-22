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
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, true);
			texture2D.LoadImage(data);
			if (size.x <= 0)
			{
				size.x = texture2D.width;
			}
			if (size.y <= 0)
			{
				size.y = texture2D.height;
			}
			Texture2D texture2D2 = SpriteUtil.ScaleTexture(texture2D, size.x, size.y, true);
			return Sprite.Create(texture2D2, new Rect(0f, 0f, texture2D2.width, texture2D2.height), pivot, resolution, 0U, SpriteMeshType.Tight);
		}
	}
}