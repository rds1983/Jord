using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jord.Utils
{
	public class ColorBuffer
	{
		private readonly byte[] _data;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public byte[] Data
		{
			get { return _data; }
		}

		public Color this[int x, int y]
		{
			get
			{
				var pos = (y * Width + x) * 4;
				return new Color(_data[pos],
					_data[pos + 1],
					_data[pos + 2],
					_data[pos + 3]);
			}

			set
			{
				var pos = (y * Width + x) * 4;
				_data[pos] = value.R;
				_data[pos + 1] = value.G;
				_data[pos + 2] = value.B;
				_data[pos + 3] = value.A;
			}
		}

		public ColorBuffer(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}

			Width = width;
			Height = height;

			_data = new byte[width*height*4];
		}

		public ColorBuffer(int width, int height, byte[] data)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}

			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			var length = width*height*4;
			if (data.Length != length)
			{
				throw new ArgumentException(string.Format("Inconsistent data length: expected={0}, provided={1}", length, data.Length));
			}

			Width = width;
			Height = height;
			_data = data;
		}

		private static byte ApplyAlpha(byte color, byte alpha)
		{
			var fc = color / 255.0f;
			var fa = alpha / 255.0f;

			var fr = (int)(255.0f * fc * fa);

			if (fr < 0)
			{
				fr = 0;
			}

			if (fr > 255)
			{
				fr = 255;
			}

			return (byte)fr;
		}

		public void PremultiplyAlpha()
		{
			var data = Data;
			for (var i = 0; i < Width * Height; ++i)
			{
				var a = Data[i * 4 + 3];

				Data[i * 4] = ApplyAlpha(Data[i * 4], a);
				Data[i * 4 + 1] = ApplyAlpha(Data[i * 4 + 1], a);
				Data[i * 4 + 2] = ApplyAlpha(Data[i * 4 + 2], a);
			}
		}

		public Texture2D CreateTexture2D(GraphicsDevice device)
		{
			var texture = new Texture2D(device, Width, Height, false, SurfaceFormat.Color);

			texture.SetData(_data);
			return texture;
		}
	}
}
