namespace VocaDb.Model.Domain {

	public class DateTimeYearAndDate {

		public DateTimeYearAndDate() {}

		public DateTimeYearAndDate(int year) {
			Year = year;
		}

		public DateTimeYearAndDate(int year, int? day, int? month) {
			Year = year;
			Month = month;
			Day = day;
		}

		public int? Day { get; set; }

		public int? Month { get; set; }

		public int Year { get; set; }

	}
}
