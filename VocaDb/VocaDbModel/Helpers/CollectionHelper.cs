using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Helpers {

	public static class CollectionHelper {

		public static CollectionDiff<T, T2> Diff<T, T2>(IEnumerable<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality) {

			var removed = old.Where(i => !newItems.Any(i2 => equality(i, i2)));
			var added = newItems.Where(i => !old.Any(i2 => equality(i2, i)));

			return new CollectionDiff<T, T2>(added, removed);

		}

		public static IEnumerable<T> RemovedItems<T>(IEnumerable<T> old, IEnumerable<T> newItems) {

			return old.Where(i => !newItems.Contains(i));

		}

		public static IEnumerable<T> RemovedItems<T, T2>(IEnumerable<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality) {

			return old.Where(i => !newItems.Any(i2 => equality(i, i2)));

		}

	}

	public class CollectionDiff<T, T2> {

		public CollectionDiff(IEnumerable<T2> added, IEnumerable<T> removed) {
			Added = added.ToArray();
			Removed = removed.ToArray();
		}

		public T2[] Added { get; private set; }

		public T[] Removed { get; private set; }

	}

}
