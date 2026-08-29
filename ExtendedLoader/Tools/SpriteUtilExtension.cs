using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Color = UnityEngine.Color;

namespace ExtendedLoader
{
	public class SpriteUtilExtension
	{
		public static Sprite LoadCustomSizedPivotSprite(string filePath, Vector2 pivot, Vector2Int size, float resolution)
		{
			byte[] bytes;
			try
			{
				bytes = File.ReadAllBytes(filePath);
			}
			catch (FileNotFoundException message)
			{
				Debug.LogError(message);
				return null;
			}
			Vector2Int imageSize = TryGetImageNativeSize(bytes);
			if (size.x <= 0)
			{
				size.x = imageSize.x;
			}
			if (size.y <= 0)
			{
				size.y = imageSize.y;
			}
			if (size != imageSize)
			{
				bytes = ScaleTextureData(bytes, size.x, size.y);
			}
			Texture2D texture;
			if (SpriteCompressionConfig.Instance.disableCompression || !(texture = new Texture2D(8, 8, TextureFormat.DXT5, true)).LoadImage(bytes, true))
			{
				texture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
				texture.LoadImage(bytes, true);
			}
			return Sprite.Create(texture, new Rect(0f, 0f, size.x, size.y), pivot, resolution, 0U, SpriteMeshType.Tight);
		}

		static byte[] ScaleTextureData(byte[] sourceData, int targetWidth, int targetHeight)
		{
			Texture2D textureSource = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			textureSource.LoadImage(sourceData);
			textureSource.wrapMode = TextureWrapMode.Clamp;
			Texture2D textureTarget = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
			Color[] pixels = textureTarget.GetPixels();
			float hMult = 0.5f / targetWidth;
			float vMult = 0.5f / targetHeight;
			float hOffset = -0.5f / textureSource.width;
			float vOffset = -0.5f / textureSource.height;
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = textureSource.GetPixelBilinear(hMult * (2 * (i % targetWidth) + 1) + hOffset, vMult * (2 * (i / targetWidth) + 1) + vOffset);
			}

			UnityEngine.Object.Destroy(textureSource);
			textureTarget.SetPixels(pixels);
			return textureTarget.EncodeToPNG();
		}

		public static Sprite LoadSpriteCompressed(string filePath, Vector2 pivot)
		{
			Sprite result;
			try
			{
				if (!File.Exists(filePath))
				{
					result = null;
				}
				else
				{
					byte[] bytes = File.ReadAllBytes(filePath);
					Texture2D texture;
					if (SpriteCompressionConfig.Instance.disableCompression || !(texture = new Texture2D(8, 8, TextureFormat.DXT5, true)).LoadImage(bytes, true))
					{
						texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
						texture.LoadImage(bytes, true);
					}
					result = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot, 100f, 0U, SpriteMeshType.Tight);
				}
			}
			catch (FileNotFoundException message)
			{
				Debug.LogError(message);
				result = null;
			}
			return result;
		}

		internal static Vector2Int TryGetImageNativeSize(string path)
		{
			using (FileStream stream = File.OpenRead(path))
			{
				return TryGetImageNativeSize(stream);
			}
		}

		internal static Vector2Int TryGetImageNativeSize(byte[] bytes)
		{
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return TryGetImageNativeSize(stream);
			}
		}

		internal static Vector2Int TryGetImageNativeSize(Stream byteStream)
		{
			if (DimensionsHelper.TryGetDimensions(byteStream, out var size))
			{
				return size;
			}
			return new Vector2Int(512, 512);
		}
	}

	static class DimensionsHelper
	{
		private static readonly Dictionary<byte[], Func<Stream, Vector2Int>> imageFormatDecoders = new Dictionary<byte[], Func<Stream, Vector2Int>>()
		{
			{ new byte[]{ 0x42, 0x4D }, DecodeBitmap},
			{ new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, DecodeGif },
			{ new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, DecodeGif },
			{ new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, DecodePng },
			{ new byte[]{ 0xff, 0xd8 }, DecodeJfif },
		};

		public static bool TryGetDimensions(Stream byteStream, out Vector2Int dimensions)
		{
			int maxMagicBytesLength = imageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;

			byte[] magicBytes = new byte[maxMagicBytesLength];

			for (int i = 0; i < maxMagicBytesLength; i += 1)
			{
				int next = byteStream.ReadByte();
				if (next == -1)
				{
					dimensions = new Vector2Int(-1, -1);
					return false;
				}
				magicBytes[i] = (byte)next;
				foreach (var kvPair in imageFormatDecoders)
				{
					if (magicBytes.StartsWith(kvPair.Key))
					{
						var result = kvPair.Value(byteStream);
						if (result.x >= 0 && result.y >= 0)
						{
							dimensions = result;
							return true;
						}
						break;
					}
				}
			}
			dimensions = new Vector2Int(-1, -1);
			return false;
		}

		private static bool StartsWith(this byte[] thisBytes, byte[] thatBytes)
		{
			for (int i = 0; i < thatBytes.Length; i += 1)
			{
				if (thisBytes[i] != thatBytes[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool TryReadInt16(this Stream byteStream, out short value)
		{
			byte[] bytes = new byte[sizeof(short)];
			for (int i = 0; i < sizeof(short); i += 1)
			{
				int next = byteStream.ReadByte();
				if (next < 0)
				{
					value = 0;
					return false;
				}
				bytes[i] = (byte)next;
			}
			value = BitConverter.ToInt16(bytes, 0);
			return true;
		}

		private static bool TryReadLittleEndianInt16(this Stream byteStream, out short value)
		{
			byte[] bytes = new byte[sizeof(short)];
			for (int i = 0; i < sizeof(short); i += 1)
			{
				int next = byteStream.ReadByte();
				if (next < 0)
				{
					value = 0;
					return false;
				}
				bytes[sizeof(short) - 1 - i] = (byte)next;
			}
			value = BitConverter.ToInt16(bytes, 0);
			return true;
		}

		private static bool TryReadInt32(this Stream byteStream, out int value)
		{
			byte[] bytes = new byte[sizeof(int)];
			for (int i = 0; i < sizeof(int); i += 1)
			{
				int next = byteStream.ReadByte();
				if (next < 0)
				{
					value = 0;
					return false;
				}
				bytes[i] = (byte)next;
			}
			value = BitConverter.ToInt32(bytes, 0);
			return true;
		}

		private static bool TryReadLittleEndianInt32(this Stream byteStream, out int value)
		{
			byte[] bytes = new byte[sizeof(int)];
			for (int i = 0; i < sizeof(int); i += 1)
			{
				int next = byteStream.ReadByte();
				if (next < 0)
				{
					value = 0;
					return false;
				}
				bytes[sizeof(int) - 1 - i] = (byte)next;
			}
			value = BitConverter.ToInt32(bytes, 0);
			return true;
		}

		private static bool TrySeek(this Stream byteStream, int forward)
		{
			for (int i = 0; i < forward; i++)
			{
				if (byteStream.ReadByte() < 0)
				{
					return false;
				}
			}
			return true;
		}

		private static Vector2Int DecodeBitmap(Stream binaryReader)
		{
			if (binaryReader.TrySeek(16) && binaryReader.TryReadInt32(out var width) && binaryReader.TryReadInt32(out var height))
			{
				return new Vector2Int(width, height);
			}
			return new Vector2Int(-1, -1);
		}

		private static Vector2Int DecodeGif(Stream binaryReader)
		{
			if (binaryReader.TryReadInt16(out var width) && binaryReader.TryReadInt16(out var height))
			{
				return new Vector2Int(width, height);
			}
			return new Vector2Int(-1, -1);
		}

		private static Vector2Int DecodePng(Stream binaryReader)
		{
			if (binaryReader.TrySeek(8) && binaryReader.TryReadLittleEndianInt32(out var width) && binaryReader.TryReadLittleEndianInt32(out var height))
			{
				return new Vector2Int(width, height);
			}
			return new Vector2Int(-1, -1);
		}

		private static Vector2Int DecodeJfif(Stream binaryReader)
		{
			while (binaryReader.ReadByte() == 0xff)
			{
				int markerInt = binaryReader.ReadByte();
				if (markerInt < 0)
				{
					break;
				}
				byte marker = (byte)markerInt;
				if (!binaryReader.TryReadLittleEndianInt16(out var chunkLength))
				{
					break;
				}

				if (marker == 0xc0)
				{
					if (binaryReader.TrySeek(1) && binaryReader.TryReadLittleEndianInt16(out var height) && binaryReader.TryReadLittleEndianInt16(out var width))
					{
						return new Vector2Int(width, height);
					}
					break;
				}

				if (!binaryReader.TrySeek(chunkLength - 2))
				{
					break;
				}
			}

			return new Vector2Int(-1, -1);
		}
	}
}