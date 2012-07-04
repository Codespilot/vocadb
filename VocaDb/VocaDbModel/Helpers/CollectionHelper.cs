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

		public T2[] Added { get; private set; }

		public virtual bool Changed {
			get {
				return (Added.Any() || Removed.Any());
			}
		}

		public T[] Removed { get; private set; }

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

		public T[] Edited { get; private set; }

	}

}
