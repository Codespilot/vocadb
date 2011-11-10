using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistCommentMap : ClassMap<ArtistComment> {

		public ArtistCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(200).Not.Nullable();

			References(m => m.Artist).Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
