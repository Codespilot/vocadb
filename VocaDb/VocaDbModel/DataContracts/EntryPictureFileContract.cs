using System.IO;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryPictureFileContract {

		public EntryPictureFileContract() { }

		public EntryPictureFileContract(EntryPictureFile picture) {

			ParamIs.NotNull(() => picture);

			EntryType = picture.EntryType;
			FileName = picture.FileName;
			Id = picture.Id;
			Mime = picture.Mime;
			Name = picture.Name;

		}

		public int ContentLength { get; set;}

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// File data stream. Only used for uploads.
		/// </summary>
		public Stream UploadedFile { get; set; }

		public EntryPictureFileContract NullToEmpty() {
			Name = Name ?? string.Empty;
			return this;
		}

	}
}
