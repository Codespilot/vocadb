﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.Service.Helpers {

	/// <summary>
	/// Tests for <see cref="FindHelpers"/>.
	/// </summary>
	[TestClass]
	public class FindHelpersTests {

		private void TestGetMatchModeAndQueryForSearch(string query, string expectedQuery, NameMatchMode? expectedMode = null, NameMatchMode originalMode = NameMatchMode.Auto) {

			var result = FindHelpers.GetMatchModeAndQueryForSearch(query, ref originalMode);

			Assert.AreEqual(expectedQuery, result, "query");
			if (expectedMode != null)
				Assert.AreEqual(expectedMode, originalMode, "matchMode");
	
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_Default() {

			TestGetMatchModeAndQueryForSearch("Hatsune Miku", "Hatsune Miku", NameMatchMode.Words);

		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_EmptyQuery() {

			TestGetMatchModeAndQueryForSearch("", "");
	
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_Trim() {

			TestGetMatchModeAndQueryForSearch("Hatsune Miku         ", "Hatsune Miku", NameMatchMode.Words);

		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_WildCard() {

			TestGetMatchModeAndQueryForSearch("Hatsune Miku*", "Hatsune Miku", NameMatchMode.StartsWith);

		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_ShortQuery() {

			TestGetMatchModeAndQueryForSearch("H", "H", NameMatchMode.StartsWith);

		}

	}
}
