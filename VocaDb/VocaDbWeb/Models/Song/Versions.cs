using System.Linq;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Song {

	public class Versions {

		public static ArchivedObjectVersion CreateForSong(ArchivedSongVersionContract artist) {

			return new ArchivedObjectVersion(artist, GetReasonName(artist.Reason, artist.Notes),
				GetChangeString(artist.ChangedFields));

		}

		private static string GetChangeString(SongEditableFields fields) {

			if (fields == SongEditableFields.Nothing)
				return string.Empty;

			var fieldNames = EnumVal<SongEditableFields>.Values.Where(f =>
				f != SongEditableFields.Nothing && fields.HasFlag(f)).Select(Translate.SongEditableField);

			return string.Join(", ", fieldNames);

		}

		private static string GetReasonName(SongArchiveReason reason, string notes) {

			if (reason == SongArchiveReason.Unknown)
				return notes;

			return Translate.SongArchiveReason(reason);

		}

		public Versions() { }

		public Versions(SongWithArchivedVersionsContract contract) {

			ParamIs.NotNull(() => contract);

			Song = contract;
			ArchivedVersions = contract.ArchivedVersions.Select(CreateForSong).ToArray();

		}

		public ArchivedObjectVersion[] ArchivedVersions { get; set; }

		public SongContract Song { get; set; }

	}

}