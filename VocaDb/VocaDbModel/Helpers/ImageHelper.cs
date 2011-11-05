using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Drawing;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Helpers {

	public static class ImageHelper {

		private static readonly string[] allowedExt = new[] { ".bmp", ".gif", ".jpg", ".jpeg", ".png" };

		public static string[] AllowedExtensions {
			get { return allowedExt; }
		}

		public static PictureThumbContract[] GenerateThumbs(Stream input, int[] sizes) {

			var thumbs = new List<PictureThumbContract>(sizes.Length);

			using (var original = Image.FromStream(input)) {

				foreach (var size in sizes) {

					if (size < original.Size.Width || size < original.Size.Height) {

						using (var scaled = ResizeToFixedSize(original, size, size))
						using (var memStream = new MemoryStream()) {

							//scaled.Save("C:\\Temp\\out", original.RawFormat);
							scaled.Save(memStream, original.RawFormat);
							var thumbBuf = StreamHelper.ReadStream(memStream, memStream.Length);
							thumbs.Add(new PictureThumbContract(thumbBuf, size));

						}

					}


				}
			}

			return thumbs.ToArray();

		}

		public static string GetExtensionFromMime(string mime) {

			switch (mime) {
				case "image/jpeg":
					return ".jpg";
				case "image/png":
					return ".png";
				case "image/gif":
					return ".gif";
				case "image/bmp":
					return ".bmp";
				default:
					return null;
			}

		}

		public static PictureDataContract GetOriginalAndResizedImages(Stream input, int length, string contentType) {

			var buf = new Byte[length];
			input.Read(buf, 0, length);

			var contract = new PictureDataContract(buf, contentType);
			var thumbs = GenerateThumbs(input, new[] { 250 });
			var thumb250 = thumbs.FirstOrDefault(t => t.Size == 250);

			contract.Thumb250 = thumb250;

			return contract;

		}

		public static bool IsValidImageExtension(string fileName) {

			var ext = Path.GetExtension(fileName);

			return (allowedExt.Any(e => string.Equals(e, ext, StringComparison.InvariantCultureIgnoreCase)));

		}

		public static Image ResizeToFixedSize(Image imgPhoto, int Width, int Height) {
			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;
			int sourceX = 0;
			int sourceY = 0;
			int destX = 0;
			int destY = 0;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)Width / (float)sourceWidth);
			nPercentH = ((float)Height / (float)sourceHeight);
			if (nPercentH < nPercentW) {
				nPercent = nPercentH;
				//destX = Convert.ToInt16((Width -
				//			  (sourceWidth * nPercent)) / 2);
			} else {
				nPercent = nPercentW;
				//destY = Convert.ToInt16((Height -
				//			  (sourceHeight * nPercent)) / 2);
			}

			int destWidth = Width = (int)(sourceWidth * nPercent);
			int destHeight = Height = (int)(sourceHeight * nPercent);

			var bmPhoto = new Bitmap(Width, Height,
							  PixelFormat.Format24bppRgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
							 imgPhoto.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.Clear(Color.Transparent);
			grPhoto.InterpolationMode =
					InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto,
				new Rectangle(destX, destY, destWidth, destHeight),
				new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
				GraphicsUnit.Pixel);

			grPhoto.Dispose();
			return bmPhoto;
		}
	}

}
