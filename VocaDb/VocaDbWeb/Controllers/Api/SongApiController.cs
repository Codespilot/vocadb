using System;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for songs
	/// </summary>
	[RoutePrefix("api/songs")]
	public class SongApiController : ApiController {

		private readonly SongService service;

		public SongApiController(SongService service) {
			this.service = service;
		}

		/// <summary>
		/// Gets a song by Id.
		/// </summary>
		/// <param name="id">Song Id (required).</param>
		/// <param name="fields">Optional fields (optional). Possible values are albums, artists, names, pvs, tags, thumbUrl, webLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>http://vocadb.net/api/songs/121</example>
		/// <returns>Song data.</returns>
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
		[ApiExplorerSettings(IgnoreApi=true)]
		public PartialFindResult<SongForApiContract> GetByName(string query, SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return new PartialFindResult<SongForApiContract>(new SongForApiContract[0], 0);

		}

		/// <summary>
		/// Gets a song by PV.
		/// </summary>
		/// <param name="pvService">PV service (required).</param>
		/// <param name="pvId">PV Id (required).</param>
		/// <param name="fields">Optional fields (optional).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Song data.</returns>
		/// <example>http://vocadb.net/api/songs?pvId=sm19923781&amp;pvService=NicoNicoDouga</example>
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

}