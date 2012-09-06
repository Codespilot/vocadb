﻿using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;
using System.Collections;

namespace VocaDb.Model.Domain.Globalization {

	public class NameManager<T> : INameManager, IEnumerable<T> where T : LocalizedStringWithId {

		private string additionalNamesString;
		private IList<T> names = new List<T>();
		private TranslatedString sortNames = new TranslatedString();

		private T FirstName(ContentLanguageSelection languageSelection) {
			return Names.FirstOrDefault(n => n.Language == languageSelection);
		}

		private T GetDefaultName() {

			if (!Names.Any())
				return null;

			var name = FirstName(sortNames.DefaultLanguage);
			
			return name ?? Names.First();

		}

		private T GetFirstName(ContentLanguageSelection languageSelection) {

			if (!Names.Any())
				return null;

			var name = FirstName(languageSelection);

			// Substitute English with Romaji
			if (name == null && languageSelection == ContentLanguageSelection.English)
				name = FirstName(ContentLanguageSelection.Romaji);

			// Substitute Romaji with English
			if (name == null && languageSelection == ContentLanguageSelection.Romaji)
				name = FirstName(ContentLanguageSelection.English);

			return name ?? GetDefaultName();

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

		public virtual string AdditionalNamesString {
			get { return additionalNamesString; }
			set {
				ParamIs.NotNull(() => value);
				additionalNamesString = value; 
			}
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

		public virtual void Add(T name, bool update = true) {
			
			Names.Add(name);

			if (update)
				UpdateSortNames();

		}

		public string GetAdditionalNamesStringForLanguage(ContentLanguagePreference languagePreference) {

			var display = SortNames[languagePreference];
			var different = SortNames.All.Where(s => s != display).Distinct();

			if (!string.IsNullOrEmpty(AdditionalNamesString))
				return string.Join(", ", different.Concat(Enumerable.Repeat(AdditionalNamesString, 1)));
			else
				return string.Join(", ", different);

		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Names.GetEnumerator();
		}

		public virtual IEnumerator<T> GetEnumerator() {
			return Names.GetEnumerator();
		}

		public EntryNameContract GetEntryName(ContentLanguagePreference languagePreference) {

			var display = SortNames[languagePreference];
			var additional = string.Join(", ", AllValues.Where(n => n != display));

			return new EntryNameContract(display, additional);

		}

		public virtual bool HasName(LocalizedString name) {

			return Names.Any(n => n.ContentEquals(name));

		}

		public virtual bool HasName(string val) {

			return Names.Any(n => n.Value.Equals(val, StringComparison.InvariantCultureIgnoreCase));

		}

		public virtual void Init(IEnumerable<LocalizedStringContract> names, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => names);
			ParamIs.NotNull(() => nameFactory);

			foreach (var name in names)
				nameFactory.CreateName(name.Value, name.Language);

			if (names.Any(n => n.Language == ContentLanguageSelection.Japanese))
				SortNames.DefaultLanguage = ContentLanguageSelection.Japanese;
			else if (names.Any(n => n.Language == ContentLanguageSelection.Romaji))
				SortNames.DefaultLanguage = ContentLanguageSelection.Romaji;
			else if (names.Any(n => n.Language == ContentLanguageSelection.English))
				SortNames.DefaultLanguage = ContentLanguageSelection.English;

		}

		public virtual void Remove(T name, bool update = true) {

			Names.Remove(name);

			if (update)
				UpdateSortNames();

		}

		public virtual CollectionDiffWithValue<T,T> Sync(IEnumerable<LocalizedStringWithIdContract> newNames, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => newNames);
			ParamIs.NotNull(() => nameFactory);

			var diff = CollectionHelper.Diff(Names, newNames, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed) {
				Remove(n);
			}

			foreach (var nameEntry in newNames) {

				var entry = nameEntry;
				var old = (entry.Id != 0 ? Names.FirstOrDefault(n => n.Id == entry.Id) : null);

				if (old != null) {

					if (!old.ContentEquals(nameEntry)) {
						old.Language = nameEntry.Language;
						old.Value = nameEntry.Value;
						edited.Add(old);
					}

				} else {

					var n = nameFactory.CreateName(nameEntry.Value, nameEntry.Language);
					created.Add(n);

				}

			}

			UpdateSortNames();

			return new CollectionDiffWithValue<T,T>(created, diff.Removed, diff.Unchanged, edited);

		}

		public virtual CollectionDiff<T, T> SyncByContent(IEnumerable<LocalizedStringContract> newNames, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => newNames);
			ParamIs.NotNull(() => nameFactory);

			var diff = CollectionHelper.Diff(Names, newNames, (n1, n2) => n1.ContentEquals(n2));
			var created = new List<T>();

			foreach (var n in diff.Removed) {
				Remove(n);
			}

			foreach (var nameEntry in diff.Added) {

				var n = nameFactory.CreateName(nameEntry.Value, nameEntry.Language);
				created.Add(n);

			}

			UpdateSortNames();

			return new CollectionDiff<T, T>(created, diff.Removed, diff.Unchanged);

		}

		public virtual void UpdateSortNames() {

			if (!Names.Any())
				return;

			var languages = new[] { ContentLanguageSelection.Japanese, ContentLanguageSelection.Romaji, ContentLanguageSelection.English };

			foreach (var l in languages)
				SetValueFor(l);

			var additionalNames = Names.Select(n => n.Value).Where(n => !SortNames.All.Contains(n));
			AdditionalNamesString = string.Join(", ", additionalNames);

		}

	}
}
