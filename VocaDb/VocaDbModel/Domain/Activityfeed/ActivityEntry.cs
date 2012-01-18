﻿using System;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Activityfeed {

	public abstract class ActivityEntry {

		private User author;

		protected ActivityEntry() {
			CreateDate = DateTime.Now;
		}

		protected ActivityEntry(User author, EntryEditEvent editEvent)	
			: this() {

			Author = author;
			EditEvent = editEvent;

		}

		public virtual User Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual EntryEditEvent EditEvent { get; set; }

		public abstract IEntryBase EntryBase { get; }

		public abstract INameManager EntryNames { get; }

		public abstract EntryType EntryType { get; }

		public virtual int Id { get; set; }

		public abstract void Accept(IActivityEntryVisitor visitor);

		public virtual bool IsDuplicate(ActivityEntry entry) {
			return (Author.Equals(entry.Author) && EntryBase.Equals(entry.EntryBase));
		}

	}

	public enum EntryEditEvent {

		Created,

		Updated

	}

}
