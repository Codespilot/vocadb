using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts {

	public class EntryReportContract {

		public EntryReportContract(EntryReport report, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => report);

			Created = report.Created;
			Entry = new EntryRefWithNameContract(report.EntryBase, languagePreference);
			Hostname = report.Hostname;
			Id = report.Id;
			Notes = report.Notes;
			User = (report.User != null ? new UserBaseContract(report.User) : null);

		}

		public EntryReportContract(SongReport songReport, ContentLanguagePreference languagePreference)
			: this((EntryReport)songReport, languagePreference) {

			SongReportType = songReport.ReportType;

		}

		public DateTime Created { get; set; }

		public EntryRefWithNameContract Entry { get; set; }

		public string Hostname { get; set; }

		public int Id { get; set; }

		public string Notes { get; set; }

		public SongReportType SongReportType { get; set; }

		public UserBaseContract User { get; set; }

	}

	public class EntryReportContractFactory {

		public EntryReportContract Create(EntryReport report, ContentLanguagePreference languagePreference) {

			if (report is SongReport)
				return new EntryReportContract((SongReport)report, languagePreference);

			return new EntryReportContract(report, languagePreference);

		}

	}

}
