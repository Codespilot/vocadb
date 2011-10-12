using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts {

	[DataContract]
	public class GenericResponse {

		public GenericResponse() {
			Successful = true;
		}

		public GenericResponse(bool successful, string message) {
			Successful = successful;
			Message = message;
		}

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool Successful { get; set; }

	}

	[DataContract]
	public class GenericResponse<TResult> : GenericResponse {

		public GenericResponse(TResult result) {
			Result = result;
		}

		public GenericResponse(string errorMessage)
			: base(false, errorMessage) {}

		[DataMember]
		public TResult Result { get; set; }

	}

}
