﻿using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongCommentMap : ClassMap<SongComment> {

		public SongCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(800).Not.Nullable();

			References(m => m.Song).Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
