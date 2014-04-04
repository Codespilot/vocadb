using System.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public class EventQueries {

		private readonly IEventRepository eventRepository;

		public EventQueries(IEventRepository eventRepository) {
			this.eventRepository = eventRepository;
		}

		public ReleaseEventContract[] List(EventSortRule sortRule, bool includeSeries = false) {
			
			return eventRepository.HandleQuery(ctx => ctx
				.Query()
				.Where(e => e.Date != null)
				.OrderBy(sortRule)
				.ToArray()
				.Select(e => new ReleaseEventContract(e, includeSeries))
				.ToArray());

		}

	}

	public enum EventSortRule {
		
		Name,

		Date,

		SeriesName

	}

	public static class EventQueryableExtender {

		public static IQueryable<ReleaseEvent> OrderBy(this IQueryable<ReleaseEvent> query, EventSortRule sortRule) {

			switch (sortRule) {
				case EventSortRule.Date:
					return query.OrderByDescending(r => r.Date);
				case EventSortRule.Name:
					return query.OrderBy(r => r.Name);
				case EventSortRule.SeriesName:
					return query
						.OrderBy(r => r.Series.Name)
						.ThenBy(r => r.SeriesNumber);
			}

			return query;

		} 

	}

}