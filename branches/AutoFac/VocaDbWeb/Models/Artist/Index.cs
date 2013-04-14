using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Artist {

	public class ArtistIndex {

		public ArtistIndex() { }

		public ArtistIndex(PartialFindResult<ArtistWithAdditionalNamesContract> result, string filter,
			ArtistType artistType, bool? draftsOnly, ArtistSortRule sort, int? page, IndexRouteParams routeParams) {

			Artists = new StaticPagedList<ArtistWithAdditionalNamesContract>(result.Items, page ?? 1, 30, result.TotalCount);
			DraftsOnly = draftsOnly ?? false;
			Filter = filter;
			ArtistType = artistType;
			Sort = sort;
			RouteParams = routeParams;

			FilterableArtistTypes = EnumVal<ArtistType>.Values.ToDictionary(a => a, Translate.ArtistTypeName);

		}

		public IPagedList<ArtistWithAdditionalNamesContract> Artists { get; set; }

		public ArtistType ArtistType { get; set; }

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public Dictionary<ArtistType, string> FilterableArtistTypes { get; set; }

		public IndexRouteParams RouteParams { get; set; }

		public ArtistSortRule Sort { get; set; }

		public IndexRouteParams CreateRouteParams(int page) {
			return new IndexRouteParams(RouteParams, page);
		}

	}

	/// <summary>
	/// Parameter collection given to index action.
	/// </summary>
	public class IndexRouteParams {

		public IndexRouteParams() { }

		public IndexRouteParams(IndexRouteParams source, int? page) {

			ParamIs.NotNull(() => source);

			artistType = (source.artistType != ArtistType.Unknown ? source.artistType : null);
			draftsOnly = (source.draftsOnly == true ? source.draftsOnly : null);
			filter = source.filter;
			matchMode = source.matchMode;
			sort = source.sort;
			this.page = page;

		}

		public ArtistType? artistType { get; set; }
		public bool? draftsOnly { get; set; }
		public string filter { get; set; }
		public NameMatchMode? matchMode { get; set; }
		public int? page { get; set; }
		public ArtistSortRule? sort { get; set; }

	}

}