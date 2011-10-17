using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using log4net;
using NHibernate;
using VocaDb.Model.Mapping;
using VocaDb.Model.Mapping.Songs;

namespace VocaDb.Model.Service {

	public class DatabaseConfiguration {

		private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseConfiguration));

		public static ISessionFactory BuildSessionFactory() {

			var config = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2008
					.ConnectionString(c => c.FromConnectionStringWithKey("Jupiter"))
					.MaxFetchDepth(1)
				)
				.Cache(c => c
					.ProviderClass("NHibernate.Caches.SysCache2.SysCacheProvider, NHibernate.Caches.SysCache2")
					.UseSecondLevelCache()
					.UseQueryCache()
				)
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<SongMap>().Conventions.AddFromAssemblyOf<ClassConventions>());

			try {
				return config.BuildSessionFactory();
			} catch (ArgumentException x) {
				log.Error("Error while building session factory", x);
				throw;
			} catch (FluentConfigurationException x) {
				log.Error("Error while building session factory", x);
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
