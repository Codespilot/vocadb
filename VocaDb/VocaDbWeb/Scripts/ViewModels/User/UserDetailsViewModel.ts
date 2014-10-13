/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../Repositories/AdminRepository.ts" />

module vdb.viewModels.user {

    import rep = vdb.repositories;

    export class UserDetailsViewModel {

        public checkSFS = (ip: string) => {

            this.adminRepo.checkSFS(ip, html => {

                $("#sfsCheckDialog").html(html);
                $("#sfsCheckDialog").dialog("open");

            });

		};

		public view = ko.observable("Overview");

		public setViewAlbums = () => {
			this.albumCollectionViewModel.init();
			this.view("Albums");
		}

		public setViewArtists = () => {
			this.followedArtistsViewModel.init();
			this.view("Artists");
		}

		public setCustomLists = () => {
			this.view("CustomLists");
		}
		public setViewSongs = () => {
			this.ratedSongsViewModel.init();
			this.view("Songs");
		}

		constructor(private adminRepo: rep.AdminRepository,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel) {
	        
        }

    }

}