namespace VocaDb.Model.Domain.Security {

	public class IPRule {

		private string address;

		public virtual string Address {
			get { return address; }
			set { 
				ParamIs.NotNullOrEmpty(() => value);
				address = value; 
			}
		}

		public virtual int Id { get; set; }

	}
}
