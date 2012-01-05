using System;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.PVs {

	public class PV : IEquatable<PV> {

		public static string GetUrl(PVService service, string pvId) {

			switch (service) {
				case PVService.Youtube:
					return "http://youtu.be/" + pvId;
				case PVService.NicoNicoDouga:
					return "http://nicovideo.jp/watch/" + pvId;
				default:
					return pvId;
			}

		}

		private string name;
		private string pvId;

		public PV() {
			pvId = string.Empty;
			Name = string.Empty;
			Service = PVService.Youtube;
			PVType = PVType.Other;
		}

		public PV(PVService service, string pvId, PVType pvType, string name)
			: this() {

			Service = service;
			PVId = pvId;
			PVType = pvType;
			Name = name;

		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNull(() => value);
				name = value;
			}
		}

		public virtual string PVId {
			get { return pvId; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				pvId = value;
			}
		}

		public virtual PVService Service { get; set; }

		public virtual PVType PVType { get; set; }

		public virtual string Url {
			get {
				return GetUrl(Service, PVId);
			}
		}

		public virtual bool ContentEquals(PVContract pv) {

			if (pv == null)
				return false;

			return (Name == pv.Name);

		}

		public virtual bool Equals(PV another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as PV);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return string.Format("PV '{0}' [{1}]", PVId, Id);
		}

	}

}
