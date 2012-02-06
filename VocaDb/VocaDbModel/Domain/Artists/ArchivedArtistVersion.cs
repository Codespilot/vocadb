﻿using System.Xml.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Domain.Artists {

	public class ArchivedArtistVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<ArtistEditableFields> {

		public static ArchivedArtistVersion Create(Artist artist, ArtistDiff diff, AgentLoginData author, ArtistArchiveReason reason, string notes) {

			var contract = new ArchivedArtistContract(artist, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return artist.CreateArchivedVersion(data, diff, author, reason, notes);

		}

		private Artist artist;
		private ArtistDiff diff;

		public ArchivedArtistVersion() {}

		public ArchivedArtistVersion(Artist artist, XDocument data, ArtistDiff diff, AgentLoginData author, int version, EntryStatus status, 
			ArtistArchiveReason reason, string notes)
			: base(data, author, version, status, notes) {

			Artist = artist;
			Diff = diff;
			Reason = reason;

			if (diff.IncludePicture)
				Picture = artist.Picture;

		}

		public virtual Artist Artist {
			get { return artist; }
			protected set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual ArtistDiff Diff {
			get { return diff; }
			protected set { diff = value; }
		}

		public override IEntryDiff DiffBase {
			get { return Diff; }
		}

		public override EntryEditEvent EditEvent {
			get {
				return (Reason == ArtistArchiveReason.Created ? EntryEditEvent.Created : EntryEditEvent.Updated);
			}
		}

		public override IEntryWithNames EntryBase {
			get { return Artist; }
		}

		public virtual PictureData Picture { get; set; }

		public virtual ArtistArchiveReason Reason { get; set; }

		public virtual ArchivedArtistVersion GetLatestVersionWithField(ArtistEditableFields field) {

			if (IsIncluded(field))
				return this;

			return Artist.ArchivedVersionsManager.GetLatestVersionWithField(field, Version);

		}

		public virtual bool IsIncluded(ArtistEditableFields field) {
			return (Diff != null && Data != null && Diff.IsIncluded(field));
		}

	}
}
