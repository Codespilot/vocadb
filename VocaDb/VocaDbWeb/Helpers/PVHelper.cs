using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Web.Helpers {

	public static class PVHelper {

		public static string GetNicoId(IEnumerable<PVContract> pvs, string nicoId) {

			var nicoPV = pvs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga && p.PVType == PVType.Original);

			if (nicoPV != null)
				return nicoPV.PVId;

			return nicoId;

		}

		public static PVContract PrimaryPV(IEnumerable<PVContract> pvs) {

			ParamIs.NotNull(() => pvs);

			PVContract primaryPv = null;
			var originalPVs = pvs.Where(p => p.PVType != PVType.Other);
			var otherPVs = pvs.Where(p => p.PVType == PVType.Other);

			if (MvcApplication.LoginManager.IsLoggedIn) {

				primaryPv = originalPVs.FirstOrDefault(p => p.Service == MvcApplication.LoginManager.LoggedUser.PreferredVideoService);

				if (primaryPv == null)
					primaryPv = otherPVs.FirstOrDefault(p => p.Service == MvcApplication.LoginManager.LoggedUser.PreferredVideoService);

			}

			if (primaryPv == null)
				primaryPv = originalPVs.FirstOrDefault();

			if (primaryPv == null)
				primaryPv = otherPVs.FirstOrDefault();

			return primaryPv;

		}

	}
}