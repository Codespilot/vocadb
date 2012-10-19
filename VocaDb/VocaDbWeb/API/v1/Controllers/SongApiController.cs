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
using VocaDb.Web.Controllers;

namespace VocaDb.Web.API.v1.Controllers
{
	public class SongApiController : Web.Controllers.ControllerBase
    {

		private const int defaultMax = 10;

		private SongService Service {
			get { return Services.Songs; }
		}

		public ActionResult ByPV(PVService service, string pvId, ContentLanguagePreference? lang, string callback, 
			DataFormat format = DataFormat.Auto) {

			var song = Service.GetSongWithPV(s => new SongForApiContract(s, lang ?? ContentLanguagePreference.Default), service, pvId);

			return Object(song, format, callback);

		}

		public ActionResult ByPVBase(PVService service, string pvId, string callback,
			DataFormat format = DataFormat.Auto) {

			var song = Service.GetSongWithPV(s => new EntryBaseContract(s), service, pvId);

			return Object(song, format, callback);

		}

		public ActionResult ByName(string query, ContentLanguagePreference? lang, int? start, int? maxResults, NameMatchMode? nameMatchMode,
			string callback, DataFormat format = DataFormat.Auto) {

			var param = new SongQueryParams(query, new SongType[] {}, 0, defaultMax, false, true, NameMatchMode.Exact, SongSortRule.Name, true, false, new int[] {});

			if (start.HasValue)
				param.Paging.Start = start.Value;

			if (maxResults.HasValue)
				param.Paging.MaxEntries = Math.Min(maxResults.Value, defaultMax);

			if (nameMatchMode.HasValue)
				param.Common.NameMatchMode = nameMatchMode.Value;

			var songs = Service.Find(s => new SongForApiContract(s, lang ?? ContentLanguagePreference.Default), param);

			return Object(songs, format, callback);

		}

		public ActionResult ParsePVUrl(string pvUrl, string callback, DataFormat format = DataFormat.Auto) {

			var result = VideoServiceHelper.ParseByUrl(pvUrl, true);

			if (!result.IsOk) {
				return Json(new GenericResponse<string>(false, result.Exception.Message));
			}

			var contract = new PVContract(result, PVType.Original);

			return Object(contract, format, callback);

		}

    }
}
