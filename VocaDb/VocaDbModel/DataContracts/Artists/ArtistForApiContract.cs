using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForApiContract : ArtistContract {

		public ArtistForApiContract() { }

		public ArtistForApiContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			CreateDate = artist.CreateDate;
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new ArtistContract(g.Group, languagePreference)).ToArray();
			Members = artist.Members.Select(m => new ArtistContract(m.Member, languagePreference)).ToArray();
			Tags = artist.Tags.Usages.Select(u => new TagUsageContract(u)).ToArray();
			WebLinks = artist.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();
			
		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ArtistContract[] Groups { get; set; }

		[DataMember]
		public ArtistContract[] Members { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
