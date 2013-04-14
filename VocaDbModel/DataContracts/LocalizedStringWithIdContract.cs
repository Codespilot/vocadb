﻿using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class LocalizedStringWithIdContract : LocalizedStringContract {

		public LocalizedStringWithIdContract() {}

		public LocalizedStringWithIdContract(LocalizedStringWithId str) 
			: base(str) {

			Id = str.Id;

		}

		[DataMember]
		public int Id { get; set; }

	}
}
