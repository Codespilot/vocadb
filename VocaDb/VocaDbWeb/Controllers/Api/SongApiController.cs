using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/songs")]
	public class SongApiController : ApiController {

		private readonly SongService service;

		public SongApiController(SongService service) {
			this.service = service;
		}

		[Flags]
		public enum SongOptionalFields {

			None = 0,
			Albums = 1,
			Artists = 2,
			Names = 4,
			PVs = 8,
			Tags = 16,
			ThumbUrl = 32,
			WebLinks = 64

		}

		[Route("{id:int}")]
		public SongForApiContract GetById(int id, SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var song = service.GetSongWithMergeRecord(id, (s, m) => new SongForApiContract(s, m, lang, 
					fields.HasFlag(SongOptionalFields.Albums),
					fields.HasFlag(SongOptionalFields.Artists), 
					fields.HasFlag(SongOptionalFields.Names), 
					fields.HasFlag(SongOptionalFields.PVs),
					fields.HasFlag(SongOptionalFields.Tags), 
					fields.HasFlag(SongOptionalFields.ThumbUrl),
					fields.HasFlag(SongOptionalFields.WebLinks)));

			return song;

		}

		[Route("")]
		public PartialFindResult<SongForApiContract> GetByName(string query, SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return new PartialFindResult<SongForApiContract>(new SongForApiContract[0], 0);

		}

		[Route("")]
		public SongForApiContract GetByPV(PVService pvService, string pvId, SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var song = service.GetSongWithPV(s => 
				new SongForApiContract(s, null, lang, 
					fields.HasFlag(SongOptionalFields.Albums),
					fields.HasFlag(SongOptionalFields.Artists), 
					fields.HasFlag(SongOptionalFields.Names), 
					fields.HasFlag(SongOptionalFields.PVs),
					fields.HasFlag(SongOptionalFields.Tags), 
					fields.HasFlag(SongOptionalFields.ThumbUrl),
					fields.HasFlag(SongOptionalFields.WebLinks)), 
				pvService, pvId);

			return song;
			//return new PartialFindResult<SongForApiContract>(new [] { song }, 1);

		}

	}

}