using System;
using System.IO;

namespace VocaDb.Model.Helpers 
{
	public static class StreamHelper {


		public static void CopyStream(Stream source, Stream target) {
			const int bufSize = 1024;
			byte[] buf = new byte[bufSize];
			int bytesRead = 0;
			while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
				target.Write(buf, 0, bytesRead);
		}

		public static byte[] ReadStream(Stream input, long length) {

			if (input.CanSeek && input.Position > 0)
				input.Position = 0;

			int buffer = 1024;
			var buf = new byte[buffer];
			var wholeBuf = new byte[length];

			int count = 0;
			int offset = 0;
			do {
				count = input.Read(buf, 0, buffer);
				Array.Copy(buf, 0, wholeBuf, offset, count);
				offset += count;
			}
			while (count != 0);

			return wholeBuf;

		}

	}

}
