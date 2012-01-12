using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Songs {

	public class SongInList {

		private SongList list;
		private Song song;

		public SongInList() {}

		public SongInList(Song song, SongList list, int order) {

			Song = song;
			List = list;
			Order = order;

		}

		public virtual int Id { get; set; }

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual SongList List {
			get { return list; }
			set {
				ParamIs.NotNull(() => value);
				list = value;
			}
		}

		public virtual int Order { get; set; }

		public virtual void ChangeSong(Song target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.ListLinks.Remove(this);
			target.ListLinks.Add(this);
			Song = target;

		}

		public virtual bool Equals(SongInList another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public virtual void Delete() {

			List.AllSongs.Remove(this);
			Song.ListLinks.Remove(this);

		}

		public override bool Equals(object obj) {
			return Equals(obj as SongInList);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return Song + " in " + List;
		}

	}

}
