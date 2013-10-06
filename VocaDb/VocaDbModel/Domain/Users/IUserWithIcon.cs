namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// Interface for <see cref="User"/> with email.
	/// Safe to send to client because the icon URL is precalculated.
	/// </summary> 
	public interface IUserWithIcon : IUser {

		string IconUrl { get; }

	}

}
