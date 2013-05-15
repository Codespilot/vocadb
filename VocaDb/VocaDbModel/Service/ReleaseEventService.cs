using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service {

	public class ReleaseEventService : ServiceBase {

		public ReleaseEventService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext,entryLinkFactory) {}

		/*public ReleaseEventDetailsContract GetReleaseEventDetailsByName(string name) {

			return HandleQuery(session => new ReleaseEventDetailsContract(session.Query<ReleaseEvent>().F(r => r.Name == name)));

		}*/

		public void Archive(ISession session, ReleaseEvent releaseEvent, ReleaseEventDiff diff, EntryEditEvent reason) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = releaseEvent.CreateArchivedVersion(diff, agentLoginData, reason);
			session.Save(archived);

		}

		public void DeleteEvent(int id) {

			DeleteEntity<ReleaseEvent>(id, PermissionToken.DeleteEntries);

		}

		public void DeleteSeries(int id) {

			DeleteEntity<ReleaseEventSeries>(id, PermissionToken.ManageEventSeries);

		}

		public ReleaseEventFindResultContract Find(string query) {

			if (string.IsNullOrEmpty(query))
				return new ReleaseEventFindResultContract();

			query = query.Trim().Normalize(NormalizationForm.FormKC);	// Replaces fullwidth characters with ASCII

			return HandleQuery(session => {

				return new ReleaseEventSearch(new QuerySourceSession(session)).Find(query);

			});

		}

		public ReleaseEventSeriesWithEventsContract[] GetReleaseEventsBySeries() {

			return HandleQuery(session => {

				var allEvents = session.Query<ReleaseEvent>().ToArray();
				var series = session.Query<ReleaseEventSeries>().OrderBy(e => e.Name).ToArray();

				var seriesContracts = series.Select(s => 
					new ReleaseEventSeriesWithEventsContract(s, allEvents.Where(e => s.Equals(e.Series)), PermissionContext.LanguagePreference));
				var ungrouped = allEvents.Where(e => e.Series == null).OrderBy(e => e.Name);

				return seriesContracts.Concat(new[] { new ReleaseEventSeriesWithEventsContract { 
					Name = string.Empty, 
					Events = ungrouped.Select(e => new ReleaseEventContract(e)).ToArray() } }).ToArray();

			});

		}

		public ReleaseEventDetailsContract GetReleaseEventDetails(int id) {

			return HandleQuery(session => new ReleaseEventDetailsContract(session.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference));

		}

		public ReleaseEventDetailsContract GetReleaseEventForEdit(int id) {

			return HandleQuery(session => new ReleaseEventDetailsContract(
				session.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference) {
					AllSeries = session.Query<ReleaseEventSeries>().Select(s => new ReleaseEventSeriesContract(s)).ToArray()
				});

		}

		public ReleaseEventWithArchivedVersionsContract GetReleaseEventWithArchivedVersions(int id) {

			return HandleQuery(session =>
				new ReleaseEventWithArchivedVersionsContract(session.Load<ReleaseEvent>(id)));

		}

		public ReleaseEventSeriesDetailsContract GetReleaseEventSeriesDetails(int id) {

			return HandleQuery(session => new ReleaseEventSeriesDetailsContract(session.Load<ReleaseEventSeries>(id), PermissionContext.LanguagePreference));

		}

		public ReleaseEventSeriesForEditContract GetReleaseEventSeriesForEdit(int id) {

			return HandleQuery(session => new ReleaseEventSeriesForEditContract(session.Load<ReleaseEventSeries>(id)));

		}

		public ReleaseEventContract UpdateEvent(ReleaseEventDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			return HandleTransaction(session => {

				ReleaseEvent ev;

				if (contract.Id == 0) {

					if (contract.Series != null) {
						var series = session.Load<ReleaseEventSeries>(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber);
						series.Events.Add(ev);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.Name);
					}

					session.Save(ev);

					Archive(session, ev, new ReleaseEventDiff(), EntryEditEvent.Created);

					AuditLog("created " + ev, session);

				} else {

					ev = session.Load<ReleaseEvent>(contract.Id);
					var diff = new ReleaseEventDiff();

					if (ev.Date != contract.Date)
						diff.Date = true;

					if (ev.Description != contract.Description)
						diff.Description = true;

					if (ev.Name != contract.Name)
						diff.Name = true;

					if (ev.SeriesNumber != contract.SeriesNumber)
						diff.SeriesNumber = true;

					ev.Date = contract.Date;
					ev.Description = contract.Description;
					ev.Name = contract.Name;
					ev.SeriesNumber = contract.SeriesNumber;

					session.Update(ev);

					Archive(session, ev, diff, EntryEditEvent.Updated);

					var logStr = string.Format("updated properties for {0} ({1})", CreateEntryLink(ev), diff.ChangedFieldsString);
					AuditLog(logStr, session);

				}

				return new ReleaseEventContract(ev);

			});

		}

		public int UpdateSeries(ReleaseEventSeriesForEditContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageEventSeries);

			return HandleTransaction(session => {

				ReleaseEventSeries series;

				if (contract.Id == 0) {

					series = new ReleaseEventSeries(contract.Name, contract.Description, contract.Aliases);

					session.Save(series);

					AuditLog("created " + series, session);

				} else {

					series = session.Load<ReleaseEventSeries>(contract.Id);

					series.Name = contract.Name;
					series.Description = contract.Description;
					series.UpdateAliases(contract.Aliases);

					AuditLog("updated " + series, session);

				}

				return series.Id;

			});

		}

	}

}
