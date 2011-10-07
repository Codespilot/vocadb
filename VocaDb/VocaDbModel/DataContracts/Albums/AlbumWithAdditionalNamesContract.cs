using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumWithAdditionalNamesContract : AlbumContract {

		public AlbumWithAdditionalNamesContract(Album album, ContentLanguagePreference languagePreference) 
			: base(album, languagePreference) {

			AdditionalNames = string.Join(", ", album.AllNames.Where(n => n != Name));

		}

		public AlbumWithAdditionalNamesContract() {}

		[DataMember]
		public string AdditionalNames { get; set; }


	}

}
