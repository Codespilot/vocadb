﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedWebLinkContract : IWebLink {

		public ArchivedWebLinkContract() {
			Category = WebLinkCategory.Other;
		}

		public ArchivedWebLinkContract(WebLink webLink) {
			
			ParamIs.NotNull(() => webLink);

			Category = webLink.Category;
			Description = webLink.Description;
			Url = webLink.Url;

		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public WebLinkCategory Category { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Url { get; set; }

	}
}
