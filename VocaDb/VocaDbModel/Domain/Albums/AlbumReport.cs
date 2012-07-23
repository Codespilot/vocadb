using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumReport : EntryReport {

		private Album album;

		public AlbumReport() { }

		public AlbumReport(Album album, AlbumReportType reportType, User user, string hostname, string notes)
			: base(user, hostname, notes) {

			Album = album;
			ReportType = reportType;

		}

		public override IEntryWithNames EntryBase {
			get { return Album; }
		}

		public virtual AlbumReportType ReportType { get; set; }

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

	}

	public enum AlbumReportType {

		InvalidInfo = 1,

		Duplicate = 2,

		Inappropriate = 3,

		Other = 4

	}

}
