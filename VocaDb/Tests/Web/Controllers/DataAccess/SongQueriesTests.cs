﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="SongQueries"/>.
	/// </summary>
	[TestClass]
	public class SongQueriesTests {

		private FakeUserMessageMailer mailer;
		private CreateSongContract newSongContract;
		private FakePermissionContext permissionContext;
		private Artist producer;
		private FakePVParser pvParser;
		private FakeSongRepository repository;
		private SongQueries queries;
		private Song song;
		private User user;
		private User user2;
		private Artist vocalist;
		private Artist vocalist2;

		private SongContract CallCreate() {
			return queries.Create(newSongContract);
		}

		private NewSongCheckResultContract CallFindDuplicates(string[] anyName = null, string[] anyPv = null, bool getPvInfo = true) {
			
			return queries.FindDuplicates(anyName ?? new string[0], anyPv ?? new string[0], getPvInfo);

		}

		private void AssertHasArtist(Song song, Artist artist, ArtistRoles? roles = null) {
			Assert.IsTrue(song.Artists.Any(a => a.Artist.Equals(artist)), song + " has " + artist);			
			if (roles.HasValue)
				Assert.IsTrue(song.Artists.Any(a => a.Artist.Equals(artist) && a.Roles == roles), artist + " has roles " + roles);
		}

		private ArtistForSongContract CreateArtistForSongContract(int artistId = 0, string artistName = null, ArtistRoles roles = ArtistRoles.Default) {
			if (artistId != 0)
				return new ArtistForSongContract { Artist = new ArtistContract { Name = artistName, Id = artistId }, Roles = roles };
			else
				return new ArtistForSongContract { Name = artistName, Roles = roles };
		}

		private T Save<T>(T entry) {
			return repository.Save(entry);
		}

		[TestInitialize]
		public void SetUp() {

			producer = CreateEntry.Producer(id: 1, name: "Tripshots");
			vocalist = CreateEntry.Vocalist(id: 39, name: "Hatsune Miku");
			vocalist2 = CreateEntry.Vocalist(id: 40, name: "Kagamine Rin");

			song = CreateEntry.Song(id: 1, name: "Nebula");
			song.LengthSeconds = 39;
			repository = new FakeSongRepository(song);
			Save(song.AddArtist(producer));
			Save(song.AddArtist(vocalist));
			Save(song.CreatePV(new PVContract { Service = PVService.Youtube, PVId = "hoLu7c2XZYU", Name = "Nebula", PVType = PVType.Original }));

			foreach (var name in song.Names)
				repository.Save(name);

			user = CreateEntry.User(id: 1, name: "Miku");
			user.GroupId = UserGroupId.Trusted;
			user2 = CreateEntry.User(id: 2, name: "Rin", email: "rin@vocadb.net");
			repository.Add(user, user2);
			repository.Add(producer, vocalist);

			repository.Add(new Tag("vocarock"), new Tag("vocaloud"));

			permissionContext = new FakePermissionContext(user);
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			newSongContract = new CreateSongContract {
				SongType = SongType.Original,
				Names = new[] {
					new LocalizedStringContract("Resistance", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistContract(producer, ContentLanguagePreference.Default),
					new ArtistContract(vocalist, ContentLanguagePreference.Default), 
				},
				PVUrl = "http://test.vocadb.net/"
			};

			pvParser = new FakePVParser();
			pvParser.ResultFunc = (url, getMeta) => 
				VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939", 
				getMeta ? VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", "testimg.jpg", 39) : VideoTitleParseResult.Empty);

			mailer = new FakeUserMessageMailer();

			queries = new SongQueries(repository, permissionContext, entryLinkFactory, pvParser, mailer, new FakeLanguageDetector());

		}

		[TestMethod]
		public void Create() {

			var result = CallCreate();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Resistance", result.Name, "Name");

			song = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			Assert.IsNotNull(song, "Song was saved to repository");
			Assert.AreEqual("Resistance", song.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, song.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, song.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(song.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", song.ArtistString.Default, "ArtistString");
			Assert.AreEqual(39, song.LengthSeconds, "Length");	// From PV
			Assert.AreEqual(PVServices.NicoNicoDouga, song.PVServices, "PVServices");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(song, archivedVersion.Song, "Archived version song");
			Assert.AreEqual(SongArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(song, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

			var pv = repository.List<PVForSong>().FirstOrDefault(p => p.Song.Id == song.Id);

			Assert.IsNotNull(pv, "PV was created");
			Assert.AreEqual(song, pv.Song, "pv.Song");
			Assert.AreEqual("Resistance", pv.Name, "pv.Name");

		}

		[TestMethod]
		public void Create_Notification() {

			repository.Save(user2.AddArtist(producer));

			CallCreate();

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.Receiver, "Receiver");

		}

		[TestMethod]
		public void Create_NoNotificationForSelf() {

			repository.Save(user.AddArtist(producer));

			CallCreate();

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification was created");

		}

		[TestMethod]
		public void Create_EmailNotification() {
			
			var subscription = repository.Save(user2.AddArtist(producer));
			subscription.EmailNotifications = true;

			CallCreate();

			var notification = repository.List<UserMessage>().First();

			Assert.AreEqual(notification.Subject, mailer.Subject, "Subject");
			Assert.IsNotNull(mailer.Body, "Body");
			Assert.AreEqual(notification.Receiver.Name, mailer.ReceiverName, "ReceiverName");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			CallCreate();

		}

		[TestMethod]
		public void Create_Tags() {
		
			pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "vocarock"});
				
			CallCreate();

			song = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			Assert.AreEqual(1, song.Tags.Tags.Count(), "Tags.Count");
			Assert.IsTrue(song.Tags.HasTag("vocarock"), "Has vocarock tag");

		}

		// Two PVs, no matches, parse song info from the NND PV.
		[TestMethod]
		public void FindDuplicates_NoMatches_ParsePVInfo() {

			// Note: for now only NNDPV will be used for song metadata parsing.
			pvParser.MatchedPVs.Add("http://youtu.be/123456567",
				VideoUrlParseResult.CreateOk("http://youtu.be/123456567", PVService.Youtube, "123456567", 
				VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", "testimg2.jpg", 33)));

			pvParser.MatchedPVs.Add("http://www.nicovideo.jp/watch/sm3183550",
				VideoUrlParseResult.CreateOk("http://www.nicovideo.jp/watch/sm3183550", PVService.NicoNicoDouga, "sm3183550", 
				VideoTitleParseResult.CreateSuccess("anger", "Tripshots", "testimg.jpg", 39)));

			var result = CallFindDuplicates(new []{ "【初音ミク】anger PV EDIT【VOCALOID3DPV】"}, new []{ "http://youtu.be/123456567", "http://www.nicovideo.jp/watch/sm3183550" });

			Assert.AreEqual("anger", result.Title, "Title"); // Title from PV
			Assert.AreEqual(0, result.Matches.Length, "No matches");

		}

		[TestMethod]
		public void FindDuplicates_MatchName() {

			var result = CallFindDuplicates(new []{ "Nebula"});

			Assert.AreEqual(1, result.Matches.Length, "Matches");
			var match = result.Matches.First();
			Assert.AreEqual(song.Id, match.Entry.Id, "Matched song");
			Assert.AreEqual(SongMatchProperty.Title, match.MatchProperty, "Matched property");

		}

		[TestMethod]
		public void FindDuplicates_SkipPVInfo() {

			var result = CallFindDuplicates(new []{ "Anger"}, new []{ "http://www.nicovideo.jp/watch/sm393939" }, getPvInfo: false);

			Assert.IsNull(result.Title, "Title");
			Assert.AreEqual(0, result.Matches.Length, "No matches");

		}

		[TestMethod]
		public void FindDuplicates_MatchPV() {

			pvParser.MatchedPVs.Add("http://youtu.be/hoLu7c2XZYU",
				VideoUrlParseResult.CreateOk("http://youtu.be/hoLu7c2XZYU", PVService.Youtube, "hoLu7c2XZYU", VideoTitleParseResult.Empty));

			var result = CallFindDuplicates(anyPv: new []{ "http://youtu.be/hoLu7c2XZYU"});

			Assert.AreEqual(1, result.Matches.Length, "Matches");
			var match = result.Matches.First();
			Assert.AreEqual(song.Id, match.Entry.Id, "Matched song");
			Assert.AreEqual(SongMatchProperty.PV, match.MatchProperty, "Matched property");

		}

		[TestMethod]
		public void Merge_ToEmpty() {

			var song2 = new Song();
			repository.Save(song2);

			queries.Merge(song.Id, song2.Id);

			Assert.AreEqual("Nebula", song2.Names.AllValues.FirstOrDefault(), "Name");
			Assert.AreEqual(2, song2.AllArtists.Count, "Artists");
			AssertHasArtist(song2, producer);
			AssertHasArtist(song2, vocalist);
			Assert.AreEqual(song.LengthSeconds, song2.LengthSeconds, "LengthSeconds");

			var mergeRecord = repository.List<SongMergeRecord>().FirstOrDefault();
			Assert.IsNotNull(mergeRecord, "Merge record was created");
			Assert.AreEqual(song.Id, mergeRecord.Source, "mergeRecord.Source");
			Assert.AreEqual(song2.Id, mergeRecord.Target.Id, "mergeRecord.Target.Id");

		}

		[TestMethod]
		public void Merge_WithArtists() {

			song.GetArtistLink(producer).Roles = ArtistRoles.Instrumentalist;
			var song2 = CreateEntry.Song();
			repository.Save(song2);
			song2.AddArtist(vocalist);
			song2.AddArtist(vocalist2).Roles = ArtistRoles.Other;

			queries.Merge(song.Id, song2.Id);
			Assert.AreEqual(3, song2.AllArtists.Count, "Artists");
			AssertHasArtist(song2, producer, ArtistRoles.Instrumentalist);
			AssertHasArtist(song2, vocalist2, ArtistRoles.Other);

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Merge_NoPermissions() {

			user.GroupId = UserGroupId.Regular;
			permissionContext.RefreshLoggedUser(repository);

			var song2 = new Song();
			repository.Save(song2);

			queries.Merge(song.Id, song2.Id);

		}

		[TestMethod]
		public void Update_Names() {
			
			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated song";

			contract = queries.UpdateBasicProperties(contract);

			var songFromRepo = repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", songFromRepo.DefaultName);
			Assert.AreEqual(1, songFromRepo.Version, "Version");
			Assert.AreEqual(2, songFromRepo.AllArtists.Count, "Number of artists");
			Assert.AreEqual(0, songFromRepo.AllAlbums.Count, "No albums");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(song, archivedVersion.Song, "Archived version song");
			Assert.AreEqual(SongArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(SongEditableFields.Names, archivedVersion.Diff.ChangedFields, "Changed fields");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(song, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		public void Update_Artists() {
			
			var newSong = CreateEntry.Song(name: "Anger");

			repository.Save(newSong);

			foreach (var name in newSong.Names)
				repository.Save(name);

			var contract = new SongForEditContract(newSong, ContentLanguagePreference.English);
			contract.Artists = new [] {
				CreateArtistForSongContract(artistId: producer.Id), 
				CreateArtistForSongContract(artistId: vocalist.Id),
				CreateArtistForSongContract(artistName: "Goomeh", roles: ArtistRoles.Vocalist),
			};

			contract = queries.UpdateBasicProperties(contract);

			var songFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(3, songFromRepo.AllArtists.Count, "Number of artists");

			AssertHasArtist(songFromRepo, producer);
			AssertHasArtist(songFromRepo, vocalist);
			Assert.AreEqual("Tripshots feat. Hatsune Miku, Goomeh", songFromRepo.ArtistString.Default, "Artist string");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(SongEditableFields.Artists, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_Artists_Notify() {
			
			repository.Save(user2.AddArtist(vocalist2));
			repository.Save(vocalist2);

			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.Artists = contract.Artists.Concat(new [] { CreateArtistForSongContract(vocalist2.Id)}).ToArray();

			queries.UpdateBasicProperties(contract);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.Receiver, "Receiver");

		}

	}
}
