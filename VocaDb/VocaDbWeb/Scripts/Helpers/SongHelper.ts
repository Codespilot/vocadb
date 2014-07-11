﻿
module vdb.helpers {

	import cls = vdb.models;
	var SongType = cls.songs.SongType;

	export class SongHelper {
		
		// Checks whether a song type is to be considered animation where animators are considered as the main role
		public static isAnimation(songType: cls.songs.SongType) {
			return songType == SongType.MusicPV || songType == SongType.DramaPV;
		}

		// Checks whether a song type is to be considered instrumental where the song is allowed to have no vocalists
		public static isInstrumental(songType: cls.songs.SongType) {
			return songType == SongType.Instrumental || songType == SongType.DramaPV;
		}

	}

}