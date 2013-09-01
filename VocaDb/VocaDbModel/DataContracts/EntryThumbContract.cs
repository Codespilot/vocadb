using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract]
	public class EntryThumbContract : IPictureWithThumbs {

		public EntryThumbContract() {}

		public EntryThumbContract(EntryThumb entryThumb) {
			EntryType = entryThumb.EntryType;
			FileName = entryThumb.FileName;
			FileNameThumb = entryThumb.FileNameThumb;
			FileNameSmallThumb = entryThumb.FileNameSmallThumb;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryType EntryType { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public string FileNameSmallThumb { get; set; }

		[DataMember]
		public string FileNameThumb { get; set; }

	}
}
