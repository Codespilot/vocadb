using System;
using System.Linq;
using System.IO;
using System.Drawing;

namespace VocaDb.Model.Helpers {

	public static class ImageHelper {

		private static readonly string[] allowedExt = new[] { ".bmp", ".gif", ".jpg", ".jpeg", ".png" };

		public static byte[] GetOriginalAndResizedImages(Stream input, int length, params Size[] sizes) {

			var buf = new Byte[length];
			input.Read(buf, 0, length);

			return buf;

		}

		public static bool IsValidImageExtension(string fileName) {

			var ext = Path.GetExtension(fileName);

			return (allowedExt.Any(e => string.Equals(e, ext, StringComparison.InvariantCultureIgnoreCase)));

		}

	}

}
