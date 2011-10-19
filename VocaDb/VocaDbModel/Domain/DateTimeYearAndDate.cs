using System;
using VocaDb.Model.DataContracts;
namespace VocaDb.Model.Domain {

	public class OptionalDateTime {

		public static OptionalDateTime Create(OptionalDateTimeContract contract) {

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

		public virtual int? Day { get; protected set; }

		public virtual bool IsEmpty {
			get { return (Year == null); }
		}

		public virtual int? Month { get; protected set; }

		public virtual int? Year { get; protected set; }

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
