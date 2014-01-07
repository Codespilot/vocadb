using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForApiContract {

		public ArtistForApiContract() { }

		public ArtistForApiContract(Artist artist, ContentLanguagePreference languagePreference, ArtistOptionalFields includedFields) {

			ArtistType = artist.ArtistType;
			CreateDate = artist.CreateDate;
			DefaultName = artist.DefaultName;
			DefaultNameLanguage = artist.Names.SortNames.DefaultLanguage;
			Id = artist.Id;
			PictureMime = artist.Picture != null ? artist.Picture.Mime : null;
			Status = artist.Status;
			Version = artist.Version;

			if (languagePreference != ContentLanguagePreference.Default)
				LocalizedName = artist.Names.SortNames[languagePreference];

			if (includedFields.HasFlag(ArtistOptionalFields.Description))
				Description = artist.Description;

			if (includedFields.HasFlag(ArtistOptionalFields.Groups))
				Groups = artist.Groups.Select(g => new ArtistContract(g.Group, languagePreference)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.Members))
				Members = artist.Members.Select(m => new ArtistContract(m.Member, languagePreference)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.Names))
				Names = artist.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.Tags))
				Tags = artist.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.WebLinks))
				WebLinks = artist.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();

		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string DefaultName { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArtistContract[] Groups { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string LocalizedName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArtistContract[] Members { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int MergedTo { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public string PictureMime { get; set;}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
