using System;

namespace VocaDb.Model.Domain.Artists {

	[Flags]
	public enum ArtistRoles {

		Default				= 0,

		/// <summary>
		/// Usually associated with remixes/covers
		/// </summary>
		Arranger			= 1,

		Composer			= 2,

		/// <summary>
		/// Usually circle/label
		/// </summary>
		Distributor			= 4,

		/// <summary>
		/// PVs, cover art, booklet
		/// </summary>
		Illustrator			= 8,

		/// <summary>
		/// Plays instruments
		/// </summary>
		Instrumentalist		= 16,

		Lyricist			= 32,

		Mastering			= 64,

		/// <summary>
		/// Usually circle/label
		/// </summary>
		Publisher			= 128,

		Vocalist			= 256,

		/// <summary>
		/// Vocaloid voice manipulator
		/// </summary>
		VoiceManipulator	= 512,

		Other				= 1024

	}
}
