using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		private EntryName GetEntryName(ISession session, EntryRef entryRef) {

			switch (entryRef.EntryType) {
				case EntryType.Album: {
					var entry = session.Load<Album>(entryRef.Id);
					return NameHelper.GetName(entry, PermissionContext.LanguagePreference);
				}

				case EntryType.Artist: {
						var entry = session.Load<Artist>(entryRef.Id);
						return NameHelper.GetName(entry, PermissionContext.LanguagePreference);
					}

				case EntryType.Song: {
						var entry = session.Load<Song>(entryRef.Id);
						return NameHelper.GetName(entry, PermissionContext.LanguagePreference);
					}

				case EntryType.SongList: {
						var entry = session.Load<SongList>(entryRef.Id);
						return new EntryName(entry.Name);
					}
				
			}

			return EntryName.Empty;

		}

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public ActivityEntryContract[] GetActivityEntries(int maxEntries) {

			return HandleQuery(session => {

				var entries = session.Query<ActivityEntry>().Where(a => a.Sticky).OrderByDescending(a => a.CreateDate).Take(maxEntries).ToArray();

				if (entries.Length < maxEntries)
					entries = entries.Concat(session.Query<ActivityEntry>().Where(a => !a.Sticky)
						.OrderByDescending(a => a.CreateDate).Take(maxEntries)).ToArray();

				var factory = new ActivityEntryContractFactory(entryRef => GetEntryName(session, entryRef));

				return entries.Select(factory.Create).ToArray();

			});

		}

		public FrontPageContract GetFrontPageContent() {

			return HandleQuery(session => new FrontPageContract(
				session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.CreateDate).Take(15),
				session.Query<Song>().Where(s => !s.Deleted)
					.OrderByDescending(s => s.CreateDate).Take(15), 
				PermissionContext.LanguagePreference));

		}

	}
}
