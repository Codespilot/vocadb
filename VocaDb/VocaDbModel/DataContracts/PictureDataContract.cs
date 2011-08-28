using System;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class PictureDataContract {

		public PictureDataContract(Byte[] bytes, string mime) {
			Bytes = bytes;
			Mime = mime;
		}

		public PictureDataContract(PictureData pictureData) {

			ParamIs.NotNull(() => pictureData);

			Bytes = pictureData.Bytes;
			Mime = pictureData.Mime;

		}

		public Byte[] Bytes { get; set; }

		public string Mime { get; set; }

	}
}
