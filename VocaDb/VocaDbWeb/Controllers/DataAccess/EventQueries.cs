using System.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public class EventQueries {

		private readonly IEventRepository eventRepository;

		public EventQueries(IEventRepository eventRepository) {
			this.eventRepository = eventRepository;
		}

		public ReleaseEventContract[] ByDate() {
			
			return eventRepository.HandleQuery(ctx => ctx
				.Query()
				.Where(e => e.Date != null)
				.OrderByDescending(e => e.Date)
				.ToArray()
				.Select(e => new ReleaseEventContract(e))
				.ToArray());

		}

	}

}