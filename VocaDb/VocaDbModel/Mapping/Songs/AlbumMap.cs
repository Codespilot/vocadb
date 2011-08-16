using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;
using VocaVoter.Model.Domain.Songs;

namespace VocaVoter.Model.Mapping.Songs {

	public class AlbumMap : ClassMap<Album> {

		public AlbumMap() {

			Schema("dbo");
			Id(m => m.Id);
			Map(m => m.ReleaseDate).Nullable();
			Component(m => m.LocalizedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
			});

		}

	}

}
