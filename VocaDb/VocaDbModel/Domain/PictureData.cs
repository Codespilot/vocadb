using System;
using VocaDb.Model.DataContracts;
using System.Drawing;

namespace VocaDb.Model.Domain {

	public class PictureData {

		public PictureData() {}

		public PictureData(Byte[] bytes, string mime) {
			Bytes = bytes;
			Mime = mime;
		}

		public PictureData(PictureDataContract contract) {

			ParamIs.NotNull(() => contract);

			Bytes = contract.Bytes;
			Mime = contract.Mime;

		}

		public virtual Byte[] Bytes { get; set; }

		public virtual string Mime { get; set; }

		public virtual PictureThumb Thumb250 { get; set; }

		/// <summary>
		/// Automatically chooses the best picture for the requested size.
		/// </summary>
		/// <param name="requestedSize">Requested size. Can be Empty in which case the original size is returned.</param>
		/// <returns></returns>
		public virtual Byte[] GetBytes(Size requestedSize) {

			if (requestedSize != Size.Empty && Thumb250 != null && Thumb250.IsValidFor(requestedSize))
				return Thumb250.Bytes;

			return Bytes;

		}

	}

	public class PictureThumb {

		public PictureThumb() { }

		public virtual Byte[] Bytes { get; set; }

		public virtual int Size { get; set; }

		public virtual bool IsValidFor(Size requestedSize) {

			return (Bytes != null && requestedSize.Height <= Size && requestedSize.Width <= Size);

		}

	}
}
