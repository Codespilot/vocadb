using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Domain.Artists;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.Song {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<SongWithAlbumContract> result, string filter, SongType songType, SongSortRule sortRule, SongViewMode viewMode, bool? draftsOnly, int? page) {

			DraftsOnly = draftsOnly ?? false;
			Songs = new StaticPagedList<SongWithAlbumContract>(result.Items, 
				page ?? 1, 30, result.TotalCount);
			Filter = filter;
			SongType = songType;
			SortRule = sortRule;
			ViewMode = viewMode;

		}

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public Dictionary<SongType, string> FilterableArtistTypes { get; set; }

		public IPagedList<SongWithAlbumContract> Songs { get; set; }

		public SongType SongType { get; set; }

		public SongSortRule SortRule { get; set; }

		public SongViewMode ViewMode { get; set; }

	}

	public enum SongViewMode {

		Details,

		Preview

	}

}