namespace VocaDb.Model.Domain.Globalization {

	public class LocalizedStringWithId : LocalizedString {

		public LocalizedStringWithId() {}

		public LocalizedStringWithId(string val, ContentLanguageSelection language) 
			: base(val, language) {}

		public virtual int Id { get; protected set; }

	}
}
