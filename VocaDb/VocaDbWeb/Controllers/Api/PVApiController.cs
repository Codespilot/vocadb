using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/pvs")]
	[ApiExplorerSettings(IgnoreApi=true)]
	public class PVApiController : ApiController {

		[Route("")]
		public PVContract GetPVByUrl(string pvUrl) {

			if (string.IsNullOrEmpty(pvUrl))
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			var result = VideoServiceHelper.ParseByUrl(pvUrl, true);

			if (!result.IsOk) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = result.Exception.Message });
			}

			var contract = new PVContract(result, PVType.Original);
			return contract;

		}

	}

}