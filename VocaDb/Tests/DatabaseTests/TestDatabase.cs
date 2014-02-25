using System;
using NHibernate;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Tests.DatabaseTests {

	public class TestDatabase {

		public const int ProducerId = 257;
		public const int SongId = 121;
		public const int Song2Id = 122;
		public const int SongWithArtistId = 7787;

		public Artist Producer { get; private set; }
		public Song Song { get; private set; }
		public Song Song2 { get; private set; }
		public Song SongWithArtist { get; private set; }

		public void Seed(ISessionFactory sessionFactory) {
			
			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction()) {
				
				Producer = new Artist(TranslatedString.Create("Junk")) { Id = ProducerId };
				session.Save(Producer);

				var tag = new Tag("Electronic");
				session.Save(tag);

				Song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English)) {
					Id = SongId, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1)
				};
				Song.Tags.Usages.Add(new SongTagUsage(Song, tag));
				session.Save(Song);

				Song2 = new Song(new LocalizedString("Tears of Palm", ContentLanguageSelection.English)) {
					Id = Song2Id, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1)
				};
				session.Save(Song2);

				SongWithArtist = new Song(new LocalizedString("Crystal Tears", ContentLanguageSelection.English)) { Id = SongWithArtistId, FavoritedTimes = 39, CreateDate = new DateTime(2012, 1, 1) };
				SongWithArtist.AddArtist(Producer);
				session.Save(SongWithArtist);

				tx.Commit();

			}

		}

	}
}
