﻿using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using System;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistContract {

		private static void DoIfExists(ArchivedArtistVersion version, ArtistEditableFields field, 
			XmlCache<ArchivedArtistContract> xmlCache, Action<ArchivedArtistContract> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField != null && versionWithField.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}

		}

		public static ArchivedArtistContract GetAllProperties(ArchivedArtistVersion version) {

			var data = new ArchivedArtistContract();
			var xmlCache = new XmlCache<ArchivedArtistContract>();
			var thisVersion = xmlCache.Deserialize(version.Version, version.Data);

			data.ArtistType = thisVersion.ArtistType;
			data.BaseVoicebank = thisVersion.BaseVoicebank;
			data.Groups = thisVersion.Groups;
			data.Id = thisVersion.Id;
			data.MainPictureMime = thisVersion.MainPictureMime;
			data.Members = thisVersion.Members;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, ArtistEditableFields.Albums, xmlCache, v => data.Albums = v.Albums);
			DoIfExists(version, ArtistEditableFields.Description, xmlCache, v => data.Description = v.Description);
			DoIfExists(version, ArtistEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, ArtistEditableFields.Pictures, xmlCache, v => data.Pictures = v.Pictures);
			DoIfExists(version, ArtistEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;

		}

		public ArchivedArtistContract() {}

		public ArchivedArtistContract(Artist artist, ArtistDiff diff) {
			
			ParamIs.NotNull(() => artist);

			Albums = (diff.IncludeAlbums ? artist.Albums.Select(a => new ObjectRefContract(a.Album)).ToArray() : null);
			ArtistType = artist.ArtistType;
			BaseVoicebank = artist.BaseVoicebank != null ? new ObjectRefContract(artist.BaseVoicebank) : null;
			Id = artist.Id;
			Description = (diff.IncludeDescription ? artist.Description : null);
			Groups = artist.Groups.Select(g => new ObjectRefContract(g.Group)).ToArray();
			MainPictureMime = artist.Picture != null ? artist.Picture.Mime : null;
			Members = artist.Members.Select(m => new ObjectRefContract(m.Member)).ToArray();
			Names = (diff.IncludeNames ? artist.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null);
			Pictures = (diff.IncludePictures ? artist.Pictures.Select(p => new ArchivedEntryPictureFileContract(p)).ToArray() : null);
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			WebLinks = (diff.IncludeWebLinks ? artist.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null);

		}

		[DataMember]
		public ObjectRefContract[] Albums { get; set; }

		[DataMember]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public ObjectRefContract BaseVoicebank { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ObjectRefContract[] Groups { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string MainPictureMime { get; set; }

		[DataMember]
		public ObjectRefContract[] Members { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ArchivedEntryPictureFileContract[] Pictures { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
