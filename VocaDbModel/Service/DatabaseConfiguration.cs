using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NLog;
using NHibernate;
using VocaDb.Model.Mapping;
using VocaDb.Model.Mapping.Songs;
using System.Configuration;

namespace VocaDb.Model.Service {

	public static class DatabaseConfiguration {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static string ConnectionStringName {
			get {
				return ConfigurationManager.AppSettings["ConnectionStringName"] ?? "Jupiter";
			}
		}

		public static ISessionFactory BuildSessionFactory() {

			var config = Fluently.Configure()
				.Database(
					MsSqlConfiguration.MsSql2008
						.ConnectionString(c => c.FromConnectionStringWithKey(ConnectionStringName))
						.MaxFetchDepth(1)
						.UseReflectionOptimizer()
				)
				.Cache(c => c
					.ProviderClass<NHibernate.Caches.SysCache2.SysCacheProvider>()
					.UseSecondLevelCache()
					.UseQueryCache()
				)
				.Mappings(m => m
					.FluentMappings.AddFromAssemblyOf<SongMap>()
					.Conventions.AddFromAssemblyOf<ClassConventions>()
				)
				/*.Diagnostics(d => d
					.Enable()
					.OutputToFile("C:\\Temp\\Fluent.txt")
				)*/
				;


			try {
				return config.BuildSessionFactory();
			} catch (ArgumentException x) {
				log.FatalException("Error while building session factory", x);
				throw;
			} catch (FluentConfigurationException x) {
				log.FatalException("Error while building session factory", x);
				throw;
			}

		}

		public static ISessionFactory BuildSessionFactory(Action<MsSqlConnectionStringBuilder> connectionStringBuilder) {

			var config = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2008
					.ConnectionString(connectionStringBuilder))
				.Cache(c => c.ProviderClass("NHibernate.Caches.SysCache2.SysCacheProvider, NHibernate.Caches.SysCache2"))
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<SongMap>().Conventions.AddFromAssemblyOf<ClassConventions>());

			return config.BuildSessionFactory();

		}

	}

}
