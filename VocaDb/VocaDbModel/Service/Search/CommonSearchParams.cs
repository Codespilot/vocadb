namespace VocaDb.Model.Service.Search {

	/// <summary>
	/// Common search parameters for all entry types.
	/// </summary>
	public class CommonSearchParams {

		public CommonSearchParams() {
			NameMatchMode = NameMatchMode.Auto;
		}

		public CommonSearchParams(string query, bool draftOnly, NameMatchMode nameMatchMode, bool onlyByName, bool moveExactToTop)
			: this() {

			DraftOnly = draftOnly;
			NameMatchMode = nameMatchMode;
			Query = query;
			OnlyByName = onlyByName;
			MoveExactToTop = moveExactToTop;

		}

		public bool DraftOnly { get; set; }

		public bool MoveExactToTop { get; set; }

		public NameMatchMode NameMatchMode { get; set; }

		public bool OnlyByName { get; set; }

		public string Query { get; set; }

	}

}
