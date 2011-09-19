using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistMap : ClassMap<Artist> {

		public ArtistMap() {

			//DiscriminateSubClassesOnColumn("ArtistType");
			Id(m => m.Id);
			Map(m => m.ArtistType).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			References(m => m.Circle);

			HasMany(m => m.Members)
				.Inverse()
				.KeyColumn("[Circle]");
			//HasMany(m => m.Metadata).Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.Names).Table("ArtistNames").Inverse().Cascade.All();
			HasMany(m => m.WebLinks).Table("ArtistWebLinks").Inverse().Cascade.All();
			HasMany(m => m.Songs).Table("ArtistsForSongs").Inverse().Cascade.All();

			Component(m => m.TranslatedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage").Not.Nullable();
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
			});

			Component(m => m.Picture, c => {
				c.Map(m => m.Bytes, "PictureBytes");
				c.Map(m => m.Mime, "PictureMime");
			});

		}

	}

	/*public class CircleMap : SubclassMap<Circle> {
		
		public CircleMap() {
			
			DiscriminatorValue("Circle");

			HasMany(m => m.CircleMembers)
				.Inverse()
				.KeyColumn("[Circle]");

		}

	}

	public class PerformerMap : SubclassMap<Performer> {

		public PerformerMap() {

			DiscriminatorValue("Performer");

		}

	}

	public class ProducerMap : SubclassMap<Producer> {

		public ProducerMap() {

			DiscriminatorValue("Producer");

		}

	}*/

}
