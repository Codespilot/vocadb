using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts {

	[DataContract]
	public class LocalizedStringContract {

		public LocalizedStringContract(LocalizedString str) {
			
			ParamIs.NotNull(() => str);

			Language = str.Language;
			Value = str.Value;

		}

		[DataMember]
		public ContentLanguageSelection Language { get; set; }

		[DataMember]
		public string Value { get; set; }

	}

}
