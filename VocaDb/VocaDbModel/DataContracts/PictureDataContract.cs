using System;
using VocaDb.Model.Domain;
using System.Runtime.Serialization;

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

		}

		[DataMember]
		public Byte[] Bytes { get; set; }

		[DataMember]
		public string Mime { get; set; }

	}
}
