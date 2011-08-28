using System;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain {

	public class PictureData {

		public PictureData() {}

		public PictureData(PictureDataContract contract) {

			Bytes = contract.Bytes;
			Mime = contract.Mime;

		}

		public Byte[] Bytes { get; set; }

		public string Mime { get; set; }

	}
}
