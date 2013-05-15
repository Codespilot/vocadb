using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Search;
using VocaDb.Tests.Mocks;

namespace VocaDb.Tests.Service.Search {

	/// <summary>
	/// Tests for <see cref="ReleaseEventSearch"/>.
	/// </summary>
	[TestClass]
	public class ReleaseEventSearchTests {

		private ReleaseEvent eventInSeries;
		private ReleaseEvent unsortedEvent;
		private QuerySourceList querySource;
		private ReleaseEventSeries series;
		private ReleaseEventSearch target;

		[TestInitialize]
		public void SetUp() {

			querySource = new QuerySourceList();

			target = new ReleaseEventSearch(querySource);

			series = new ReleaseEventSeries("Comiket", string.Empty, new[] { "C", "Comic Market" });

			eventInSeries = new ReleaseEvent(string.Empty, null, series, 84);
			querySource.Add(eventInSeries);

			unsortedEvent = new ReleaseEvent(string.Empty, null, "Vocaloid Festa");
			querySource.Add(unsortedEvent);

			series.Events.Add(eventInSeries);
			querySource.Add(series);

		}

		private ReleaseEventFindResultContract Find(string query) {
			return target.Find(query);
		}

		/// <summary>
		/// Find by combined series and event.
		/// </summary>
		[TestMethod]
		public void FindSeriesAndEvent() {

			var result = Find("Comiket 84");

			Assert.IsNotNull(result, "Result");
			Assert.AreEqual("Comiket 84", result.EventName, "EventName");

		}

		/// <summary>
		/// Find by series, unknown event.
		/// </summary>
		[TestMethod]
		public void FindNewEventEventInSeriesExact() {

			var result = Find("Comiket 85");

			Assert.IsNotNull(result, "Result");
			Assert.IsNotNull(result.Series, "Series");
			Assert.AreEqual("Comiket", result.Series.Name, "Series");
			Assert.AreEqual(85, result.SeriesNumber, "SeriesNumber");

		}

		/// <summary>
		/// Find by series, unknown event.
		/// </summary>
		[TestMethod]
		public void FindUnsortedEvent() {

			var result = Find("Vocaloid Festa");

			Assert.IsNotNull(result, "Result");
			Assert.AreEqual("Vocaloid Festa", result.EventName, "EventName");

		}

		/// <summary>
		/// Doesn't match any series.
		/// </summary>
		[TestMethod]
		public void FindNothing() {

			var result = Find("Does not exist");

			Assert.IsNotNull(result, "Result");
			Assert.AreEqual(null, result.EventName, "EventName");

		}

	}

}
