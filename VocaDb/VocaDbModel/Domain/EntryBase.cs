using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain {

	public interface IEntryBase {

		string DefaultName { get; }

		EntryType EntryType { get; }

		int Id { get; }

	}

}
