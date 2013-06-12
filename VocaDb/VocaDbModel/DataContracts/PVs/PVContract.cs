﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.PVs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PVContract : IPV {

		public PVContract() { }

		public PVContract(PV pv) {

			ParamIs.NotNull(() => pv);

			Author = pv.Author;
			Id = pv.Id;
			Name = pv.Name;
			PVId = pv.PVId;
			Service = pv.Service;
			PVType = pv.PVType;
			Url = pv.Url;

		}

		public PVContract(ArchivedPVContract contract) {

			ParamIs.NotNull(() => contract);

			Author = contract.Author;
			Length = contract.Length;
			Name = contract.Name;
			PVId = contract.PVId;
			Service = contract.Service;
			PVType = contract.PVType;

		}

		public PVContract(PVForSong pv)
			: this((PV)pv) {

			Length = pv.Length;
			ThumbUrl = pv.ThumbUrl;

		}

		public PVContract(VideoUrlParseResult parseResult, PVType type) {

			ParamIs.NotNull(() => parseResult);

			Author = parseResult.Author;
			Length = parseResult.LengthSeconds ?? 0;
			Name = parseResult.Title;
			PVId = parseResult.Id;
			Service = parseResult.Service;
			ThumbUrl = parseResult.ThumbUrl;
			PVType = type;

			Url = PV.GetUrl(Service, PVId);
			
		}

		[DataMember]
		public string Author { get; set; }

		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Length in seconds, 0 if not specified.
		/// </summary>
		[DataMember]
		public int Length { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public PVService Service { get; set; }

		[DataMember]
		public PVType PVType { get; set; }

		[DataMember]
		public string ThumbUrl { get; set; }

		[DataMember]
		public string Url { get; set; }

		/// <summary>
		/// Compares editable properties.
		/// </summary>
		/// <param name="pv">Contract to be compared to. Can be null.</param>
		/// <returns>True if the editable properties of this contract are the same as the one being compared to.</returns>
		public bool ContentEquals(PVContract pv) {

			if (pv == null)
				return false;

			return (Name == pv.Name);

		}

		public PVContract NullToEmpty() {
			Author = Author ?? string.Empty;
			Name = Name ?? string.Empty;
			ThumbUrl = ThumbUrl ?? string.Empty;
			return this;
		}

	}

}
