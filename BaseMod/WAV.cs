using System.IO;

namespace BaseMod
{
	public class WAV
	{
		static float bytesToFloat(byte firstByte, byte secondByte)
		{
			short num = (short)(secondByte << 8 | firstByte);
			return num / 32768f;
		}
		static int bytesToInt(byte[] bytes, int offset = 0)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num |= bytes[offset + i] << i * 8;
			}
			return num;
		}
		static byte[] GetBytes(string filename)
		{
			return File.ReadAllBytes(filename);
		}
		public float[] LeftChannel
		{
			get; internal set;
		}
		public float[] RightChannel
		{
			get; internal set;
		}
		public int ChannelCount
		{
			get; internal set;
		}
		public int SampleCount
		{
			get; internal set;
		}
		public int Frequency
		{
			get; internal set;
		}
		public WAV(string filename) : this(GetBytes(filename))
		{
		}
		public WAV(byte[] wav)
		{
			ChannelCount = wav[22];
			Frequency = bytesToInt(wav, 24);
			int i = 12;
			while (wav[i] != 100 || wav[i + 1] != 97 || wav[i + 2] != 116 || wav[i + 3] != 97)
			{
				i += 4;
				int num = wav[i] + wav[i + 1] * 256 + wav[i + 2] * 65536 + wav[i + 3] * 16777216;
				i += 4 + num;
			}
			i += 8;
			SampleCount = (wav.Length - i) / 2;
			bool flag = ChannelCount == 2;
			if (flag)
			{
				SampleCount /= 2;
			}
			LeftChannel = new float[SampleCount];
			bool flag2 = ChannelCount == 2;
			if (flag2)
			{
				RightChannel = new float[SampleCount];
			}
			else
			{
				RightChannel = null;
			}
			int num2 = 0;
			while (i < wav.Length)
			{
				LeftChannel[num2] = bytesToFloat(wav[i], wav[i + 1]);
				i += 2;
				bool flag3 = ChannelCount == 2;
				if (flag3)
				{
					RightChannel[num2] = bytesToFloat(wav[i], wav[i + 1]);
					i += 2;
				}
				num2++;
			}
		}
		public override string ToString()
		{
			return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", new object[]
			{
				LeftChannel,
				RightChannel,
				ChannelCount,
				SampleCount,
				Frequency
			});
		}
	}
}
