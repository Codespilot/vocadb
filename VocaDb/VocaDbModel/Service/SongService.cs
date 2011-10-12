using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using log4net;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(SongService));

		private void Archive(ISession session, Song song) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, agentLoginData);
			session.Save(archived);

		}

		private int GetSongCount(ISession session, string query) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Song>()
					.Where(s => !s.Deleted)
					.Count();

			}

			var direct = session.Query<Song>()
				.Where(s =>
					!s.Deleted &&
					(string.IsNullOrEmpty(query)
						|| s.TranslatedName.English.Contains(query)
						|| s.TranslatedName.Romaji.Contains(query)
						|| s.TranslatedName.Japanese.Contains(query)
					|| (s.ArtistString.Contains(query))
					|| (s.NicoId != null && s.NicoId == query)))
				.ToArray();

			var additionalNames = session.Query<SongName>()
				.Where(m => m.Value.Contains(query) && !m.Song.Deleted)
				.Select(m => m.Song)
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

		public SongService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) {

		}

		public ArtistForSongContract AddArtist(int songId, int artistId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("linking {0} to {1}", artist, song));

				var artistForSong = artist.AddSong(song);
				session.Save(artistForSong);

				song.UpdateArtistString();
				session.Update(song);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public ArtistForSongContract AddArtist(int songId, string newArtistName) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog(string.Format("creating new artist {0} to {1}", newArtistName, song));

				var artist = new Artist(new TranslatedString(newArtistName));

				var artistForSong = artist.AddSong(song);
				session.Save(artist);

				song.UpdateArtistString();
				session.Update(song);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public LyricsForSongContract CreateLyrics(int songId, ContentLanguageSelection language, string value, string source) {

			ParamIs.NotNullOrEmpty(() => value);
			ParamIs.NotNull(() => source);

			PermissionContext.HasPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog("creating lyrics for " + song);

				var entry = song.CreateLyrics(language, value, source);

				session.Update(song);
				return new LyricsForSongContract(entry);

			});

		}

		public SongContract Create(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			AuditLog("creating a new song with name " + name);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = new Song(new TranslatedString(name), null);

				session.Save(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public LocalizedStringWithIdContract CreateName(int songId, string nameVal, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => nameVal);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog("creating a name " + nameVal + " for " + song);

				var name = song.CreateName(nameVal, language);
				session.Save(name);
				return new LocalizedStringWithIdContract(name);

			});

		}

		public PVForSongContract CreatePVForSong(int songId, PVService service, string pvId, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvId);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				AuditLog("creating a PV for " + song);

				var pv = song.CreatePV(service, pvId, pvType);
				session.Save(pv);

				if (string.IsNullOrEmpty(song.NicoId) && service == PVService.NicoNicoDouga && pvType == PVType.Original) {
					song.NicoId = pvId;
					session.Update(song);
				}

				return new PVForSongContract(pv);

			});

		}

		public PVForSongContract CreatePVForSong(int songId, string pvUrl, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			var result = VideoServiceHelper.ParseByUrl(pvUrl);

			return CreatePVForSong(songId, result.Service, result.Id, pvType);

		}

		public SongContract CreateSong(CreateSongContract contract) {

			return HandleTransaction(session => {

				if (!string.IsNullOrEmpty(contract.BasicData.NicoId)) {

					var existing = session.Query<Song>().FirstOrDefault(s => s.NicoId == contract.BasicData.NicoId);

					if (existing != null) {
						throw new ServiceException("Song with NicoId '" + contract.BasicData.NicoId + "' has already been added");
					}			
		
				}

				var song = new Song(new TranslatedString(contract.BasicData.Name), contract.BasicData.NicoId);

				if (contract.AlbumId != null)
					song.AddAlbum(session.Load<Album>(contract.AlbumId.Value), 0);

				if (contract.PerformerId != null)
					song.AddArtist(session.Load<Artist>(contract.PerformerId.Value));

				if (contract.ProducerId != null)
					song.AddArtist(session.Load<Artist>(contract.ProducerId.Value));

				song.UpdateArtistString();
				session.Save(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", a));

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			});

		}

		public void DeleteArtistForSong(int artistForSongId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var artistForSong = session.Load<ArtistForSong>(artistForSongId);

				AuditLog("deleting " + artistForSong);

				artistForSong.Song.DeleteArtistForSong(artistForSong);
				session.Delete(artistForSong);
				session.Update(artistForSong.Song);

			});

		}

		public void DeleteName(int nameId) {

			DeleteEntity<SongName>(nameId, PermissionFlags.ManageDatabase);

		}

		public void DeletePvForSong(int pvForSongId) {

			DeleteEntity<PVForSong>(pvForSongId, PermissionFlags.ManageDatabase);

		}

		public PartialFindResult<SongWithAdditionalNamesContract> Find(string query, int start, int maxResults, bool getTotalCount = false) {

			return HandleQuery(session => {

				if (string.IsNullOrWhiteSpace(query)) {

					var songs = session.Query<Song>()
						.Where(s => !s.Deleted)
						.OrderBy(s => s.TranslatedName.Romaji)
						.Skip(start)
						.Take(maxResults)
						.ToArray();

					var contracts = songs.Select(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
						.ToArray();

					var count = (getTotalCount ? GetSongCount(session, query) : 0);

					return new PartialFindResult<SongWithAdditionalNamesContract>(contracts, count);

				} else {

					var direct = session.Query<Song>()
						.Where(s =>
							!s.Deleted &&
							(string.IsNullOrEmpty(query)
								|| s.TranslatedName.English.Contains(query)
								|| s.TranslatedName.Romaji.Contains(query)
								|| s.TranslatedName.Japanese.Contains(query)
							|| (s.ArtistString.Contains(query))
							|| (s.NicoId != null && s.NicoId == query)))
						.OrderBy(s => s.TranslatedName.Romaji)
						.Take(maxResults)
						.ToArray();

					var additionalNames = session.Query<SongName>()
						.Where(m => m.Value.Contains(query) && !m.Song.Deleted)
						.Select(m => m.Song)
						.OrderBy(s => s.TranslatedName.Romaji)
						.Distinct()
						.Take(maxResults)
						.ToArray()
						.Where(a => !direct.Contains(a));

					var contracts = direct.Concat(additionalNames)
						.Skip(start)
						.Take(maxResults)
						.Select(a => new SongWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
						.ToArray();

					var count = (getTotalCount ? GetSongCount(session, query) : 0);

					return new PartialFindResult<SongWithAdditionalNamesContract>(contracts, count);

				}

			});

		}

		public SongContract[] FindByName(string term, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Song>()
					.Where(s =>
						!s.Deleted &&
						(string.IsNullOrEmpty(term)
							|| s.TranslatedName.English.Contains(term)
							|| s.TranslatedName.Romaji.Contains(term)
							|| s.TranslatedName.Japanese.Contains(term)
						|| (s.NicoId != null && s.NicoId == term)))
					.Take(maxResults)
					.ToArray();

				var additionalNames = session.Query<SongName>()
					.Where(m => m.Value.Contains(term) && !m.Song.Deleted)
					.Select(m => m.Song)
					.Distinct()
					.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				return direct.Concat(additionalNames)
					.Take(maxResults)
					.Select(a => new SongContract(a, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public LyricsForSongContract GetRandomSongWithLyricsDetails() {

			return HandleQuery(session => {

				var ids = session.Query<LyricsForSong>().Select(s => s.Id).ToArray();
				var id = ids[new Random().Next(ids.Length)];

				return new LyricsForSongContract(session.Load<LyricsForSong>(id));

			});

		}

		public int GetSongCount(string query) {

			return HandleQuery(session => {

				if (string.IsNullOrWhiteSpace(query)) {

					return session.Query<Song>()
						.Where(s => !s.Deleted)
						.Count();

				}

				//var artistForSong = (artistId != null ? session.Query<ArtistForSong>()
				//	.Where(a => a.Artist.Id == artistId).Select(a => a.Song).ToArray() : null);

				var direct = session.Query<Song>()
					.Where(s => 
						!s.Deleted &&
						(string.IsNullOrEmpty(query)
							|| s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
							|| s.TranslatedName.Japanese.Contains(query)
						|| (s.ArtistString.Contains(query))
						|| (s.NicoId == null || s.NicoId == query)))
					.ToArray();

				var additionalNames = session.Query<SongName>()
					.Where(m => m.Value.Contains(query) && !m.Song.Deleted)
					.Select(m => m.Song)
					.Distinct()
					.ToArray()
					.Where(a => !direct.Contains(a))
					.ToArray();

				return direct.Count() + additionalNames.Count();

			});

		}

		public SongDetailsContract GetSongDetails(int songId) {

			return HandleQuery(session => {

				var contract = new SongDetailsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference);

				if (PermissionContext.LoggedUser != null)
					contract.IsFavorited = session.Query<FavoriteSongForUser>()
						.Any(s => s.Song.Id == songId && s.User.Id == PermissionContext.LoggedUser.Id);

				return contract;

			});

		}

		public SongForEditContract GetSongForEdit(int songId) {

			return HandleQuery(session => new SongForEditContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public SongContract[] GetSongs(string filter, int start, int count) {

			return HandleQuery(session => session.Query<Song>()
				.Where(s => string.IsNullOrEmpty(filter) 
					|| s.TranslatedName.Japanese.Contains(filter) 
					|| s.NicoId == filter)
				.OrderBy(s => s.TranslatedName.Japanese)
				.Skip(start)
				.Take(count)
				.ToArray()
				.Select(s => new SongContract(s, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public SongForEditContract UpdateBasicProperties(SongForEditContract properties) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(properties.Song.Id);

				AuditLog(string.Format("updating properties for {0}", song));

				Archive(session, song);

				song.NicoId = properties.Song.NicoId;
				song.TranslatedName.CopyFrom(properties.TranslatedName);

				if (!string.IsNullOrEmpty(properties.Song.NicoId) && !song.PVs.Any(p => p.Service == PVService.NicoNicoDouga && p.PVId == properties.Song.NicoId)) {
					var pv = song.CreatePV(PVService.NicoNicoDouga, properties.Song.NicoId, PVType.Original);
					session.Save(pv);
				}

				session.Update(song);
				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

		public SongForEditContract UpdateLyrics(int songId, IEnumerable<LyricsForSongContract> lyrics) {
			
			ParamIs.NotNull(() => lyrics);

			var validLyrics = lyrics.Where(l => !string.IsNullOrEmpty(l.Value));

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog("updating lyrics for " + song);

				var deleted = song.Lyrics.Where(l => !validLyrics.Any(l2 => l.Id == l2.Id)).ToArray();

				foreach (var l in deleted) {
					song.Lyrics.Remove(l);
					session.Delete(l);
				}

				foreach (var entry in validLyrics) {

					var old = song.Lyrics.FirstOrDefault(l => l.Id == entry.Id);

					if (old != null) {

						old.Language = entry.Language;
						old.Source = entry.Source;
						old.Value = entry.Value;
						session.Update(old);

					} else {

						var l = song.CreateLyrics(entry.Language, entry.Value, entry.Source);
						session.Save(l);

					}

				}

				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

		public void UpdateNameLanguage(int nameId, ContentLanguageSelection lang) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<SongName>(nameId, name => name.Language = lang);

		}

		public void UpdateNameValue(int nameId, string val) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<SongName>(nameId, name => name.Value = val);

		}

	}

}
