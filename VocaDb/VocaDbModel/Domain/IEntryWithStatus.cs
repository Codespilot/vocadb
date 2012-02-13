using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain {

	public interface IEntryWithStatus : IEntryBase {

		EntryStatus Status { get; }

	}

}
