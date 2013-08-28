using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Album"/>.
	/// </summary>
	public class AlbumQueries {

		private readonly IUserPermissionContext permissionContext;
		private readonly IAlbumRepository repository;

		public AlbumQueries(IAlbumRepository repository, IUserPermissionContext permissionContext) {
			this.repository = repository;
			this.permissionContext = permissionContext;
		}

		public AlbumContract[] GetRelatedAlbums(int albumId) {

			return repository.HandleQuery(ctx => {

				var album = ctx.Load(albumId);
				var q = new RelatedAlbumsQuery(ctx);
				var albums = q.GetRelatedAlbums(album);

				return albums.ArtistMatches
					.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
					.OrderBy(a => a.Name)
					.Concat(albums.TagMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name))
					.ToArray();

			});

		}

	}
}