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
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(SongService));

		private void Archive(ISession session, Song song) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, agentLoginData);
			session.Save(archived);

		}

		private int GetSongCount(ISession session, string query, bool onlyByName = false) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Song>()
					.Where(s => !s.Deleted)
					.Count();

			} else {

				var directQ = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (query.Length < 3) {

					directQ = directQ.Where(s =>
						s.TranslatedName.English == query
							|| s.TranslatedName.Romaji == query
							|| s.TranslatedName.Japanese == query);

				} else {

					directQ = directQ.Where(s =>
						s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
							|| s.TranslatedName.Japanese.Contains(query)
							|| (!onlyByName && s.ArtistString.Contains(query))
							|| (s.NicoId != null && s.NicoId == query));

				}

				var direct = directQ.ToArray();

				var additionalNamesQ = session.Query<SongName>()
					.Where(m => !m.Song.Deleted);
					
				if (query.Length < 3) {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

				} else {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

				}

				var additionalNames = additionalNamesQ
					.Select(m => m.Song)
					.Distinct()
					.ToArray()
					.Where(a => !direct.Contains(a))
					.ToArray();

				return direct.Count() + additionalNames.Count();
			}

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

				var artist = new Artist(newArtistName);

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

				var song = new Song(name);

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

				if (service == PVService.NicoNicoDouga && pvType == PVType.Original) {
					song.UpdateNicoId();
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

		public SongContract Create(CreateSongContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			AuditLog("creating a new song with name '" + contract.NameOriginal + "'");

			return HandleTransaction(session => {

				VideoUrlParseResult pvResult = null;

				if (!string.IsNullOrEmpty(contract.PVUrl)) {

					pvResult = VideoServiceHelper.ParseByUrl(contract.PVUrl);

					var existing = session.Query<PVForSong>().FirstOrDefault(
						s => s.Service == pvResult.Service && s.PVId == pvResult.Id && !s.Song.Deleted);

					if (existing != null) {
						throw new VideoParseException(string.Format("Song '{0}' already contains this PV", 
							existing.Song.TranslatedName[PermissionContext.LanguagePreference]));
					}			
		
				}

				var song = new Song();

				if (!string.IsNullOrEmpty(contract.NameOriginal))
					song.CreateName(contract.NameOriginal, ContentLanguageSelection.Japanese);

				if (!string.IsNullOrEmpty(contract.NameRomaji))
					song.CreateName(contract.NameRomaji, ContentLanguageSelection.Romaji);

				if (!string.IsNullOrEmpty(contract.NameEnglish))
					song.CreateName(contract.NameEnglish, ContentLanguageSelection.English);

				if (!string.IsNullOrEmpty(contract.NameOriginal))
					song.TranslatedName.DefaultLanguage = ContentLanguageSelection.Japanese;
				else if (!string.IsNullOrEmpty(contract.NameRomaji))
					song.TranslatedName.DefaultLanguage = ContentLanguageSelection.Romaji;
				else if (!string.IsNullOrEmpty(contract.NameEnglish))
					song.TranslatedName.DefaultLanguage = ContentLanguageSelection.English;

				session.Save(song);

				foreach (var artist in contract.Artists) {
					session.Save(song.AddArtist(session.Load<Artist>(artist.Id)));
				}

				if (pvResult != null) {
					session.Save(song.CreatePV(pvResult.Service, pvResult.Id, PVType.Original));
				}

				song.UpdateArtistString();
				session.Update(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.DeleteEntries);

			UpdateEntity<Song>(id, (session, a) => {

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

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var pvForSong = session.Load<PVForSong>(pvForSongId);

				AuditLog("deleting " + pvForSong);

				pvForSong.OnDelete();
				session.Delete(pvForSong);
				session.Update(pvForSong.Song);

			});

		}

		public PartialFindResult<SongWithAdditionalNamesContract> Find(string query, int start, int maxResults, 
			bool getTotalCount = false, NameMatchMode nameMatchMode = NameMatchMode.Auto, bool onlyByName = false) {

			return HandleQuery(session => {

				SongWithAdditionalNamesContract[] contracts;

				if (string.IsNullOrEmpty(query)) {

					contracts = session.Query<Song>()
						.Where(s => !s.Deleted)
						.OrderBy(s => s.Names.SortNames.Romaji)
						.Skip(start)
						.Take(maxResults)
						.ToArray()
						.Select(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
						.ToArray();

				} else {

					var directQ = session.Query<Song>()
						.Where(s => !s.Deleted);

					if (nameMatchMode == NameMatchMode.Exact || (nameMatchMode == NameMatchMode.Auto && query.Length < 3)) {

						directQ = directQ.Where(s =>
							s.Names.SortNames.English == query
								|| s.Names.SortNames.Romaji == query
								|| s.Names.SortNames.Japanese == query);

					} else {

						directQ = directQ.Where(s =>
							s.Names.SortNames.English.Contains(query)
								|| s.Names.SortNames.Romaji.Contains(query)
								|| s.Names.SortNames.Japanese.Contains(query)
								|| (!onlyByName && s.ArtistString.Contains(query))
								|| (s.NicoId != null && s.NicoId == query));

					}

					var direct = directQ
						.OrderBy(s => s.Names.SortNames.Romaji)
						.ToArray();

					var additionalNamesQ = session.Query<SongName>()
						.Where(m => !m.Song.Deleted);

					if (!string.IsNullOrEmpty(query) && query.Length < 3) {

						additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

					} else if (!string.IsNullOrEmpty(query)) {

						additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

					}

					var additionalNames = additionalNamesQ
						.Select(m => m.Song)
						.OrderBy(s => s.Names.SortNames.Romaji)
						.Distinct()
						//.Take(maxResults)
						.ToArray()
						.Where(a => !direct.Contains(a));

					contracts = direct.Concat(additionalNames)
						.Skip(start)
						.Take(maxResults)
						.Select(a => new SongWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
						.ToArray();

				}

				int count = (getTotalCount ? GetSongCount(session, query, onlyByName) : 0);

				return new PartialFindResult<SongWithAdditionalNamesContract>(contracts, count);

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

		public SongContract GetSong(int id) {

			return HandleQuery(
				session => new SongContract(session.Load<Song>(id), PermissionContext.LanguagePreference));

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

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionFlags.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Song>(sourceId);
				var target = session.Load<Song>(targetId);

				AuditLog("Merging " + source + " to " + target);
				Archive(session, source);
				Archive(session, target);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url);
					session.Save(link);
				}

				var pvs = source.PVs.Where(a => !target.HasPV(a.Service, a.PVId));
				foreach (var p in pvs) {
					var pv = target.CreatePV(p.Service, p.PVId, p.PVType);
					session.Save(p);
				}

				var artists = source.Artists.Where(a => !target.HasArtist(a.Artist)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				var albums = source.Albums.Where(s => !target.IsOnAlbum(s.Album)).ToArray();
				foreach (var s in albums) {
					s.Move(target);
					session.Update(s);
				}

				var userFavorites = source.UserFavorites.Where(a => !target.IsFavoritedBy(a.User)).ToArray();
				foreach (var u in userFavorites) {
					u.Move(target);
					session.Update(u);
				}

				source.Deleted = true;
				session.Update(source);

				target.UpdateArtistString();
				target.UpdateNicoId();
				session.Update(target);

			});

		}

		public SongForEditContract UpdateBasicProperties(SongForEditContract properties) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(properties.Song.Id);

				AuditLog(string.Format("updating properties for {0}", song));

				Archive(session, song);

				song.TranslatedName.CopyFrom(properties.TranslatedName);

				/*if (!string.IsNullOrEmpty(properties.Song.NicoId) && !song.PVs.Any(p => p.Service == PVService.NicoNicoDouga && p.PVId == properties.Song.NicoId)) {
					var pv = song.CreatePV(PVService.NicoNicoDouga, properties.Song.NicoId, PVType.Original);
					session.Save(pv);
				}*/

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
