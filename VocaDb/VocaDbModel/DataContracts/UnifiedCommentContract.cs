﻿using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts {

	public class UnifiedCommentContract : CommentContract {

		private string GetArtistString(IEntryBase entry, ContentLanguagePreference languagePreference) {

			if (entry is Album)
				return ((Album)entry).ArtistString[languagePreference];
			else if (entry is Song)
				return ((Song)entry).ArtistString[languagePreference];
			else
				return null;

		}

		private string GetMime(IEntryBase entry) {

			var album = entry as Album;
			if (album != null && album.CoverPictureData != null)
				return album.CoverPictureData.Mime;

			var artist = entry as Artist;
			if (artist != null && artist.Picture != null)
				return artist.Picture.Mime;

			return string.Empty;

		}

		private string GetSongThumbUrl(IEntryBase entry) {
			return (entry is Song ? VideoServiceHelper.GetThumbUrl(((Song)entry).PVs.PVs) : string.Empty);
		}

		public UnifiedCommentContract() { }

		public UnifiedCommentContract(Comment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Entry = new EntryWithImageContract(comment.Entry, GetMime(comment.Entry), languagePreference);
			ArtistString = GetArtistString(comment.Entry, languagePreference);
			SongThumbUrl = GetSongThumbUrl(comment.Entry);

		}

		public string ArtistString { get; set; }

		public EntryWithImageContract Entry { get; set; }

		public string SongThumbUrl { get; set; }

	}

}
