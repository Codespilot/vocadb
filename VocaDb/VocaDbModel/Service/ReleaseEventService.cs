using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service {

	public class ReleaseEventService : ServiceBase {

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

		public ReleaseEventSeriesWithEventsContract[] GetReleaseEventsBySeries() {

			return HandleQuery(session => {

				var allEvents = session.Query<ReleaseEvent>().OrderBy(e => e.Name).ToArray();
				var series = session.Query<ReleaseEventSeries>().OrderBy(e => e.Name).ToArray();

				var seriesContracts = series.Select(s => new ReleaseEventSeriesWithEventsContract(s, allEvents.Where(e => s.Equals(e.Series))));
				var ungrouped = allEvents.Where(e => e.Series == null);

				return seriesContracts.Concat(new[] { new ReleaseEventSeriesWithEventsContract { 
					Name = string.Empty, 
					Events = ungrouped.Select(e => new ReleaseEventDetailsContract(e)).ToArray() } }).ToArray();

			});

		}

		public ReleaseEventDetailsContract GetReleaseEventDetails(int id) {

			return HandleQuery(session => new ReleaseEventDetailsContract(session.Load<ReleaseEvent>(id)));

		}

		public ReleaseEventDetailsContract GetReleaseEventForEdit(int id) {

			return HandleQuery(session => new ReleaseEventDetailsContract(
				session.Load<ReleaseEvent>(id)) {
					AllSeries = session.Query<ReleaseEventSeries>().Select(s => new ReleaseEventSeriesContract(s)).ToArray()
				});

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
