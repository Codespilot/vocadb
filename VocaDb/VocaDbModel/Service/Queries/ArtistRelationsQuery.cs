using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public class ArtistRelationsQuery {

		private readonly IRepositoryContext<Artist> ctx;

		public ArtistRelationsQuery(IRepositoryContext<Artist> ctx) {
			this.ctx = ctx;
		}

		/*public ArtistRelationsForApi GetRelations(Artist artist) {
			


		}*/

	}

}
