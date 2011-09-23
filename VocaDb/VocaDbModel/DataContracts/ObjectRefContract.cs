using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ObjectRefContract {

		public ObjectRefContract() {}

		public ObjectRefContract(int id, string nameHint) {

			Id = id;
			NameHint = nameHint;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string NameHint { get; set; }

	}
}
