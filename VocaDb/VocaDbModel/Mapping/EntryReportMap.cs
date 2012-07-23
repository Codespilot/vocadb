using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping {

	public class EntryReportMap : ClassMap<EntryReport> {

		public EntryReportMap() {

			Id(m => m.Id);
			DiscriminateSubClassesOnColumn("[EntryType]");

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Hostname).Length(50).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();

			References(m => m.User).Nullable();

		}

	}

	public class SongReportMap : SubclassMap<SongReport> {

		public SongReportMap() {

			DiscriminatorValue("Song");

			Map(m => m.ReportType).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}


	}

}
