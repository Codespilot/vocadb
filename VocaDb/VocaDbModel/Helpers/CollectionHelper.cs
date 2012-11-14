using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Helpers {

	public static class CollectionHelper {

		public static CollectionDiff<T, T2> Diff<T, T2>(IEnumerable<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality) {

			ParamIs.NotNull(() => old);
			ParamIs.NotNull(() => newItems);
			ParamIs.NotNull(() => equality);

			var removed = old.Where(i => !newItems.Any(i2 => equality(i, i2)));
			var added = newItems.Where(i => !old.Any(i2 => equality(i2, i)));
			var unchanged = old.Except(removed);

			return new CollectionDiff<T, T2>(added, removed, unchanged);

		}

		public static IEnumerable<T> MoveToTop<T>(IEnumerable<T> source, T top) {

			return Enumerable.Repeat(top, 1).Concat(source.Except(Enumerable.Repeat(top, 1)));

		}

		public static IEnumerable<T> MoveToTop<T>(IEnumerable<T> source, T[] top) {

			return top.Concat(source.Except(top));

		}

		public static void RemoveAll<T>(IList<T> list, Func<T, bool> pred) {

			bool changed = true;

			while (changed) {

				changed = false;

				foreach (var item in list) {
					if (pred(item)) {
						list.Remove(item);
						changed = true;
						break;
					}
				}

			}

		}

		public static IEnumerable<T> RemovedItems<T>(IEnumerable<T> old, IEnumerable<T> newItems) {

			return old.Where(i => !newItems.Contains(i));

		}

		public static IEnumerable<T> RemovedItems<T, T2>(IEnumerable<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality) {

			return old.Where(i => !newItems.Any(i2 => equality(i, i2)));

		}

		public static IEnumerable<T> SkipNull<T>(params T[] items) where T : class {

			return items.Where(i => i != null);

		}

	}

	/// <summary>
	/// Difference between two collections.
	/// </summary>
	/// <typeparam name="T">Type of the old collection.</typeparam>
	/// <typeparam name="T2">Type of the new collection (may be the same as old).</typeparam>
	public class CollectionDiff<T, T2> {

		public CollectionDiff(IEnumerable<T2> added, IEnumerable<T> removed, IEnumerable<T> unchanged) {

			ParamIs.NotNull(() => added);
			ParamIs.NotNull(() => removed);
			ParamIs.NotNull(() => unchanged);

			Added = added.ToArray();
			Removed = removed.ToArray();
			Unchanged = unchanged.ToArray();

		}

		/// <summary>
		/// Entries that didn't exist in the old set but do exist in the new one.
		/// </summary>
		public T2[] Added { get; private set; }

		/// <summary>
		/// Whether the contents of the sets were changed.
		/// </summary>
		public virtual bool Changed {
			get {
				return (Added.Any() || Removed.Any());
			}
		}

		/// <summary>
		/// Entries that existed in the old set but not in the new.
		/// </summary>
		public T[] Removed { get; private set; }

		/// <summary>
		/// Entries that existed in both old and new sets.
		/// Note: the contents of those entriers might still be changed, depending on equality.
		/// </summary>
		public T[] Unchanged { get; private set; }

	}

	public class CollectionDiffWithValue<T, T2> : CollectionDiff<T, T2> {

		public CollectionDiffWithValue(IEnumerable<T2> added, IEnumerable<T> removed, 
			IEnumerable<T> unchanged, IEnumerable<T> edited)
			: base(added, removed, unchanged) {

			ParamIs.NotNull(() => edited);

			Edited = edited.ToArray();

		}

		public override bool Changed {
			get {
				return (base.Changed || Edited.Any());
			}
		}

		/// <summary>
		/// Entries that existed in both old and new sets AND whose contents were changed.
		/// </summary>
		public T[] Edited { get; private set; }

	}

}
