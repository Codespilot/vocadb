using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers {

	public class StatsController : Controller {

		class LocalizedValue {

			public TranslatedString Name { get; set; }

			public int Value { get; set; }

		}

		private JsonResult SimpleBarChart(string title, string seriesName, IList<string> categories, IList<int> data) {
			
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1));

			return Json(new {
				chart = new {
					type = "bar",
					height = 600
				},
				title = new {
					text = title
				},
				xAxis = new {
					categories,
					title = new {
						text = (string)null
					}
				},
				yAxis = new {
					title = new {
						text = seriesName
					}
				},
				plotOptions = new {
					bar = new {
						dataLabels = new {
							enabled = true
						}
					}
				},
				legend = new {
					enabled = false
				},
				series = new Object[] {
					new {
						name = seriesName,
						data						
					}
				}
				
			}, JsonRequestBehavior.AllowGet);


		}

		private JsonResult SimpleBarChart<T>(Func<IQueryable<T>, IQueryable<LocalizedValue>> func, string title, string seriesName) {

			var values = GetTopValues(func);

			var categories = values.Select(p => p.Name[permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart(title, seriesName, categories, data);

		}

		private LocalizedValue[] GetTopValues<T>(Func<IQueryable<T>, IQueryable<LocalizedValue>> func) {
			
			return userRepository.HandleQuery(ctx => {
				
				return func(ctx.OfType<T>().Query())
					.OrderByDescending(a => a.Value)
					.Take(25)
					.ToArray();

			});

		}

		private readonly IUserPermissionContext permissionContext;
		private readonly IUserRepository userRepository;

		public StatsController(IUserRepository userRepository, IUserPermissionContext permissionContext) {
			this.userRepository = userRepository;
			this.permissionContext = permissionContext;
		}

		public JsonResult AlbumsPerProducer() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllAlbums.Count(s => !s.IsSupport && !s.Album.Deleted && s.Album.DiscType != DiscType.Compilation)
					}), "Albums by producer", "Songs");

		}

		public JsonResult AlbumsPerVocaloid() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Vocaloid || a.ArtistType == ArtistType.UTAU)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllAlbums.Count(s => !s.IsSupport && !s.Album.Deleted)
					}), "Albums by Vocaloid/UTAU", "Songs");

		}

		public JsonResult SongsPerProducer() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllSongs.Count(s => !s.IsSupport && !s.Song.Deleted && s.Song.SongType == SongType.Original)
					}), "Original songs by producer", "Songs");

		}

		public JsonResult SongsPerVocaloid() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Vocaloid || a.ArtistType == ArtistType.UTAU)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllSongs.Count(s => !s.IsSupport && !s.Song.Deleted)
					}), "Songs by Vocaloid/UTAU", "Songs");

		}

		public JsonResult FollowersPerProducer() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.Users.Count
					}), "Followers by producer", "Followers");

		}

		public ActionResult Index() {
			
			return View();

		}

	}

}