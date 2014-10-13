﻿using System.Web.Optimization;

namespace VocaDb.Web.App_Start {

	public static class BundleConfig {

		public static void RegisterBundles(BundleCollection bundles) {

			bundles.Add(new ScriptBundle("~/bundles/shared/libs").Include(
				"~/Scripts/jquery-1.8.2.js", 
				"~/Scripts/bootstrap.js",
				//"~/Scripts/jquery-ui-1.10.1.js", // doesn't work if bundled together
				"~/Scripts/knockout-{version}.js",
				"~/Scripts/lodash.js", 
				"~/Scripts/jquery.qtip.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/shared/jqui").Include(
				"~/Scripts/jquery-ui-1.10.4.js"
			));

			// SHARED BUNDLES
			// Legacy common scripts - should be phased out
			bundles.Add(new ScriptBundle("~/bundles/VocaDB").Include("~/Scripts/VocaDB.js"));

			// Included on every page
			// Generally the references go from viewmodels -> repositories -> models -> support classes
			bundles.Add(new ScriptBundle("~/bundles/shared/common").Include(
				"~/Scripts/Shared/NamesList.js",
				"~/Scripts/Shared/GlobalSearchBox.js", 
				"~/Scripts/Shared/Messages.js",
				"~/Scripts/Shared/GlobalFunctions.js",
				"~/Scripts/Shared/UrlMapper.js",
				"~/Scripts/Shared/EntryUrlMapper.js",
				"~/Scripts/Shared/ReportEntryPopup.js",
				"~/Scripts/KnockoutExtensions/Dialog.js",
				"~/Scripts/KnockoutExtensions/EntryToolTip.js",
				"~/Scripts/KnockoutExtensions/jqButton.js",
				"~/Scripts/KnockoutExtensions/jqButtonset.js",
				"~/Scripts/KnockoutExtensions/StopBinding.js",
				"~/Scripts/Models/SongVoteRating.js",				// Referred by UserRepository
				"~/Scripts/Repositories/AdminRepository.js",
				"~/Scripts/Repositories/EntryReportRepository.js",
				"~/Scripts/Repositories/UserRepository.js",
				"~/Scripts/Repositories/SongRepository.js",
				"~/Scripts/Repositories/ArtistRepository.js",
				"~/Scripts/ViewModels/TopBarViewModel.js"
			));

			// Included on all entry edit and create pages (album, artist, my settings etc.)
			bundles.Add(new ScriptBundle("~/bundles/shared/edit").Include(
				"~/Scripts/knockout-sortable.js",
				"~/Scripts/marked.js",
				"~/Scripts/Models/WebLinkCategory.js",
				"~/Scripts/Shared/WebLinkMatcher.js",
				"~/Scripts/ViewModels/WebLinkEditViewModel.js",
				"~/Scripts/ViewModels/WebLinksEditViewModel.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/SongAutoComplete.js",
				"~/Scripts/KnockoutExtensions/FocusOut.js",
				"~/Scripts/KnockoutExtensions/qTip.js",
				"~/Scripts/KnockoutExtensions/Markdown.js",
				"~/Scripts/KnockoutExtensions/InitialValue.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqxRating").Include(
				"~/Scripts/jqwidgets27/jqxcore.js", "~/Scripts/jqwidgets27/jqxrating.js"));


			// VIEW-SPECIFIC BUNDLES
			bundles.Add(new ScriptBundle("~/bundles/Album/Details").Include(
				"~/Scripts/ViewModels/Album/AlbumDetailsViewModel.js",
				"~/Scripts/Album/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Album/Edit").Include(
				"~/Scripts/KnockoutExtensions/ParseInteger.js",
				"~/Scripts/Models/Albums/AlbumType.js",
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
				"~/Scripts/ViewModels/SongInAlbumEditViewModel.js",
				"~/Scripts/ViewModels/AlbumEditViewModel.js",
				"~/Scripts/Album/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Create").Include(
				"~/Scripts/ViewModels/ArtistCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Details").Include(
				"~/Scripts/ViewModels/Artist/ArtistDetailsViewModel.js",
				"~/Scripts/Artist/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Edit").Include(
				"~/Scripts/Models/Artists/ArtistType.js",
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistLinkViewModel.js",
				"~/Scripts/ViewModels/ArtistEditViewModel.js",
				"~/Scripts/Artist/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Home/Index").Include(
				"~/Scripts/ViewModels/NewsListViewModel.js",
				"~/Scripts/Home/Index.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Search/Index").Include(
				"~/Scripts/Shared/EntrySearchDrop.js",
				"~/Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.js",
				"~/Scripts/KnockoutExtensions/Song/SongTypeLabel.js",
				"~/Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/Models/PVServiceIcons.js",				
				"~/Scripts/Models/Artists/ArtistType.js",
				"~/Scripts/Models/PVs/PVService.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/EntryRepository.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/Search/SearchViewModel.js",
				"~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
				"~/Scripts/ViewModels/Search/ArtistSearchViewModel.js",
				"~/Scripts/ViewModels/Search/AlbumSearchViewModel.js",
				"~/Scripts/ViewModels/Search/SongSearchViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Create").Include(
				"~/Scripts/ViewModels/SongCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Details").Include(				
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/Song/SongDetailsViewModel.js",
				"~/Scripts/Song/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Edit").Include(
				"~/Scripts/Models/Artists/ArtistRoles.js",
				"~/Scripts/Models/Artists/ArtistType.js",
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Models/Songs/SongType.js",
				"~/Scripts/Models/Tags/Tag.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/SongHelper.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
				"~/Scripts/ViewModels/SongEditViewModel.js",
				"~/Scripts/Song/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Index").Include(
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/KnockoutExtensions/Song/PVPreviewStatus.js",
				"~/Scripts/Song/Index.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Details").Include(
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/SongList/SongListViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Tag/Edit").Include(
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/ViewModels/TagEditViewModel.js",
				"~/Scripts/Tag/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/AlbumCollection").Include(
				"~/Scripts/Shared/EntrySearchDrop.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/User/AlbumCollectionViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Details").Include(
				"~/Scripts/Shared/EntrySearchDrop.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/User/FollowedArtistsViewModel.js",
				"~/Scripts/ViewModels/User/RatedSongsSearchViewModel.js",
				"~/Scripts/ViewModels/User/AlbumCollectionViewModel.js",
				"~/Scripts/ViewModels/User/UserDetailsViewModel.js",
				"~/Scripts/User/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Messages").Include(
				"~/Scripts/ViewModels/User/UserMessagesViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/MySettings").Include(
				"~/Scripts/ViewModels/User/MySettingsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/RatedSongs").Include(
				"~/Scripts/Shared/EntrySearchDrop.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/User/RatedSongsSearchViewModel.js"
			));


#if DEBUG
			bundles.Add(new ScriptBundle("~/bundles/tests")
				.IncludeDirectory("~/Scripts/Models", "*.js")
				.IncludeDirectory("~/Scripts/Repositories", "*.js", true)
				.IncludeDirectory("~/Scripts/ViewModels", "*.js", true)
				.Include("~/Scripts/Shared/WebLinkMatcher.js")
				.IncludeDirectory("~/Scripts/Tests", "*.js", true)
			);
#endif


			// Base CSS
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css", 
				"~/Content/Site.css", 
				//"~/Content/Styles/Snow2013.css",
				"~/Content/Styles/PVViewer_Black.css",
				"~/Content/Styles/ExtLinks.css", 
				"~/Content/Styles/Overrides.css",
				"~/Content/Styles/StyleOverrides.css",
				"~/Content/Styles/Search.css"
				));

			bundles.Add(new StyleBundle("~/Content/embedSong").Include(
				"~/Content/bootstrap.css", "~/Content/EmbedSong.css"));

			// CSS for jqxRating
			bundles.Add(new StyleBundle("~/Scripts/jqwidgets27/styles/css").Include(
				"~/Scripts/jqwidgets27/styles/jqx.base.css"));

		}

	}

}