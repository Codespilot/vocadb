﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	/// <summary>
	/// Tests for <see cref="CollectionHelper"/>.
	/// </summary>
	[TestClass]
	public class CollectionHelperTests {

		class Entity : IEntryWithIntId {

			public int Id { get; set; }

			public string Val { get; set; }

		}

		class EntityProto {
			
			public int Id { get; set; }

			public int Val { get; set; }

		}

		private bool Equality(string str, int val) {
			return str == val.ToString();
		}

		private bool EqualityEntity(Entity old, EntityProto val) {
			return old.Id == val.Id;
		}

		private string Fac(int val) {
			return val.ToString();
		}

		private Entity FacEntity(EntityProto val) {
			return new Entity { Val = val.Val.ToString() };
		}

		private bool Update(Entity old, EntityProto val) {

			if (old.Val == val.Val.ToString())
				return false;

			old.Val = val.Val.ToString();
			return true;

		}

		private Entity[] EntityList(params string[] str) {
			int id = 1;
			return str.Select(s => new Entity { Id = id++, Val = s}).ToArray();
		}

		private EntityProto[] EntityProtoList(params int[] str) {
			int id = 1;
			return str.Select(s => new EntityProto { Id = id++, Val = s}).ToArray();
		}

		private List<string> List(params string[] str) {
			return new List<string>(str);
		}

		private CollectionDiffWithValue<Entity, Entity> TestSyncWithValue(Entity[] oldItems, EntityProto[] newItems, 
			int addedCount = 0, int removedCount = 0, int editedCount = 0, int unchangedCount = 0) {
			
			var list = oldItems.ToList();
			var result = CollectionHelper.SyncWithContent(list, newItems, EqualityEntity, FacEntity, Update, null);

			Assert.IsNotNull(result, "result is not null");
			Assert.AreEqual(addedCount > 0 || removedCount > 0 || editedCount > 0, result.Changed, "is changed");
			Assert.AreEqual(addedCount, result.Added.Length, "Aadded");
			Assert.AreEqual(editedCount, result.Edited.Length, "Edited");
			Assert.AreEqual(removedCount, result.Removed.Length, "Removed");
			Assert.AreEqual(unchangedCount, result.Unchanged.Length, "Unchanged");
			return result;

		}

		[TestMethod]
		public void SortByIds() {
			
			var entries = EntityList("Meiko", "Rin", "Miku", "Luka", "Kaito");
			var idList = new[] { 1, 5, 3, 2, 4 }; // Meiko, Kaito, Miku, Rin, Luka

			var result = CollectionHelper.SortByIds(entries, idList);

			Assert.AreEqual("Meiko", result[0].Val);
			Assert.AreEqual("Kaito", result[1].Val);
			Assert.AreEqual("Miku", result[2].Val);
			Assert.AreEqual("Rin", result[3].Val);
			Assert.AreEqual("Luka", result[4].Val);

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

		[TestMethod]
		public void SyncWithContent_Added() {
			
			var result = TestSyncWithValue(EntityList(), EntityProtoList(39), addedCount: 1);
			Assert.AreEqual("39", result.Added.First().Val, "Added entity matches prototype");

		}

		[TestMethod]
		public void SyncWithContent_Removed() {
			
			var result = TestSyncWithValue(EntityList("39"), EntityProtoList(), removedCount: 1);
			Assert.AreEqual("39", result.Removed.First().Val, "Removed entity matches prototype");

		}

		[TestMethod]
		public void SyncWithContent_Unchanged() {
			
			var result = TestSyncWithValue(EntityList("39"), EntityProtoList(39), unchangedCount: 1);
			Assert.AreEqual("39", result.Unchanged.First().Val, "Unchanged entity matches prototype");

		}

		[TestMethod]
		public void SyncWithContent_Edited() {
			
			var result = TestSyncWithValue(EntityList("39"), EntityProtoList(3939), editedCount: 1, unchangedCount: 1);
			Assert.AreEqual("3939", result.Edited.First().Val, "Edited entity matches prototype");

		}

		// Replace the entity in the list with a completely new one.
		[TestMethod]
		public void SyncWithContent_Replaced() {
			
			var oldList = EntityList("39");
			var newList = EntityProtoList(3939);
			newList.First().Id = 2;

			var result = TestSyncWithValue(oldList, newList, addedCount: 1, removedCount: 1);
			Assert.AreEqual("3939", result.Added.First().Val, "Added entity matches prototype");
			Assert.AreEqual("39", result.Removed.First().Val, "Removed entity matches prototype");

		}
	}

}
