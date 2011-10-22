﻿using System;
using VocaDb.Model.DataContracts;
using System.Drawing;

namespace VocaDb.Model.Domain {

	public class PictureData {

		public PictureData() {
		}

		public PictureData(Byte[] bytes, string mime) {
			Bytes = bytes;
			Mime = mime;
		}

		public PictureData(PictureDataContract contract) {

			ParamIs.NotNull(() => contract);

			Bytes = contract.Bytes;
			Mime = contract.Mime;

			if (contract.Thumb250 != null)
				Thumb250 = new PictureThumb250(contract.Thumb250);

		}

		public virtual Byte[] Bytes { get; set; }

		public virtual string Mime { get; set; }

		public virtual PictureThumb250 Thumb250 { get; set; }

		/// <summary>
		/// Automatically chooses the best picture for the requested size.
		/// </summary>
		/// <param name="requestedSize">Requested size. Can be Empty in which case the original size is returned.</param>
		/// <returns></returns>
		public virtual Byte[] GetBytes(Size requestedSize) {

			if (HasThumb(requestedSize))
				return Thumb250.Bytes;

			return Bytes;

		}

		public virtual bool HasThumb(Size requestedSize) {

			return requestedSize != Size.Empty && Thumb250 != null && Thumb250.IsValidFor(requestedSize);

		}

	}

	public class PictureThumb {

		public PictureThumb() { }

		public PictureThumb(byte[] bytes, int size) {

			Bytes = bytes;
			Size = size;

		}

		public PictureThumb(PictureThumbContract contract) {
			
			ParamIs.NotNull(() => contract);

			Bytes = contract.Bytes;
			Size = contract.Size;

		}

		public virtual Byte[] Bytes { get; set; }

		public virtual int Size { get; set; }

		public virtual bool IsValidFor(Size requestedSize) {

			return (Bytes != null && requestedSize.Height <= Size && requestedSize.Width <= Size);

		}

	}

	public class PictureThumb250 : PictureThumb {
		
		public PictureThumb250() {}

		public PictureThumb250(byte[] bytes)
			: base(bytes, 250) {}

		public PictureThumb250(PictureThumbContract contract)
			: base(contract) {
			
		}

		public override int Size {
			get { return 250; }
			set { base.Size = value; }
		}

	}
}
