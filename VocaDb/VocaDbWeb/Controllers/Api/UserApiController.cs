using System.Web.Http;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/users")]
	public class UserApiController : ApiController {

		private readonly IUserPermissionContext permissionContext;
		private readonly UserService service;

		public UserApiController(UserService service, IUserPermissionContext permissionContext) {
			this.service = service;
			this.permissionContext = permissionContext;
		}

		[Route("current/ratedSongs/{songId:int}")]
		[Authorize]
		public string PostRating(int songId, SongVoteRating rating) {
			
			service.UpdateSongRating(permissionContext.LoggedUserId, songId, rating);
			return "OK";

		}

	}
}