﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NLog;

namespace VocaDb.SiteMapGenerator.VocaDb {

	public class VocaDbClient {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly string apiRoot;

		private async Task<T> GetEntries<T>(string apiUrl) {
			
			var uri = new Uri(apiUrl);
			using (var client = new HttpClient()) {
				
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response;

				try {
					response = await client.GetAsync(uri);
					response.EnsureSuccessStatusCode();
				} catch (HttpRequestException x) {
					log.FatalException("Unable to get entries from VocaDB API", x);
					throw;
				}

				try {
					var entries = await response.Content.ReadAsAsync<T>();
					return entries;
				} catch (UnsupportedMediaTypeException x) {
					log.FatalException("Unable to get entries from VocaDB API", x);
					throw;			
				}

			}

		}

		public VocaDbClient(string apiRoot) {
			this.apiRoot = apiRoot;
		}

		public async Task<int[]> GetAlbums() {
			log.Info("Getting albums");
			return await GetEntries<int[]>(string.Format("{0}api/albums/ids", apiRoot));
		}

		public async Task<int[]> GetArtists() {
			log.Info("Getting artists");
			return await GetEntries<int[]>(string.Format("{0}api/artists/ids", apiRoot));
		}

		public async Task<int[]> GetSongs() {
			log.Info("Getting songs");
			return await GetEntries<int[]>(string.Format("{0}api/songs/ids", apiRoot));
		}

		public async Task<string[]> GetTags() {
			log.Info("Getting tags");
			return await GetEntries<string[]>(string.Format("{0}api/tags/names?maxResults=100000&allowAliases=false", apiRoot));
		}

	}

}
