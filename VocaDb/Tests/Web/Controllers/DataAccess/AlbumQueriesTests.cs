using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="AlbumQueries"/>.
	/// </summary>
	[TestClass]
	public class AlbumQueriesTests {

		private Album album;
		private InMemoryImagePersister imagePersister;
		private FakeUserMessageMailer mailer;
		private CreateAlbumContract newAlbumContract;
		private FakePermissionContext permissionContext;
		private Artist producer;
		private FakeAlbumRepository repository;
		private AlbumQueries queries;
		private User user;
		private Artist vocalist;

		private SongInAlbumEditContract CreateSongInAlbumEditContract(int trackNumber, int songId = 0, string songName = null) {
			return new SongInAlbumEditContract { DiscNumber = 1, TrackNumber = trackNumber, SongId = songId, SongName = songName, Artists = new ArtistContract[0] };
		}

		private ArtistForAlbumContract CreateArtistForAlbumContract(int artistId = 0, string artistName = null) {
			return new ArtistForAlbumContract { Artist = new ArtistContract { Name = artistName, Id = artistId } };
		}

		private AlbumForEditContract CallUpdate(AlbumForEditContract contract) {
			return queries.UpdateBasicProperties(contract, null);
		}

		private AlbumForEditContract CallUpdate(Stream image) {
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			using (var stream = image) {
				return queries.UpdateBasicProperties(contract, new EntryPictureFileContract { UploadedFile = stream, Mime = MediaTypeNames.Image.Jpeg });
			}		
		}

		[TestInitialize]
		public void SetUp() {
			
			producer = CreateEntry.Producer();
			vocalist = CreateEntry.Vocalist();

			album = CreateEntry.Album(id: 39, name: "Synthesis");
			repository = new FakeAlbumRepository(album);
			foreach (var name in album.Names)
				repository.Save(name);
			user = CreateEntry.User(1, "Miku");
			repository.Save(user);
			repository.Save(producer, vocalist);

			permissionContext = new FakePermissionContext(user);
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			newAlbumContract = new CreateAlbumContract {
				DiscType = DiscType.Album,
				Names = new[] {
					new LocalizedStringContract("Another Dimensions", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistContract(producer, ContentLanguagePreference.Default),
					new ArtistContract(vocalist, ContentLanguagePreference.Default), 
				}
			};

			imagePersister = new InMemoryImagePersister();
			mailer = new FakeUserMessageMailer();
			queries = new AlbumQueries(repository, permissionContext, entryLinkFactory, imagePersister, mailer);

		}

		[TestMethod]
		public void Create() {

			var result = queries.Create(newAlbumContract);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Another Dimensions", result.Name, "Name");

			album = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Another Dimensions"));

			Assert.IsNotNull(album, "Album was saved to repository");
			Assert.AreEqual("Another Dimensions", album.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, album.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, album.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(album.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", album.ArtistString.Default, "ArtistString");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			queries.Create(newAlbumContract);

		}

		[TestMethod]
		public void CreateComment() {

			var result = queries.CreateComment(39, "Hello world");
			Assert.IsNotNull(result, "Result");

			var comment = repository.List<AlbumComment>().FirstOrDefault();
			Assert.IsNotNull(comment, "Comment was saved");
			Assert.AreEqual(user, comment.Author, "Author");
			Assert.AreEqual(album, comment.Album, "Album");
			Assert.AreEqual("Hello world", comment.Message, "Comment message");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void CreateComment_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);
			
			queries.CreateComment(39, "Hello world");

		}

		[TestMethod]
		public void GetCoverPictureThumb() {
			
			var contract = CallUpdate(ResourceHelper.TestImage());

			var result = queries.GetCoverPictureThumb(contract.Id);

			Assert.IsNotNull(result, "result");
			Assert.IsNotNull(result.Picture, "Picture");
			Assert.IsNotNull(result.Picture.Bytes, "Picture content");
			Assert.AreEqual(contract.CoverPictureMime, result.Picture.Mime, "Picture MIME");
			Assert.AreEqual(contract.Id, result.EntryId, "EntryId");

		}

		[TestMethod]
		public void Update_Names() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);

			contract.Names.AllNames.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated album";

			contract = CallUpdate(contract);
			Assert.AreEqual(album.Id, contract.Id, "Update album Id as expected");

			var albumFromRepo = repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", albumFromRepo.DefaultName);
			Assert.AreEqual(1, albumFromRepo.Version, "Version");
			Assert.AreEqual(0, albumFromRepo.AllArtists.Count, "No artists");
			Assert.AreEqual(0, albumFromRepo.AllSongs.Count, "No songs");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(AlbumEditableFields.Names, archivedVersion.Diff.ChangedFields, "Changed fields");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		public void Update_Tracks() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			var existingSong = CreateEntry.Song(name: "Nebula");
			repository.Save(existingSong);

			contract.Songs = new[] {
				CreateSongInAlbumEditContract(1, songId: existingSong.Id),
				CreateSongInAlbumEditContract(2, songName: "Anger")
			};

			contract = CallUpdate(contract);

			var albumFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(2, albumFromRepo.AllSongs.Count, "Number of songs");

			var track1 = albumFromRepo.GetSongByTrackNum(1, 1);
			Assert.AreEqual(existingSong, track1.Song, "First track");

			var track2 = albumFromRepo.GetSongByTrackNum(1, 2);
			Assert.AreEqual("Anger", track2.Song.DefaultName, "Second track name");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Tracks, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_CoverPicture() {
			
			var contract = CallUpdate(ResourceHelper.TestImage());

			var albumFromRepo = repository.Load(contract.Id);

			Assert.IsNotNull(albumFromRepo.CoverPictureData, "CoverPictureData");
			Assert.IsNotNull(albumFromRepo.CoverPictureData.Bytes, "Original bytes are saved");
			Assert.IsNull(albumFromRepo.CoverPictureData.Thumb250, "Thumb bytes not saved anymore");
			Assert.AreEqual(MediaTypeNames.Image.Jpeg, albumFromRepo.CoverPictureData.Mime, "CoverPictureData.Mime");

			var thumbData = new EntryThumb(albumFromRepo, albumFromRepo.CoverPictureData.Mime);
			Assert.IsFalse(imagePersister.HasImage(thumbData, ImageSize.Original), "Original file was not created"); // Original saved in CoverPictureData.Bytes
			Assert.IsTrue(imagePersister.HasImage(thumbData, ImageSize.Thumb), "Thumbnail file was saved");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Cover, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		/*
		// TODO: artists not updated this way
		[TestMethod]
		public void Update_Artists() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			contract.ArtistLinks = new [] {
				CreateArtistForAlbumContract(artistId: producer.Id), 
				CreateArtistForAlbumContract(artistId: vocalist.Id)
			};

			contract = CallUpdate(contract);

			var albumFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(2, albumFromRepo.AllArtists.Count, "Number of artists");

			Assert.IsTrue(albumFromRepo.AllArtists.Any(a => a.Id == producer.Id), "Has producer");
			Assert.IsTrue(albumFromRepo.AllArtists.Any(a => a.Id == vocalist.Id), "Has vocalist");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", albumFromRepo.ArtistString, "Artist string");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Artists, archivedVersion.Diff.ChangedFields, "Changed fields");

		}*/

	}

}
