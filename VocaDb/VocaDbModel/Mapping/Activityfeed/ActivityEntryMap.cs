using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Mapping.Activityfeed {

	public class ActivityEntryMap : ClassMap<ActivityEntry> {

		public ActivityEntryMap() {

			DiscriminateSubClassesOnColumn("[EntryType]");
			Table("ActivityEntries");
			Id(m => m.Id);

			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Sticky).Not.Nullable();

			References(m => m.Author).Not.Nullable();

		}

	}

	public class EntryEditedEntryMap : SubclassMap<EntryEditedEntry> {

		public EntryEditedEntryMap() {

			DiscriminatorValue("EntryEdited");

			Component(m => m.EntryRef, c => {
				c.Map(m => m.EntryType).Column("[EntryEditedEntryRefType]");
				c.Map(m => m.Id).Column("[EntryEditedEntryRefId]");
			});

			Map(m => m.EditEvent).Column("[EntryEditedEditEvent]").Not.Nullable();

		}

	}

	public class PlaintextEntryMap : SubclassMap<PlaintextEntry> {

		public PlaintextEntryMap() {

			DiscriminatorValue("Plaintext");

			Map(m => m.Text).Column("[TextEntryText]").Not.Nullable();

		}

	}

}
