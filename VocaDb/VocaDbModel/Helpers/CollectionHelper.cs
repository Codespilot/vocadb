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

		public T[] Removed { get; private set; }

		public T[] Unchanged { get; private set; }

	}

}
