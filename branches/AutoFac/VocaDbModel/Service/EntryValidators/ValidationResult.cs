using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Service.EntryValidators {

	public class ValidationResult {

		public ValidationResult(IEnumerable<string> errors) {
			Errors = errors.ToArray();
		}

		public string[] Errors { get; private set; }

		public bool Passed {
			get {
				return !Errors.Any();
			}
		}

	}

}
