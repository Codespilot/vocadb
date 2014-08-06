namespace VocaDb.Model.Domain {

	public interface IEntryWithIntId {

		int Id { get; set; }

	}

	public static class IEntryWithIntIdExtender {

		public static bool NullSafeIdEquals(this IEntryWithIntId left, IEntryWithIntId right) {

			if (left == null && right == null)
				return true;

			if (left != null && right != null)
				return left.Id == right.Id;

			return false;
			
		}

	}

}
