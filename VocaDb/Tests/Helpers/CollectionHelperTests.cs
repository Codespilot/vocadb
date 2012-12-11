using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	/// <summary>
	/// Tests for <see cref="CollectionHelper"/>.
	/// </summary>
	[TestClass]
	public class CollectionHelperTests {

		private bool Equality(string str, int val) {
			return str == val.ToString();
		}

		private string Fac(int val) {
			return val.ToString();
		}

		private List<string> List(params string[] str) {
			return new List<string>(str);
		}

		[TestMethod]
		public void Sync_Added() {

			var oldItems = List();
			var newItems = new[] { 39 };

			var result = CollectionHelper.Sync(oldItems, newItems, Equality, Fac);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(1, result.Added.Length, "1 added");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.AreEqual("39", result.Added.First(), "added items matches prototype");

		}

		[TestMethod]
		public void Sync_NotChanged() {

			var oldItems = List("39");
			var newItems = new[] { 39 };

			var result = CollectionHelper.Sync(oldItems, newItems, Equality, Fac);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsFalse(result.Changed, "is not changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			Assert.AreEqual("39", result.Unchanged.First(), "unchanged item matches prototype");

		}

		[TestMethod]
		public void Sync_Replaced() {

			var oldItems = List("39");
			var newItems = new[] { 3939 };

			var result = CollectionHelper.Sync(oldItems, newItems, Equality, Fac);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(1, result.Added.Length, "1 added");
			Assert.AreEqual(1, result.Removed.Length, "1 removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.AreEqual("3939", result.Added.First(), "added item matches prototype");
			Assert.AreEqual("39", result.Removed.First(), "removed item matches prototype");

		}

		[TestMethod]
		public void Sync_Removed() {

			var oldItems = List("39");
			var newItems = new int[] { };

			var result = CollectionHelper.Sync(oldItems, newItems, Equality, Fac);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(1, result.Removed.Length, "1 removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.AreEqual("39", result.Removed.First(), "removed item matches prototype");

		}


	}

}
