using System.Web.Http;
using System.Web.Http.Cors;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for users.
	/// </summary>
	[RoutePrefix("api/users")]
	public class UserApiController : ApiController {

		private readonly IUserPermissionContext permissionContext;
		private readonly UserService service;

		public UserApiController(UserService service, IUserPermissionContext permissionContext) {
			this.service = service;
			this.permissionContext = permissionContext;
		}

		/// <summary>
		/// Add or update rating for a specific song, for the currently logged in user.
		/// If the user has already rated the song, any previous rating is replaced.
		/// Authorization cookie must be included.
		/// This API supports CORS.
		/// </summary>
		/// <param name="songId">ID of the song to be rated.</param>
		/// <param name="rating">Rating to be given. Possible values are Nothing, Like, Favorite.</param>
		/// <returns>The string "OK" if successful.</returns>
		[Route("current/ratedSongs/{songId:int}")]
		[Authorize]
		[EnableCors(origins: "*", headers: "*", methods: "post", SupportsCredentials = true)]
		public string PostSongRating(int songId, SongVoteRating rating) {
			
			service.UpdateSongRating(permissionContext.LoggedUserId, songId, rating);
			return "OK";

		}

	}
}