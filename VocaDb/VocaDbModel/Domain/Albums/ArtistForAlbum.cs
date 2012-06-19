using System;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Albums {

	public class ArtistForAlbum : IArtistWithSupport, IEquatable<ArtistForAlbum> {

		private Artist artist;
		private Album album;
		private string notes;

		public ArtistForAlbum() {
			IsSupport = false;
			Notes = string.Empty;
			Roles = ArtistRoles.Default;
		}

		public ArtistForAlbum(Album album, Artist artist, bool support, ArtistRoles roles)
			: this() {

			Album = album;
			Artist = artist;
			IsSupport = support;
			Roles = roles;

		}

		public ArtistForAlbum(Album album, string name, bool support, ArtistRoles roles)
			: this() {

			Album = album;
			IsSupport = support;
			Name = name;
			Roles = roles;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				//ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual ArtistCategories ArtistCategories {
			get {
				return ArtistHelper.GetCategories(this);
			}
		}

		public virtual int Id { get; set; }

		public virtual bool IsSupport { get; set; }

		public virtual string Name { get; set; }

		public virtual string Notes {
			get { return notes; }
			set {
				ParamIs.NotNull(() => value);
				notes = value; 
			}
		}

		public virtual ArtistRoles Roles { get; set; }

		public virtual bool ArtistLinkEquals(ArtistForAlbum another) {

			if (another == null)
				return false;

			return ((Artist != null && Artist.Equals(another.Artist)) || (Artist == null && another.Artist == null && Name == another.Name));

		}

		/*public virtual bool ContentEquals(ArtistForAlbumContract contract) {

			if (contract == null)
				return false;

			return (IsSupport == contract.IsSupport && Roles == contract.Roles);

		}*/

		public virtual void Delete() {
			Album.DeleteArtistForAlbum(this);
		}

		public virtual bool Equals(ArtistForAlbum another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistForAlbum);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Album target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Album.AllArtists.Remove(this);
			Album = target;
			target.AllArtists.Add(this);

		}

		public virtual void Move(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Artist.AllAlbums.Remove(this);
			Artist = target;
			target.AllAlbums.Add(this);

		}

		public override string ToString() {
			return string.Format("{0} for {1}", Artist, Album);
		}

	}
}
