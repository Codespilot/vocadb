using System;
using VocaDb.Model.DataContracts;
namespace VocaDb.Model.Domain {

	public class OptionalDateTime {

		public static OptionalDateTime Create(OptionalDateTimeContract contract) {

			ParamIs.NotNull(() => contract);

			return new OptionalDateTime(contract.Year, contract.Day, contract.Month);

		}

		public OptionalDateTime() {}

		public OptionalDateTime(int year) {

			ParamIs.NonNegative(() => year);
			Year = year;

		}

		public OptionalDateTime(int? year, int? day, int? month) {

			if (year != null) {

				if (year < 0)
					throw new FormatException("Invalid date");

				if (month != null) {

					if (month.Value < 1 || month.Value > 12)
						throw new FormatException("Invalid date");


					if (day != null) {
						if (day.Value < 1 || day.Value > DateTime.DaysInMonth(year.Value, month.Value))
							throw new FormatException("Invalid date");
					}
				}
			}

			Year = year;
			Month = month;
			Day = day;

		}

		public virtual int? Day { get; protected set; }

		public virtual int? Month { get; protected set; }

		public virtual int? Year { get; protected set; }

	}
}
