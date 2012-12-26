using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.Song {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<SongWithAlbumAndPVsContract> result, string filter, NameMatchMode nameMatchMode, SongType songType, string timeFilter, bool onlyWithPVs, 
			SongSortRule sortRule, SongViewMode viewMode, 
			bool draftsOnly, int page, int pageSize, IndexRouteParams routeParams) {

			DraftsOnly = draftsOnly;
			Songs = new StaticPagedList<SongWithAlbumAndPVsContract>(result.Items, 
				page, pageSize, result.TotalCount);
			Filter = filter;
			NameMatchMode = nameMatchMode;
			SongType = songType;
			Since = timeFilter;
			OnlyWithPVs = onlyWithPVs;
			SortRule = sortRule;
			ViewMode = viewMode;
			RouteParams = routeParams;

		}

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public Dictionary<SongType, string> FilterableArtistTypes { get; set; }

		public NameMatchMode NameMatchMode { get; set; }

		public bool OnlyWithPVs { get; set; }

		public IndexRouteParams RouteParams { get; set; }

		public string Since { get; set; }

		public IPagedList<SongWithAlbumAndPVsContract> Songs { get; set; }

		public SongType SongType { get; set; }

		public SongSortRule SortRule { get; set; }

		public SongViewMode ViewMode { get; set; }

		public IndexRouteParams CreateRouteParams(int page) {
			return new IndexRouteParams(RouteParams, page);
		}

	}

	/// <summary>
	/// Parameter collection given to index action.
	/// </summary>
	public class IndexRouteParams {

		public IndexRouteParams() {}

		public IndexRouteParams(IndexRouteParams index, int? page)
			: this() {

			ParamIs.NotNull(() => index);

			draftsOnly = index.draftsOnly;
			filter = index.filter;
			matchMode = index.matchMode;
			onlyWithPVs = index.onlyWithPVs;
			pageSize = index.pageSize;
			since = index.since;
			songType = index.songType;
			sort = index.sort;
			view = index.view;
			this.page = page;

		}

		public bool? draftsOnly { get; set; }

		public string filter { get; set; }

		public NameMatchMode? matchMode { get; set; }

		public bool? onlyWithPVs { get; set; }

		public int? page { get; set; }

		public int? pageSize { get; set; }

		public string since { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public SongType? songType { get; set; }

		public SongSortRule? sort { get; set; }

		public SongViewMode? view { get; set; }

	}

	public enum SongViewMode {

		Details,

		Preview

	}

}