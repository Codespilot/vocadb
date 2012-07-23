using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class SongReport : EntryReport {

		private Song song;

		public SongReport() { }

		public SongReport(Song song, User user, string hostname, string notes) 
			: base(user, hostname, notes) {

			Song = song;

		}

		public override IEntryBase EntryBase {
			get { return Song; }
		}

		public virtual SongReportType ReportType { get; set; }

		public virtual Song Song {
			get { return song; }
			set { 
				ParamIs.NotNull(() => value);
				song = value; 
			}
		}

	}

	public enum SongReportType {

		BrokenPV		= 1,

		InvalidInfo		= 2,

		Duplicate		= 3,

		InAppropriate	= 4,

		Other			= 5

	}

}
