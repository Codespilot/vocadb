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

		public setView = (viewName: string) => {
			
			switch (viewName) {
				case "Albums":
					this.albumCollectionViewModel.init();
					break;
				case "Artists":
					this.followedArtistsViewModel.init();
					break;
				case "Songs":
					this.ratedSongsViewModel.init();
					break;
			}

			window.location.hash = viewName != "Overview" ? viewName : "";
			this.view(viewName);		

		}

		public setOverview = () => this.setView("Overview");
		public setViewAlbums = () => this.setView("Albums");
		public setViewArtists = () => this.setView("Artists");
		public setCustomLists = () => this.setView("CustomLists");
		public setViewSongs = () => this.setView("Songs");

		constructor(private adminRepo: rep.AdminRepository,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel) {
	        
        }

    }

}