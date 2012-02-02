using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongListMap : ClassMap<SongList> {

		public SongListMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Length(2000).Not.Nullable();
			Map(m => m.Name).Length(200).Not.Nullable();

			References(m => m.Author).Not.Nullable();

			HasMany(m => m.AllSongs)
				.KeyColumn("[List]")
				.OrderBy("[Order]")
				.Inverse().Cascade.AllDeleteOrphan()
				.Cache.ReadWrite();

		}

	}

	public class SongInListMap : ClassMap<SongInList> {

		public SongInListMap() {

			Table("SongsInLists");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Order).Not.Nullable();

			References(m => m.List).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

}
