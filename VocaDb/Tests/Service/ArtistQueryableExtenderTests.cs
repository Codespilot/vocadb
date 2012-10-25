﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.Service {

	/// <summary>
	/// Tests for <see cref="ArtistQueryableExtender"/>.
	/// </summary>
	[TestClass]
	public class ArtistQueryableExtenderTests {

		private List<ArtistName> CreateArtistNames(params string[] names) {
			return names.Select(name => new ArtistName { Value = name }).ToList();
		}

		private List<ArtistName> artists; 

		private void SequenceEqual(IEnumerable<ArtistName> actual, string message, params string[] expected) {
			Assert.IsTrue(actual.Select(n => n.Value).SequenceEqual(expected), message);
		}

		[TestInitialize]
		public void SetUp() {

			artists = CreateArtistNames("HSP", "Hiroyuki ODA", "8#Prince");

		}

		/// <summary>
		/// Search doesn't match.
		/// HSS -> nothing
		/// </summary>
		[TestMethod]
		public void AddArtistNameFilter_NotMatch_NotFound() {

			var result = artists.AsQueryable().AddArtistNameFilter("HSS");

			SequenceEqual(result, "result");

		}

		/// <summary>
		/// Query is a P name, just below the minimum length for a Contains query.
		/// HSP -> HSP
		/// </summary>
		[TestMethod]
		public void AddArtistNameFilter_PName_QueryJustBelowMinLengthForContains_Found() {

			var result = artists.AsQueryable().AddArtistNameFilter("HSP");

			SequenceEqual(result, "result", "HSP");

		}

		/// <summary>
		/// Query is not a P name, just below the minimum length for a Contains query.
		/// HS -> HSP
		/// </summary>
		[TestMethod]
		public void AddArtistNameFilter_NotPName_QueryJustBelowMinLengthForContains_Found() {

			var result = artists.AsQueryable().AddArtistNameFilter("HS");

			SequenceEqual(result, "result", "HSP");

		}

		/// <summary>
		/// Query is not a P name, just above the minimum length for a Contains query.
		/// The query ends in P however.
		/// 8#P -> nothing
		/// Note: this is a special case that could be handled with Contains as well.
		/// </summary>
		[TestMethod]
		public void AddArtistNameFilter_NotPNameButEndsInP_QueryJustAboveMinLengthForContains_Found() {

			var result = artists.AsQueryable().AddArtistNameFilter("8#P");

			SequenceEqual(result, "result");

		}

		/// <summary>
		/// Query is not a P name, just above the minimum length for a Contains query.
		/// Hir -> Hiroyuki ODA
		/// </summary>
		[TestMethod]
		public void AddArtistNameFilter_NotPNameDoesNotEndInP_QueryJustAboveMinLengthForContains_Found() {

			var result = artists.AsQueryable().AddArtistNameFilter("Hir");

			SequenceEqual(result, "result", "Hiroyuki ODA");

		}

		/// <summary>
		/// Query is long enough for Contains/Words query.
		/// Hiroyuki -> Hiroyuki ODA
		/// </summary>
		[TestMethod]
		public void AddArtistNameFilter_NotPName_QueryLongEnoughForContains_Found() {

			var result = artists.AsQueryable().AddArtistNameFilter("Hiroyuki");

			SequenceEqual(result, "result", "Hiroyuki ODA");

		}

	}

}
