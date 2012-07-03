﻿using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumCommentMap : ClassMap<AlbumComment> {

		public AlbumCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.AuthorName).Length(100).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(800).Not.Nullable();

			References(m => m.Album).Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
