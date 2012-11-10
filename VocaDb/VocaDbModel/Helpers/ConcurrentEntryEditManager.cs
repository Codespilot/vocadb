using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers {

	/// <summary>
	/// Manages concurrent entry edits.
	/// </summary>
	public class ConcurrentEntryEditManager {

		public static EntryEditData Nothing = new EntryEditData();

		public struct EntryEditData {

			public EntryEditData(IUser user)
				: this() {

				UserId = user.Id;
				UserName = user.Name;
				Time = DateTime.Now;

			}

			public DateTime Time { get; private set; }

			public int UserId { get; private set; }

			public string UserName { get; private set; }

			public void Refresh(IUser user) {

				if (user.Id == UserId)
					Time = DateTime.Now;

			}

		}

		private static readonly Dictionary<EntryRef, EntryEditData> editors = new Dictionary<EntryRef, EntryEditData>();

		private static void ClearExpiredUsages() {

			var cutoffDate = DateTime.Now - TimeSpan.FromMinutes(5);

			lock (editors) {

				var expired = editors.Where(e => e.Value.Time < cutoffDate).Select(e => e.Key).ToArray();

				foreach (var e in expired)
					editors.Remove(e);

			}

		}

		private static void AddOrUpdate(EntryRef entry, IUser user) {

			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => user);

			lock (editors) {

				if (editors.ContainsKey(entry))
					editors[entry].Refresh(user);
				else
					editors.Add(entry, new EntryEditData(user));

			}

		}

		private static EntryEditData GetEditor(EntryRef entry) {

			ParamIs.NotNull(() => entry);

			lock (editors) {

				if (editors.ContainsKey(entry))
					return editors[entry];

			}

			return Nothing;

		}

		public static EntryEditData CheckConcurrentEdits(EntryRef entry, IUser user) {

			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => user);

            ClearExpiredUsages();

			var editor = GetEditor(entry);

			if (editor.UserId != Nothing.UserId && editor.UserId != user.Id)
				return editor;

			AddOrUpdate(entry, user);

			return Nothing;

		}

	}
}
