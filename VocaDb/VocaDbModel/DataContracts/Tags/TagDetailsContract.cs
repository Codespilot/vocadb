using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract {

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => tag);

			Albums = tag.AlbumTagUsages.OrderByDescending(a => a.Count)
				.Select(a => new AlbumTagUsageContract(a, languagePreference)).ToArray();
			Artists = tag.ArtistTagUsages.OrderByDescending(a => a.Count)
				.Select(a => new ArtistTagUsageContract(a, languagePreference)).ToArray();
			Name = tag.Name;

		}

		public AlbumTagUsageContract[] Albums { get; set; }

		public ArtistTagUsageContract[] Artists { get; set; }

		public string Name { get; set; }

	}

}
