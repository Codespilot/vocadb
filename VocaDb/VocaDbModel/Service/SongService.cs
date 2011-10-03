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
using VocaDb.Model.Domain.Ranking;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using log4net;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(SongService));

		private void Archive(ISession session, Song song) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, agentLoginData);
			session.Save(archived);

		}

		public SongService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) {

		}

		public ArtistForSongContract AddArtist(int songId, int artistId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			AuditLog("adding artist '" + artistId + "' to song '" + songId + "'");

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);

				var artistForSong = artist.AddSong(session.Load<Song>(songId));
				session.Update(artistForSong);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public ArtistForSongContract AddArtist(int songId, string newArtistName) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			AuditLog("creating artist '" + newArtistName + "' to song '" + songId + "'");

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				var artist = new Artist(new TranslatedString(newArtistName));

				var artistForSong = artist.AddSong(song);
				session.Save(artist);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public LyricsForSongContract CreateLyrics(int songId, ContentLanguageSelection language, string value, string source) {

			ParamIs.NotNullOrEmpty(() => value);
			ParamIs.NotNull(() => source);

			PermissionContext.HasPermission(PermissionFlags.ManageSongs);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				var entry = song.CreateLyrics(language, value, source);

				session.Update(song);
				return new LyricsForSongContract(entry);

			});

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

				return new SongContract(song);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageSongs);

			UpdateEntity<Album>(id, (session, a) => {

				log.Info(string.Format("'{0}' deleting song '{1}'", PermissionContext.Name, a.Name));

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			});

		}

		public SongWithAdditionalNamesContract[] Find(string query, int start, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Song>()
					.Where(s => 
						!s.Deleted &&
						(string.IsNullOrEmpty(query)
							|| s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
							|| s.TranslatedName.Japanese.Contains(query)
						|| (s.ArtistString.Contains(query))
						|| (s.NicoId != null && s.NicoId == query)))
					.OrderBy(s => s.TranslatedName.Japanese)
					.Take(maxResults)
					.ToArray();

				var additionalNames = session.Query<SongName>()
					.Where(m => m.Value.Contains(query) && !m.Song.Deleted)
					.Select(m => m.Song)
					.Distinct()
					.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				return direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.Select(a => new SongWithAdditionalNamesContract(a))
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
					.OrderBy(s => s.TranslatedName.Japanese)
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

			return HandleQuery(session => new SongDetailsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

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
				.Select(s => new SongContract(s))
				.ToArray());

		}

		public SongForEditContract UpdateLyrics(int songId, IEnumerable<LyricsForSongContract> lyrics) {
			
			ParamIs.NotNull(() => lyrics);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				var deleted = song.Lyrics.Where(l => !lyrics.Any(l2 => l.Id == l2.Id)).ToArray();
				var added = lyrics.Where(l => !song.Lyrics.Any(l2 => l.Id == l2.Id)).ToArray();

				foreach (var l in deleted) {
					song.Lyrics.Remove(l);
					session.Delete(l);
				}

				foreach (var entry in lyrics) {

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

	}

}
