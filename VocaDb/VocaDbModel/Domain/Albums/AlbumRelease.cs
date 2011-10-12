using System;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumRelease {

		private string catNum;
		private string eventName;
		private Artist publisher;

		public virtual string CatNum {
			get { return catNum; }
			set { catNum = value; }
		}

		public virtual DateTime ReleaseDate { get; set; }

		public virtual string EventName {
			get { return eventName; }
			set { eventName = value; }
		}

		public virtual Artist Publisher {
			get { return publisher; }
			set { publisher = value; }
		}

	}
}
