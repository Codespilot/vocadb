using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class SongList : IEntryBase {

		private User author;
		private string description;
		private string name;
		private IList<SongInList> songs = new List<SongInList>();

		public SongList() {
			Description = string.Empty;
		}

		public SongList(string name, User author) {
			Name = name;
			Author = author;
		}

		public virtual IList<SongInList> AllSongs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual User Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual EntryType EntryType {
			get {
				return EntryType.SongList;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrWhiteSpace(() => value);
				name = value;
			}
		}

		public virtual IEnumerable<SongInList> SongLinks {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		public override string ToString() {
			return "song list '" + Name + "' [" + Id + "]";
		}

	}
}
