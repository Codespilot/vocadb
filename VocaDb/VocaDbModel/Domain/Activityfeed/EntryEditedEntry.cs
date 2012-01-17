﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public class EntryEditedEntry : ActivityEntry {

		private EntryRef entryRef;

		public EntryEditedEntry() { }

		public EntryEditedEntry(User author, bool sticky, EntryRef entryRef, EntryEditEvent editEvent)
			: base(author, sticky) {

			EntryRef = entryRef;
			EditEvent = editEvent;

		}

		public virtual EntryEditEvent EditEvent { get; set; }

		public virtual EntryRef EntryRef {
			get { return entryRef; }
			set {
				ParamIs.NotNull(() => value);
				entryRef = value;
			}
		}

		public override ActivityEntryType EntryType {
			get {
				return ActivityEntryType.EntryEdited;
			}
		}

	}

	public enum EntryEditEvent {

		Created,

		Updated

	}

}
