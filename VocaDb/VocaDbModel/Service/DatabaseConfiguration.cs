using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using VocaDb.Model.Mapping;
using VocaDb.Model.Mapping.Songs;

namespace VocaDb.Model.Service {

	public class DatabaseConfiguration {

		public static ISessionFactory BuildSessionFactory() {

			var config = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2008
					.ConnectionString(c => c.FromConnectionStringWithKey("Jupiter"))
					.Cache(c => c.ProviderClass("NHibernate.Caches.SysCache2.SysCacheProvider, NHibernate.Caches.SysCache2")))
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<SongMap>().Conventions.AddFromAssemblyOf<ClassConventions>());

			return config.BuildSessionFactory();

		}

		public static ISessionFactory BuildSessionFactory(Action<MsSqlConnectionStringBuilder> connectionStringBuilder) {

			var config = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2008
					.ConnectionString(connectionStringBuilder)
					.Cache(c => c.ProviderClass("NHibernate.Caches.SysCache2.SysCacheProvider, NHibernate.Caches.SysCache2")))
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<SongMap>().Conventions.AddFromAssemblyOf<ClassConventions>());

			return config.BuildSessionFactory();

		}

	}

}
