﻿using System;
using VocaDb.Model.DataContracts;
namespace VocaDb.Model.Domain {

	public class OptionalDateTime : IOptionalDateTime {

		public static bool operator ==(OptionalDateTime p1, OptionalDateTime p2) {

			if (ReferenceEquals(p1, null) && ReferenceEquals(p2, null))
				return true;

			if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
				return false;

			return p1.Equals(p2);

		}

		public static bool operator !=(OptionalDateTime p1, OptionalDateTime p2) {
			return !(p1 == p2);
		}

		public static OptionalDateTime Create(IOptionalDateTime contract) {

			ParamIs.NotNull(() => contract);

			return new OptionalDateTime(contract.Year, contract.Month, contract.Day);

		}

		public static bool IsValid(int? year, int? day, int? month) {

			if (year != null) {

				if (year < 0)
					return false;

				if (month != null) {

					if (month.Value < 1 || month.Value > 12)
						return false;


					if (day != null) {
						if (day.Value < 1 || day.Value > DateTime.DaysInMonth(year.Value, month.Value))
							return false;
					}
				}
			}

			return true;

		}

		public static DateTime ToDateTime(int? year, int? month, int? day) {

			return new DateTime(year ?? DateTime.MinValue.Year, month ?? 1, day ?? 1);

		}

		public static void Validate(int? year, int? day, int? month) {

			if (!IsValid(year, day, month))
				throw new FormatException("Invalid date");

		}

		public OptionalDateTime() {}

		public OptionalDateTime(int year) {

			ParamIs.NonNegative(() => year);
			Year = year;

		}

		public OptionalDateTime(int? year, int? month, int? day) {

			Validate(year, day, month);

			Year = year;
			Month = month;
			Day = day;

		}

		public virtual int? Day { get; set; }

		public virtual bool IsEmpty {
			get { return (Year == null); }
		}

		public virtual int? Month { get; set; }

		/// <summary>
		/// Sortable datetime for database (not currently in use).
		/// </summary>
		public virtual DateTime SortableDateTime {
			get {
				return new DateTime(Year ?? 1970, Month ?? 1, Day ?? 1);
			}
// ReSharper disable ValueParameterNotUsed
			protected set { }
// ReSharper restore ValueParameterNotUsed
		}

		public virtual int? Year { get; set; }

		public virtual bool Equals(OptionalDateTime another) {

			if (another == null)
				return IsEmpty;

			return (Year == another.Year && Month == another.Month && Day == another.Day);

		}

		public override bool Equals(object obj) {
			
			if (!(obj is OptionalDateTime))
				return false;

			return Equals((OptionalDateTime)obj);

		}

		public override int GetHashCode() {

			return ToString().GetHashCode();

		}

		public DateTime ToDateTime() {

			return ToDateTime(Year, Month, Day);

		}

		/// <summary>
		/// Provides a localized string representation of this date.
		/// </summary>
		/// <remarks>
		/// If the date is empty, an empty string will be returned.
		/// If only year is specified, that year is returned.
		/// If full date is specified, a localized short date string (formatted according to current culture) will be returned.
		/// </remarks>
		public override string ToString() {
			if (Year != null) {
				if (Month != null && Day != null)
					return new DateTime(Year.Value, Month.Value, Day.Value).ToShortDateString();
				else
					return Year.Value.ToString();
			}
			return string.Empty;
		}

	}
}
