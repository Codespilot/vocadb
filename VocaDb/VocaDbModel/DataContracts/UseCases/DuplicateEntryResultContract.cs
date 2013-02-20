using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Model.DataContracts.UseCases {

	public class DuplicateEntryResultContract<T> where T : struct {

		public DuplicateEntryResultContract(EntryRefWithCommonPropertiesContract entry, T matchProperty) {

			Entry = entry;
			MatchProperty = matchProperty;

		}

		public EntryRefWithCommonPropertiesContract Entry { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public T MatchProperty { get; set; }

	}

}
