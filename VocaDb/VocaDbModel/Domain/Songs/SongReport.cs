namespace VocaDb.Model.Domain.Songs {

	public class SongReport : EntryReport {

		private Song song;

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

		BrokenPV		= 0,

		InvalidInfo		= 1,

		InAppropriate	= 2,

		Other			= 3

	}

}
