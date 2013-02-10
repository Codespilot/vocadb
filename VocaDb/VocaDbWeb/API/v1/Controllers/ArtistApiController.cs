using System;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.Artist;
using VocaDb.Web.Controllers;

namespace VocaDb.Web.API.v1.Controllers {
	public class ArtistApiController : Web.Controllers.ControllerBase {

		private const int defaultMax = 10;

		private ArtistService Service {
			get { return Services.Artists; }
		}

		public ActionResult ByName(string query, ContentLanguagePreference? lang, int? start, int? maxResults, NameMatchMode? nameMatchMode,
			string callback, DataFormat format = DataFormat.Auto) {

			if (string.IsNullOrEmpty(query))
				return Object(new PartialFindResult<SongForApiContract>(), format, callback);

			var param = new ArtistQueryParams(query, new ArtistType[] { }, 0, defaultMax, false, true, NameMatchMode.Exact, ArtistSortRule.Name, false);

			if (start.HasValue)
				param.Paging.Start = start.Value;

			if (maxResults.HasValue)
				param.Paging.MaxEntries = Math.Min(maxResults.Value, defaultMax);

			if (nameMatchMode.HasValue)
				param.Common.NameMatchMode = nameMatchMode.Value;

			var songs = Service.FindArtists(s => new ArtistForApiContract(s, lang ?? ContentLanguagePreference.Default), param);

			return Object(songs, format, callback);

		}

	}
}