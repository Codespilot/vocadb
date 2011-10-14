using System;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumRelease {

		private string catNum;
		private string eventName;
		private Artist publisher;

		public AlbumRelease() {}

		public AlbumRelease(AlbumReleaseContract contract, Artist label = null) {
			
			ParamIs.NotNull(() => contract);

			CatNum = contract.CatNum;
			ReleaseDate = (contract.ReleaseDate != null ? OptionalDateTime.Create(contract.ReleaseDate) : null);
			EventName = contract.EventName;
			Label = label;

		}

		public virtual string CatNum {
			get { return catNum; }
			set { catNum = value; }
		}

		public virtual OptionalDateTime ReleaseDate { get; set; }

		public virtual string EventName {
			get { return eventName; }
			set { eventName = value; }
		}

		public virtual Artist Label {
			get { return publisher; }
			set { publisher = value; }
		}

	}
}
