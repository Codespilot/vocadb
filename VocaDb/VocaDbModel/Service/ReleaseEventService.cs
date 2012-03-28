using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service {

	public class ReleaseEventService : ServiceBase {

		private static readonly Regex eventNameRegex = new Regex(@"(.+)(\d+)");

		public ReleaseEventService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext,entryLinkFactory) {}

		/*public ReleaseEventDetailsContract GetReleaseEventDetailsByName(string name) {

			return HandleQuery(session => new ReleaseEventDetailsContract(session.Query<ReleaseEvent>().F(r => r.Name == name)));

		}*/

		public void DeleteEvent(int id) {

			DeleteEntity<ReleaseEvent>(id, PermissionToken.ManageDatabase);

		}

		public void DeleteSeries(int id) {

			DeleteEntity<ReleaseEventSeries>(id, PermissionToken.ManageEventSeries);

		}

		public ReleaseEventFindResultContract Find(string query) {

			return HandleQuery(session => {

				// Attempt to match exact name
				var ev = session.Query<ReleaseEvent>().FirstOrDefault(e => e.Name == query);

				if (ev != null)
					return new ReleaseEventFindResultContract(ev);

				var match = eventNameRegex.Match(query);

				if (match.Success) {

					var seriesName = match.Groups[1].Value;
					var seriesNumber = int.Parse(match.Groups[2].Value);

					// Attempt to match series + series number
					ev = session.Query<ReleaseEvent>().FirstOrDefault(e => (seriesName.Contains(e.Series.Name) 
						|| e.Series.Aliases.Any(a => seriesName.Contains(a.Name))) && e.SeriesNumber == seriesNumber);

					if (ev != null)
						return new ReleaseEventFindResultContract(ev);

					// Attempt to match just the series
					var series = session.Query<ReleaseEventSeries>().FirstOrDefault(s => seriesName.Contains(s.Name) || s.Aliases.Any(a => seriesName.Contains(a.Name)));

					if (series != null)
						return new ReleaseEventFindResultContract(series, seriesNumber, query);

				}

				var events = session.Query<ReleaseEvent>().Where(e => query.Contains(e.Name) || e.Name.Contains(query)).Take(2).ToArray();

				if (events.Length != 1) {
					return new ReleaseEventFindResultContract(query);
				}

				return new ReleaseEventFindResultContract(events[0]);

			});

		}

		public ReleaseEventSeriesWithEventsContract[] GetReleaseEventsBySeries() {

			return HandleQuery(session => {

				var allEvents = session.Query<ReleaseEvent>().OrderBy(e => e.Date).ToArray();
				var series = session.Query<ReleaseEventSeries>().OrderBy(e => e.Name).ToArray();

				var seriesContracts = series.Select(s => 
					new ReleaseEventSeriesWithEventsContract(s, allEvents.Where(e => s.Equals(e.Series)), PermissionContext.LanguagePreference));
				var ungrouped = allEvents.Where(e => e.Series == null);

				return seriesContracts.Concat(new[] { new ReleaseEventSeriesWithEventsContract { 
					Name = string.Empty, 
					Events = ungrouped.Select(e => new ReleaseEventDetailsContract(e, PermissionContext.LanguagePreference)).ToArray() } }).ToArray();

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

		public ReleaseEventSeriesDetailsContract GetReleaseEventSeriesDetails(int id) {

			return HandleQuery(session => new ReleaseEventSeriesDetailsContract(session.Load<ReleaseEventSeries>(id), PermissionContext.LanguagePreference));

		}

		public ReleaseEventSeriesForEditContract GetReleaseEventSeriesForEdit(int id) {

			return HandleQuery(session => new ReleaseEventSeriesForEditContract(session.Load<ReleaseEventSeries>(id)));

		}

		public int UpdateEvent(ReleaseEventDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			return HandleTransaction(session => {

				ReleaseEvent ev;

				if (contract.Id == 0) {

					if (contract.Series != null) {
						var series = session.Load<ReleaseEventSeries>(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.Name);
					}

					session.Save(ev);

					AuditLog("created " + ev, session);

				} else {

					ev = session.Load<ReleaseEvent>(contract.Id);

					ev.Date = contract.Date;
					ev.Description = contract.Description;
					ev.Name = contract.Name;
					ev.SeriesNumber = contract.SeriesNumber;

					AuditLog("updated " + ev, session);

				}

				return ev.Id;

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
