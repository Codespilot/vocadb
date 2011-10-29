﻿using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Globalization {

	public class NameManager<T> where T : LocalizedStringWithId {

		private IList<T> names = new List<T>();
		private TranslatedString sortNames = new TranslatedString();

		private T GetDefaultName() {

			if (!Names.Any())
				return null;

			var name = Names.FirstOrDefault(n => n.Language == sortNames.DefaultLanguage);
			
			return name ?? Names.First();

		}

		private T GetFirstName(ContentLanguageSelection languageSelection) {

			if (!Names.Any())
				return null;

			var name = Names.FirstOrDefault(n => n.Language == languageSelection);

			return name ?? GetDefaultName();

			/*if (name == null)
				name = Names.FirstOrDefault(n => n.Language == ContentLanguageSelection.Unspecified);

			return name;*/

		}

		private void SetValueFor(ContentLanguageSelection language) {

			if (!Names.Any())
				return;

			var name = GetFirstName(language);

			if (name != null)
				SortNames[language] = name.Value;

			if (string.IsNullOrEmpty(SortNames[language]))
				SortNames[language] = Names.First().Value;

		}

		public virtual IEnumerable<string> AllValues {
			get {

				return SortNames.All
					.Concat(Names.Select(n => n.Value))
					.Distinct();

			}
		}

		public virtual IList<T> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual TranslatedString SortNames {
			get { return sortNames; }
			set {
				ParamIs.NotNull(() => value);
				sortNames = value;
			}
		}

		/*public virtual bool HasNameFor(ContentLanguageSelection language) {

			return Names.Any(n => n.Language == language || n.Language == ContentLanguageSelection.Unspecified);

		}*/

		public virtual void Add(T name, bool update = true) {
			
			Names.Add(name);

			if (update)
				UpdateSortNames();

		}

		public virtual bool HasName(LocalizedString name) {

			return Names.Any(n => n.ContentEquals(name));

		}

		public virtual void Remove(T name, bool update = true) {

			Names.Remove(name);

			if (update)
				UpdateSortNames();

		}

		public virtual CollectionDiff<T,T> Sync(IEnumerable<LocalizedStringWithIdContract> newNames, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => newNames);
			ParamIs.NotNull(() => nameFactory);

			var diff = CollectionHelper.Diff(Names, newNames, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();

			foreach (var n in diff.Removed) {
				Remove(n);
			}

			foreach (var nameEntry in newNames) {

				var entry = nameEntry;
				var old = (entry.Id != 0 ? Names.FirstOrDefault(n => n.Id == entry.Id) : null);

				if (old != null) {

					old.Language = nameEntry.Language;
					old.Value = nameEntry.Value;

				} else {

					var n = nameFactory.CreateName(nameEntry.Value, nameEntry.Language);
					created.Add(n);

				}

			}

			UpdateSortNames();

			return new CollectionDiff<T,T>(created, diff.Removed, diff.Unchanged);

		}

		public virtual void UpdateSortNames() {

			if (!Names.Any())
				return;

			var languages = new[] { ContentLanguageSelection.Japanese, ContentLanguageSelection.Romaji, ContentLanguageSelection.English };

			foreach (var l in languages)
				SetValueFor(l);		

		}

	}
}
