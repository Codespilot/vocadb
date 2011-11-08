using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumTagUsage : TagUsage {

		private Album album;

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public override IEntryBase Entry {
			get { return Album; }
		}

	}

}
