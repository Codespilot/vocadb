﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain {

	public class EntryPictureFileManager<T> : IEnumerable<T> where T : EntryPictureFile {

		private IList<T> pictures = new List<T>();

		public virtual IList<T> Pictures {
			get { return pictures; }
			set {
				ParamIs.NotNull(() => value);
				pictures = value;
			}
		}

		public virtual void Add(T pic) {
			pictures.Add(pic);
		}

		public virtual CollectionDiffWithValue<T, T> SyncPictures(
			IEnumerable<EntryPictureFileContract> newPictures, User user, Func<string, string, User, T> picFactory) {

			ParamIs.NotNull(() => newPictures);

			var diff = CollectionHelper.Diff(Pictures, newPictures, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed) {
				Pictures.Remove(n);
			}

			foreach (var newEntry in diff.Added) {

				var l = picFactory(newEntry.Name, newEntry.Mime, user);
				l.UploadedFile = newEntry.FileName;
				created.Add(l);

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newPictures.First(e => e.Id == entry.Id);

				if (entry.Name != newEntry.Name) {
					linkEntry.Name = newEntry.Name;
					edited.Add(linkEntry);
				}

			}

			return new CollectionDiffWithValue<T, T>(created, diff.Removed, diff.Unchanged, edited);

		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual IEnumerator<T> GetEnumerator() {
			return Pictures.GetEnumerator();
		}

	}

}
