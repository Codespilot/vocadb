﻿using System;
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

		public ReleaseEventSeriesWithEventsContract[] GetReleaseEventsBySeries() {

			return HandleQuery(session => {

				var allEvents = session.Query<ReleaseEvent>().OrderBy(e => e.Name).ToArray();
				var series = session.Query<ReleaseEventSeries>().OrderBy(e => e.Name).ToArray();

				var seriesContracts = series.Select(s => new ReleaseEventSeriesWithEventsContract(s, allEvents.Where(e => series.Equals(e.Series))));
				var ungrouped = allEvents.Where(e => e.Series == null);

				return seriesContracts.Concat(new[] { new ReleaseEventSeriesWithEventsContract { 
					Name = string.Empty, 
					Events = ungrouped.Select(e => new ReleaseEventDetailsContract(e)).ToArray() } }).ToArray();

			});

		}

		public ReleaseEventSeriesForEditContract GetReleaseEventSeriesForEdit(int id) {

			return HandleQuery(session => new ReleaseEventSeriesForEditContract(session.Load<ReleaseEventSeries>(id)));

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
