using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public PartialFindResult<ActivityEntryContract> GetActivityEntries(int maxEntries) {

			return GetActivityEntries(0, maxEntries);

		}

		public PartialFindResult<ActivityEntryContract> GetActivityEntries(int start, int maxEntries) {

			return HandleQuery(session => {

				var entries = session.Query<ActivityEntry>().OrderByDescending(a => a.CreateDate).Skip(start).Take(maxEntries).ToArray();

				var contracts = entries
					.Where(e => !e.EntryBase.Deleted)
					.Select(e => new ActivityEntryContract(e, PermissionContext.LanguagePreference))
					.ToArray();

				var count = session.Query<ActivityEntry>().Count();

				return new PartialFindResult<ActivityEntryContract>(contracts, count);

			});

		}

		public FrontPageContract GetFrontPageContent() {

			const int maxNewsEntries = 4;
			const int maxActivityEntries = 20;

			return HandleQuery(session => {

				var activityEntries = session.Query<ActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Take(maxActivityEntries)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted);

				var newsEntries = session.Query<NewsEntry>().Where(n => n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries).ToArray();

				if (newsEntries.Length < maxNewsEntries)
					newsEntries = newsEntries.Concat(session.Query<NewsEntry>().Where(n => !n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries - newsEntries.Length)).ToArray();

				var topAlbums = session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.RatingAverageInt)
					.OrderByDescending(a => a.RatingCount)
					.Take(10).ToArray();
				var newAlbums = session.Query<Album>().Where(a => !a.Deleted 
					&& a.OriginalRelease.ReleaseDate.Year != null 
					&& a.OriginalRelease.ReleaseDate.Month != null 
					&& a.OriginalRelease.ReleaseDate.Day != null)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Month)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Day)
					.Take(10).ToArray();

				var newSongs = session.Query<Song>().Where(s => !s.Deleted && s.PVServices != PVServices.Nothing)
					.OrderByDescending(s => s.CreateDate)
					.Take(20).ToArray();

				return new FrontPageContract(activityEntries, newsEntries, newAlbums, topAlbums, newSongs, PermissionContext.LanguagePreference);

			});

		}

	}
}
