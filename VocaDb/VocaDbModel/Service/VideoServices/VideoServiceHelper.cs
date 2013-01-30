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

		public static T GetPV<T>(T[] allPvs, PVService service) 
			where T : class, IPV {

			var servicePvs = allPvs.Where(p => p.Service == service).ToArray();

			return servicePvs.FirstOrDefault(p => p.PVType == PVType.Original)
				?? servicePvs.FirstOrDefault(p => p.PVType == PVType.Reprint)
				?? servicePvs.FirstOrDefault();

		}

		public static T GetPV<T>(T[] allPvs) where T : class, IPV {

			return allPvs.FirstOrDefault(p => p.PVType == PVType.Original)
				?? allPvs.FirstOrDefault(p => p.PVType == PVType.Reprint)
				?? allPvs.FirstOrDefault();

		}

		public static string GetThumbUrl(PVForSong pv) {

			return Services[pv.Service].GetThumbUrlById(pv.PVId);

		}

		public static string GetThumbUrl(IList<PVForSong> pvs) {

			ParamIs.NotNull(() => pvs);

			if (!pvs.Any())
				return string.Empty;

			var pv = pvs.FirstOrDefault(p => p.PVType == PVType.Original && !string.IsNullOrEmpty(p.ThumbUrl));

			if (pv == null)
				pv = pvs.FirstOrDefault(p => p.PVType == PVType.Reprint && !string.IsNullOrEmpty(p.ThumbUrl));

			if (pv == null)
				pv = pvs.FirstOrDefault(p => p.PVType == PVType.Original);

			if (pv == null)
				pv = pvs.FirstOrDefault();

			return (pv != null ? (!string.IsNullOrEmpty(pv.ThumbUrl) ? pv.ThumbUrl : GetThumbUrl(pv)) : string.Empty);

		}

		/// <summary>
		/// Gets a thumb URL, preferring a service that's not NND (because nico thumbs don't work in RSS feeds etc.)
		/// </summary>
		/// <param name="pvs">List of PVs. Cannot be null.</param>
		/// <returns>Thumb URL. Cannot be null. Can be empty if there's no PV.</returns>
		public static string GetThumbUrlPreferNotNico(IList<PVForSong> pvs) {

			ParamIs.NotNull(() => pvs);

			if (!pvs.Any())
				return string.Empty;

			var notNico = pvs.Where(p => p.Service != PVService.NicoNicoDouga).ToArray();

			var pv = notNico.FirstOrDefault(p => !string.IsNullOrEmpty(p.ThumbUrl) && (p.PVType == PVType.Original || p.PVType == PVType.Reprint));

			if (pv == null)
				pv = notNico.FirstOrDefault(p => !string.IsNullOrEmpty(p.ThumbUrl));

			if (pv == null)
				pv = pvs.FirstOrDefault(p => !string.IsNullOrEmpty(p.ThumbUrl));

			if (pv == null)
				pv = pvs.FirstOrDefault();

			return (pv != null ? (!string.IsNullOrEmpty(pv.ThumbUrl) ? pv.ThumbUrl : GetThumbUrl(pv)) : string.Empty);

		}

		public static T PrimaryPV<T>(IEnumerable<T> pvs, PVService? preferredService = null) 
			where T : class, IPV {

			ParamIs.NotNull(() => pvs);

			var p = pvs.ToArray();

			if (preferredService.HasValue)
				return GetPV(p, preferredService.Value) ?? GetPV(p);
			else
				return GetPV(p);

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
