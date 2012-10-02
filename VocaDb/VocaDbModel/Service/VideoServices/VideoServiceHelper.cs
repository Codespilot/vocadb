using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public static class VideoServiceHelper {

		private static readonly VideoService[] services = new[] { 
			VideoService.NicoNicoDouga,
			VideoService.SoundCloud,
			VideoService.Youtube,
			VideoService.Vimeo
		};

		public static readonly Dictionary<PVService, VideoService> Services = services.ToDictionary(s => s.Service);

		public static string GetThumbUrl(PVForSong pv) {

			return Services[pv.Service].GetThumbUrlById(pv.PVId);

		}

		public static string GetThumbUrl(IEnumerable<PVForSong> pvs) {

			var pv = pvs.FirstOrDefault(p => p.PVType == PVType.Original && !string.IsNullOrEmpty(p.ThumbUrl));
			if (pv == null)
				pv = pvs.FirstOrDefault(p => p.PVType == PVType.Original);
			if (pv == null)
				pv = pvs.FirstOrDefault();

			return (pv != null ? (!string.IsNullOrEmpty(pv.ThumbUrl) ? pv.ThumbUrl : GetThumbUrl(pv)) : string.Empty);

		}

		public static PVContract PrimaryPV(IEnumerable<PVContract> pvs, PVService? preferredService = null) {

			ParamIs.NotNull(() => pvs);

			PVContract primaryPv = null;
			var originalPVs = pvs.Where(p => p.PVType != PVType.Other);
			var otherPVs = pvs.Where(p => p.PVType == PVType.Other);

			if (preferredService != null) {

				primaryPv = originalPVs.FirstOrDefault(p => p.Service == preferredService);

				if (primaryPv == null)
					primaryPv = otherPVs.FirstOrDefault(p => p.Service == preferredService);

			}

			if (primaryPv == null)
				primaryPv = originalPVs.FirstOrDefault();

			if (primaryPv == null)
				primaryPv = otherPVs.FirstOrDefault();

			return primaryPv;

		}

		public static PV PrimaryPV(IEnumerable<PV> pvs, PVService? preferredService = null) {

			ParamIs.NotNull(() => pvs);

			PV primaryPv = null;
			var originalPVs = pvs.Where(p => p.PVType != PVType.Other);
			var otherPVs = pvs.Where(p => p.PVType == PVType.Other);

			if (preferredService != null) {

				primaryPv = originalPVs.FirstOrDefault(p => p.Service == preferredService);

				if (primaryPv == null)
					primaryPv = otherPVs.FirstOrDefault(p => p.Service == preferredService);

			}

			if (primaryPv == null)
				primaryPv = originalPVs.FirstOrDefault();

			if (primaryPv == null)
				primaryPv = otherPVs.FirstOrDefault();

			return primaryPv;

		}

		public static VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			var service = services.FirstOrDefault(s => s.IsValidFor(url));

			if (service == null) {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher);
			}

			return service.ParseByUrl(url, getTitle);

		}

	}
}
