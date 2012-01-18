using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.DataContracts {

	public class EntryNameContract {

		public static EntryNameContract Empty {
			get {
				return new EntryNameContract(string.Empty);
			}
		}

		public EntryNameContract(string displayName, string additionalNames) {
			DisplayName = displayName;
			AdditionalNames = additionalNames;
		}

		public EntryNameContract(string displayName)
			: this(displayName, string.Empty) {}

		public string AdditionalNames { get; set; }

		public string DisplayName { get; set; }



	}

}
