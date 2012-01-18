﻿using System;
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
			Cache.ReadWrite();

			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.EditEvent).Not.Nullable();

			References(m => m.Author).Not.Nullable();

		}

	}

	public class AlbumActivityEntryMap : SubclassMap<AlbumActivityEntry> {

		public AlbumActivityEntryMap() {

			DiscriminatorValue("Album");

			References(m => m.Entry).Column("[Album]").Not.Nullable();

		}

	}

	public class ArtistActivityEntryMap : SubclassMap<ArtistActivityEntry> {

		public ArtistActivityEntryMap() {

			DiscriminatorValue("Artist");

			References(m => m.Entry).Column("[Artist]").Not.Nullable();

		}

	}

	public class SongActivityEntryMap : SubclassMap<SongActivityEntry> {

		public SongActivityEntryMap() {

			DiscriminatorValue("Song");

			References(m => m.Entry).Column("[Song]").Not.Nullable();

		}

	}

}
