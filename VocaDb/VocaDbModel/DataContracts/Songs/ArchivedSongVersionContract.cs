using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class ArchivedSongVersionContract : ArchivedObjectVersionContract {

		public ArchivedSongVersionContract() { }

		public ArchivedSongVersionContract(ArchivedSongVersion archivedVersion)
			: base(archivedVersion) {

			ChangedFields = (archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields : SongEditableFields.Nothing);
			Reason = archivedVersion.Reason;

		}

		public SongEditableFields ChangedFields { get; set; }

		public SongArchiveReason Reason { get; set; }

	}

}
