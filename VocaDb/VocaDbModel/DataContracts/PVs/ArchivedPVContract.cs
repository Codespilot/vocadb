﻿using System.Runtime.Serialization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.PVs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedPVContract {

		public ArchivedPVContract() {
			Author = ThumbUrl = string.Empty;
		}

		public ArchivedPVContract(PV pv)
			: this() {

			ParamIs.NotNull(() => pv);

			Author = pv.Author;
			Name = pv.Name;
			PVId = pv.PVId;
			Service = pv.Service;
			PVType = pv.PVType;

		}

		public ArchivedPVContract(PVForSong pv)
			: this((PV)pv) {

			Length = pv.Length;
			ThumbUrl = pv.ThumbUrl;

		}

		[DataMember]
		public string Author { get; set; }

		[DataMember]
		public int Length { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		public PVService Service { get; set; }

		[DataMember]
		public PVType PVType { get; set; }

		[DataMember]
		public string ThumbUrl { get; set; }

	}

}
