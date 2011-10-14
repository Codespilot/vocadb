using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class OptionalDateTimeContract {

		public OptionalDateTimeContract() { }

		public OptionalDateTimeContract(OptionalDateTime dateTime) {

			ParamIs.NotNull(() => dateTime);

			Day = dateTime.Day;
			Month = dateTime.Month;
			Year = dateTime.Year;

		}

		[DataMember]
		public int? Day { get; set; }

		[DataMember]
		public int? Month { get; set; }

		[DataMember]
		public int? Year { get; set; }


	}

}
