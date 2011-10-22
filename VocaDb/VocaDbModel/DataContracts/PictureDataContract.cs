using System;
using VocaDb.Model.Domain;
using System.Runtime.Serialization;
using System.Drawing;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PictureDataContract {

		public PictureDataContract() { }

		public PictureDataContract(Byte[] bytes, string mime) {
			Bytes = bytes;
			Mime = mime;
		}

		public PictureDataContract(PictureData pictureData) {

			ParamIs.NotNull(() => pictureData);

			Bytes = pictureData.Bytes;
			Mime = pictureData.Mime;
			Thumb250 = (pictureData.Thumb250 != null ? new PictureThumbContract(pictureData.Thumb250) : null);

		}

		[DataMember]
		public Byte[] Bytes { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public PictureThumbContract Thumb250 { get; set; }

	}

	[DataContract]
	public class PictureThumbContract {

		public PictureThumbContract() {}

		public PictureThumbContract(byte[] bytes, int size) {

			Bytes = bytes;
			Size = size;

		}

		public PictureThumbContract(PictureThumb thumb) {
			
			ParamIs.NotNull(() => thumb);

			Bytes = thumb.Bytes;
			Size = thumb.Size;

		}

		[DataMember]
		public virtual Byte[] Bytes { get; set; }

		[DataMember]
		public virtual int Size { get; set; }


	}

}
