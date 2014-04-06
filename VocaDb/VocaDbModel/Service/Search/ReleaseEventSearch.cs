using System;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search {

	public class ReleaseEventSearch {

		private static readonly Regex eventNameRegex = new Regex(@"([^\d]+)(\d+)(?:\s(\w+))?");

		private readonly IQuerySource querySource;

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
		}

		public ReleaseEventSearch(IQuerySource querySource) {
			this.querySource = querySource;
		}

		public ReleaseEventFindResultContract Find(string query) {

			// Attempt to match exact name
			var ev = Query<ReleaseEvent>().FirstOrDefault(e => e.Name == query);

			if (ev != null)
				return new ReleaseEventFindResultContract(ev);

			var match = eventNameRegex.Match(query);

			if (match.Success) {

				var seriesName = match.Groups[1].Value.Trim();
				var seriesNumber = Convert.ToInt32(match.Groups[2].Value);
				var seriesSuffix = (match.Groups.Count >= 4 ? match.Groups[3].Value : string.Empty);

				// Attempt to match series + series number
				var results = Query<ReleaseEvent>()
					.Where(e => e.SeriesNumber == seriesNumber 
						&& e.SeriesSuffix == seriesSuffix 
						&& (seriesName.StartsWith(e.Series.Name) || e.Series.Name.Contains(seriesName)
							|| e.Series.Aliases.Any(a => seriesName.StartsWith(a.Name) || a.Name.Contains(seriesName)))).ToArray();

				if (results.Length > 1)
					return new ReleaseEventFindResultContract();

				if (results.Length == 1)
					return new ReleaseEventFindResultContract(results[0]);

				// Attempt to match just the series
				var series = Query<ReleaseEventSeries>().FirstOrDefault(s => seriesName.StartsWith(s.Name) || s.Name.Contains(seriesName) || s.Aliases.Any(a => seriesName.StartsWith(a.Name) || a.Name.Contains(seriesName)));

				if (series != null)
					return new ReleaseEventFindResultContract(series, seriesNumber, seriesSuffix, query);

			}

			var events = Query<ReleaseEvent>().Where(e => query.Contains(e.Name) || e.Name.Contains(query)).Take(2).ToArray();

			if (events.Length != 1) {
				return new ReleaseEventFindResultContract(query);
			}

			return new ReleaseEventFindResultContract(events[0]);

		}

	}

}
