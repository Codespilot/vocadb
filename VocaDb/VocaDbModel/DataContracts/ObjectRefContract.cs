﻿using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ObjectRefContract {

		public ObjectRefContract() {}

		public ObjectRefContract(int id, string nameHint) {

			Id = id;
			NameHint = nameHint;

		}

		public ObjectRefContract(IEntryBase entry) {

			ParamIs.NotNull(() => entry);

			Id = entry.Id;
			NameHint = entry.DefaultName;

		}

		/// <summary>
		/// Id of the referred object.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string NameHint { get; set; }

		public override string ToString() {
			return string.Format("{0} [{1}]", NameHint, Id);
		}

	}
}
