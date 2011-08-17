using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class AlbumMap : ClassMap<Album> {

		public AlbumMap() {

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
