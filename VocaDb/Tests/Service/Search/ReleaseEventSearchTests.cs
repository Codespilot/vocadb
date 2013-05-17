﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		private int eventId;
		private ReleaseEventSearch target;

		private void AreEqual(ReleaseEvent expected, ReleaseEventFindResultContract actual) {
			
			Assert.IsNotNull(actual, "Result");
			Assert.AreEqual(expected.Name, actual.EventName, "EventName");
			Assert.AreEqual(expected.Id, actual.EventId, "EventId");

		}

		private ReleaseEvent CreateEvent(ReleaseEventSeries series, int number) {

			var e = new ReleaseEvent(string.Empty, null, series, number) { Id = eventId++ };
			querySource.Add(e);
			series.Events.Add(e);

			return e;

		}

		private ReleaseEvent CreateEvent(string name) {

			var e = new ReleaseEvent(string.Empty, null, name) { Id = eventId++ };
			querySource.Add(e);

			return e;

		}

		private ReleaseEventSeries CreateSeries(string name, params string[] aliases) {

			var s = new ReleaseEventSeries(name, string.Empty, aliases);
			querySource.Add(s);

			return s;

		}

		[TestInitialize]
		public void SetUp() {

			querySource = new QuerySourceList();

			target = new ReleaseEventSearch(querySource);

			series = CreateSeries("Comiket", "C", "Comic Market");

			eventInSeries = CreateEvent(series, 84);
			unsortedEvent = CreateEvent("Vocaloid Festa");

		}

		private ReleaseEventFindResultContract Find(string query) {
			return target.Find(query);
		}

		/// <summary>
		/// Test preconditions
		/// </summary>
		[TestMethod]
		public void Ctor() {

			Assert.AreEqual("Comiket 84", eventInSeries.Name, "Name");
			Assert.AreEqual("Vocaloid Festa", unsortedEvent.Name, "Name");

		}

		/// <summary>
		/// Find by combined series and event.
		/// </summary>
		[TestMethod]
		public void FindSeriesAndEvent() {

			var result = Find("Comiket 84");

			AreEqual(eventInSeries, result);

		}

		/// <summary>
		/// Find by alias.
		/// </summary>
		[TestMethod]
		public void FindAlias() {

			var result = Find("C84");

			AreEqual(eventInSeries, result);

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
		/// Find by combined series and event, partial match.
		/// </summary>
		[TestMethod]
		public void FindSeriesAndEventPartial() {

			var voMas = CreateSeries("The Voc@loid M@ster");
			var voMas23 = CreateEvent(voMas, 23);
			var result = Find("Voc@loid M@ster 23");

			AreEqual(voMas23, result);

		}

		/// <summary>
		/// Find by series whose name contains a number.
		/// </summary>
		[TestMethod]
		[Ignore]
		public void FindSeriesWithNumber() {

			CreateSeries("M3");

			var result = Find("M3 2013");

			Assert.IsNotNull(result, "Result");
			Assert.IsNotNull(result.Series, "Series");
			Assert.AreEqual("M3", result.Series.Name, "Series");
			Assert.AreEqual(2013, result.SeriesNumber, "SeriesNumber");

		}

		/// <summary>
		/// Find by series, unknown event.
		/// </summary>
		[TestMethod]
		public void FindUnsortedEvent() {

			var result = Find("The Vocaloid Festa");

			AreEqual(unsortedEvent, result);

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
