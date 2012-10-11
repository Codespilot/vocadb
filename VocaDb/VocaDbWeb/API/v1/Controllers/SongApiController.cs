using System;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.API.v1.Controllers
{
	public class SongApiController : Web.Controllers.ControllerBase
    {

		private const int defaultMax = 10;

		private SongService Service {
			get { return Services.Songs; }
		}

		public ActionResult ByPV(PVService service, string pvId, ContentLanguagePreference? lang) {

			var song = Service.GetSongWithPV(s => new SongForApiContract(s, lang ?? ContentLanguagePreference.Default), service, pvId);

			return Xml(song);

		}

		public ActionResult ByName(string query, ContentLanguagePreference? lang, int? start, int? maxResults, NameMatchMode? nameMatchMode) {

			var param = new SongQueryParams(query, new SongType[] {}, 0, defaultMax, false, true, NameMatchMode.Exact, SongSortRule.Name, true, false, new int[] {});

			if (start.HasValue)
				param.Paging.Start = start.Value;

			if (maxResults.HasValue)
				param.Paging.MaxEntries = Math.Min(maxResults.Value, defaultMax);

			if (nameMatchMode.HasValue)
				param.Common.NameMatchMode = nameMatchMode.Value;

			var songs = Service.Find(s => new SongForApiContract(s, lang ?? ContentLanguagePreference.Default), param);

			return Xml(songs);

		}

		public ActionResult ParsePVUrl(string pvUrl) {

			var result = VideoServiceHelper.ParseByUrl(pvUrl, true);

			if (!result.IsOk) {
				return Json(new GenericResponse<string>(false, result.Exception.Message));
			}

			var contract = new PVContract(result, PVType.Original);

			return Json(contract);

		}

    }
}
