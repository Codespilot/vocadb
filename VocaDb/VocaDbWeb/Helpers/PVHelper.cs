using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Helpers {

	public static class PVHelper {

		public static string GetNicoId(IEnumerable<PVContract> pvs, string nicoId) {

			var nicoPV = pvs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga && p.PVType == PVType.Original);

			if (nicoPV != null)
				return nicoPV.PVId;

			return nicoId;

		}

		public static PVContract PrimaryPV(IEnumerable<PVContract> pvs) {

			var preferredService = MvcApplication.LoginManager.IsLoggedIn ? (PVService?)MvcApplication.LoginManager.LoggedUser.PreferredVideoService : null;

			return VideoServiceHelper.PrimaryPV(pvs, preferredService);

		}

	}
}