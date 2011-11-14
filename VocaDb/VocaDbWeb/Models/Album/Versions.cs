using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model;
using VocaDb.Web.Models.Shared;
using VocaDb.Model.Domain.Albums;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Album {

	public class Versions {

		private string GetChangeString(AlbumEditableFields fields) {

			if (fields == AlbumEditableFields.Nothing)
				return string.Empty;

			var fieldNames = EnumVal<AlbumEditableFields>.Values.Where(a => fields.HasFlag(a)).Select(f => Translate.AlbumEditableField(f));
			return string.Join(", ", fieldNames);

		}

		private string GetReasonName(AlbumArchiveReason reason, string notes) {

			if (reason == AlbumArchiveReason.Unknown && !string.IsNullOrEmpty(notes))
				return notes;

			return Translate.AlbumArchiveReason(reason);

		}

		public Versions() { }

		public Versions(AlbumWithArchivedVersionsContract contract) {

			ParamIs.NotNull(() => contract);

			Album = contract;
			ArchivedVersions = contract.ArchivedVersions.Select(a => 
				new ArchivedObjectVersion(a, GetReasonName(a.Reason, a.Notes), GetChangeString(a.ChangedFields))).ToArray();

		}

		public AlbumContract Album { get; set; }

		public ArchivedObjectVersion[] ArchivedVersions { get; set; }

	}
}