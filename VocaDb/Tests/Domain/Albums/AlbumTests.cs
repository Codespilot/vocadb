﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Domain.Albums {

	[TestClass]
	public class AlbumTests {

		private Album album;
		private Artist producer;
		private ArtistContract producerContract;
		private Artist vocalist;
		private ArtistContract vocalistContract;
		private Song song1;
		private SongInAlbumEditContract songInAlbumContract; 

		private void AssertEquals(SongInAlbum first, SongInAlbumEditContract second, string message) {
			
			Assert.IsTrue(Album.TrackPropertiesEqual(first, second), message);
			Assert.IsTrue(Album.TrackArtistsEqual(first.Song, second), message);

		}

		private Artist[] GetArtists(ArtistContract[] contracts) {

			return contracts
				.Select(a => a.Id == vocalist.Id ? vocalist : new Artist(TranslatedString.Create(a.Name)) {Id = a.Id})
				.ToArray();

		}

		private Song GetSong(SongInAlbumEditContract contract) {

			if (contract.SongId == song1.Id)
				return song1;

			return new Song(new LocalizedString(contract.SongName, ContentLanguageSelection.Unspecified));

		}

		private void UpdateSongArtists(Song song, ArtistContract[] artists) {
			song.SyncArtists(artists, GetArtists);
		}

		private CollectionDiffWithValue<SongInAlbum, SongInAlbum> SyncSongs(SongInAlbumEditContract[] newSongs) {
			return album.SyncSongs(newSongs, GetSong, UpdateSongArtists);
		}

		[TestInitialize]
		public void SetUp() {

			album = new Album(new LocalizedString("Synthesis", ContentLanguageSelection.English));

			producer = new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer };
			vocalist = new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 39, ArtistType = ArtistType.Vocaloid };
			producerContract = new ArtistContract(producer, ContentLanguagePreference.Default);
			vocalistContract = new ArtistContract(vocalist, ContentLanguagePreference.Default);

			song1 = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English)) { Id = 2 };
			song1.AddArtist(producer);

			album.AddArtist(producer);
			album.AddArtist(vocalist);

			var songInAlbum = new SongInAlbum(song1, album, 1, 1);
			songInAlbumContract = new SongInAlbumEditContract(songInAlbum, ContentLanguagePreference.Default);
			songInAlbumContract.Artists = new[] { producerContract };

		}

		[TestMethod]
		public void Ctor_LocalizedString() {

			var result = new Album(new LocalizedString("album", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, result.Names.Count(), "Names count");
			Assert.IsTrue(result.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(result.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("album", result.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");

		}

		[TestMethod]
		public void CreateWebLink() {

			album.CreateWebLink("test link", "http://www.test.com", WebLinkCategory.Other);

			Assert.AreEqual(1, album.WebLinks.Count, "Should have one link");
			var link = album.WebLinks.First();
			Assert.AreEqual("test link", link.Description, "description");
			Assert.AreEqual("http://www.test.com", link.Url, "url");
			Assert.AreEqual(WebLinkCategory.Other, link.Category, "category");

		}

		[TestMethod]
		public void SyncSongs_NoExistingLinks() {

			var newSongs = new[] { songInAlbumContract };

			var result = SyncSongs(newSongs);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(1, result.Added.Length, "1 added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			AssertEquals(result.Added.First(), songInAlbumContract, "added song matches contract");
			Assert.AreEqual(result.Added.First().Song.ArtistList.Count(), 1, "one artist");

		}

		[TestMethod]
		public void SyncSongs_NotChanged() {

			album.AddSong(song1, 1, 1);
			var newSongs = new[] { songInAlbumContract };

			var result = SyncSongs(newSongs);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsFalse(result.Changed, "is not changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			AssertEquals(result.Unchanged.First(), songInAlbumContract, "unchanged song matches contract");

		}

		/// <summary>
		/// Edited track properties other than artists.
		/// </summary>
		[TestMethod]
		public void Sync_Contracts_EditedProperties() {

			album.AddSong(song1, 1, 1);
			songInAlbumContract.TrackNumber = 2;
			var newSongs = new[] { songInAlbumContract };

			var result = SyncSongs(newSongs);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(1, result.Edited.Length, "1 edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			AssertEquals(result.Edited.First(), songInAlbumContract, "edited song matches contract");
			Assert.AreEqual(2, result.Edited.First().TrackNumber, "edited song track number is updated");

		}

		/// <summary>
		/// Edited track artists list.
		/// </summary>
		[TestMethod]
		public void Sync_Contracts_EditedArtists() {

			album.AddSong(song1, 1, 1);
			songInAlbumContract.Artists = new[] { producerContract, vocalistContract };
			var newSongs = new[] { songInAlbumContract };

			var result = SyncSongs(newSongs);

			Assert.IsNotNull(result, "result is not null");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			AssertEquals(result.Unchanged.First(), songInAlbumContract, "edited song matches contract");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", result.Unchanged.First().Song.ArtistString.Default, "edited song artist string is updated");

		}

		[TestMethod]
		public void Sync_Contracts_Removed() {

			album.AddSong(song1, 1, 1);
			var newSongs = new SongInAlbumEditContract[0];

			var result = SyncSongs(newSongs);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(1, result.Removed.Length, "1 removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			AssertEquals(result.Removed.First(), songInAlbumContract, "removed song matches contract");

		}

		/*[TestMethod]
		public void SyncArtists_Duplicate() {

			var newArtists = new[] {
				new ArtistForAlbumContract(new ArtistForAlbum(album, artist, false, ArtistRoles.Default), ContentLanguagePreference.Default),
				new ArtistForAlbumContract(new ArtistForAlbum(album, artist, false, ArtistRoles.Composer), ContentLanguagePreference.Default),
			};

			album.SyncArtists(newArtists, c => c.Artist.Id == artist.Id ? artist : null);

			Assert.AreEqual(1, song.AllArtists.Count, "Only one artist");
			Assert.AreEqual(artist, song.AllArtists.First().Artist, "Artist is as expected");

		}*/

	}

}
