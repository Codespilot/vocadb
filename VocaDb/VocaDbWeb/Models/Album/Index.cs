using System.ComponentModel.DataAnnotations;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.Albums;
using PagedList;

namespace VocaDb.Web.Models.Album {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<AlbumWithAdditionalNamesContract> result, string filter, DiscType discType, 
			AlbumSortRule sortRule, EntryViewMode view, int? page, bool? draftsOnly, IndexRouteParams routeParams) {

			Page = page ?? 1;
			Albums = new StaticPagedList<AlbumWithAdditionalNamesContract>(result.Items, 
				Page, 30, result.TotalCount);
			DiscType = discType;
			DraftsOnly = draftsOnly ?? false;
			Filter = filter;
			Sort = sortRule;
			View = view;
			RouteParams = routeParams;

		}

		public IPagedList<AlbumWithAdditionalNamesContract> Albums { get; set; }

		public DiscType DiscType { get; set; }

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public int Page { get; set; }

		public IndexRouteParams RouteParams { get; set; }

		public AlbumSortRule Sort { get; set; }

		public EntryViewMode View { get; set; }

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
			discType = (source.discType != DiscType.Unknown ? source.discType : null);
			draftsOnly = (source.draftsOnly == true ? source.draftsOnly : null);
			filter = source.filter;
			sort = source.sort;
			view = source.view;
			this.page = page;
		}

		public DiscType? discType { get; set; }
		public bool? draftsOnly { get; set; }
		public string filter { get; set; }
		public int? page { get; set; }
		public AlbumSortRule? sort { get; set; }
		public EntryViewMode? view { get; set; }

	}

}