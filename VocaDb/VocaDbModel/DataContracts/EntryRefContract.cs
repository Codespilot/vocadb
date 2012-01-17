using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class EntryRefContract {

		public EntryRefContract(EntryRef entryRef) {

			ParamIs.NotNull(() => entryRef);

			EntryType = entryRef.EntryType;
			Id = entryRef.Id;

		}

		public EntryType EntryType { get; set; }

		public int Id { get; set; }

	}

}
