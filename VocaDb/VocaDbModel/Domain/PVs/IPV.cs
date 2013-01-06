namespace VocaDb.Model.Domain.PVs {

	/// <summary>
	/// Interface for PVs
	/// </summary>
	public interface IPV {

		/// <summary>
		/// PV type
		/// </summary>
		PVType PVType { get; }

		/// <summary>
		/// PV service
		/// </summary>
		PVService Service { get; }

	}

}
