using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEventContract {

		public ArchivedEventContract() { }

		public ArchivedEventContract(ReleaseEvent ev) {

			ParamIs.NotNull(() => ev);

			Date = ev.Date;
			Description = ev.Description;
			Id = ev.Id;
			Name = ev.Name;
			Series = (ev.Series != null ? new EntryRefContract(ev.Series) : null);
			SeriesNumber = ev.SeriesNumber;

		}

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public EntryRefContract Series { get; set; }

		[DataMember]
		public int SeriesNumber { get; set; }

	}

}
