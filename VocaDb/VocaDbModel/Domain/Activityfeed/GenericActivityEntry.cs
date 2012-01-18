using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Activityfeed {

	public abstract class GenericActivityEntry<T> : ActivityEntry where T : class, IEntryBase, IEntryWithNames {

		private T entry;

		public GenericActivityEntry() { }

		public GenericActivityEntry(T entry, EntryEditEvent editEvent, User author)
			: base(author, editEvent) {

			Entry = entry;
			EditEvent = editEvent;

		}

		public virtual T Entry {
			get { return entry; }
			set {
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public override IEntryBase EntryBase {
			get { return Entry; }
		}

		public override INameManager EntryNames {
			get { return Entry.Names; }
		}

		public override EntryType EntryType {
			get { return Entry.EntryType; }
		}

	}

	public class AlbumActivityEntry : GenericActivityEntry<Album> {

		public AlbumActivityEntry() { }

		public AlbumActivityEntry(Album album, EntryEditEvent editEvent, User author)
			: base(album, editEvent, author) { }

		public override void Accept(IActivityEntryVisitor visitor) {
			visitor.Visit(this);
		}

	}

	public class ArtistActivityEntry : GenericActivityEntry<Artist> {

		public ArtistActivityEntry() { }

		public ArtistActivityEntry(Artist artist, EntryEditEvent editEvent, User author)
			: base(artist, editEvent, author) { }

		public override void Accept(IActivityEntryVisitor visitor) {
			visitor.Visit(this);
		}

	}

	public class SongActivityEntry : GenericActivityEntry<Song> {

		public SongActivityEntry() { }

		public SongActivityEntry(Song song, EntryEditEvent editEvent, User author)
			: base(song, editEvent, author) { }

		public override void Accept(IActivityEntryVisitor visitor) {
			visitor.Visit(this);
		}

	}

}
