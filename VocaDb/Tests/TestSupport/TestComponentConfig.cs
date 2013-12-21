using Autofac;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Repositories.NHibernate;
using VocaDb.Model.Service.Search;

namespace VocaDb.Tests.TestSupport {

	public static class TestComponentConfig {

		/// <summary>
		/// Creates additional required database schemas.
		/// NHibernate schema export doesn't create any schemas, so only dbo schema is created by default.
		/// </summary>
		private static void CreateSchemas(ISessionFactory sessionFactory) {

			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction()) {
				
				session.CreateSQLQuery(@"
					IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'mikudb')
					BEGIN
					EXEC('CREATE SCHEMA [mikudb]');
					END
				").ExecuteUpdate();

				tx.Commit();

			}

		}

		private static void FinishDatabaseConfig(ISessionFactory sessionFactory) {
			
			/* 
			 * Id in Tags table is identity even though it's not the primary Id.
			 * This isn't supported by FluentNHibernate so the column needs to be recreated as identity.
			 * Source: http://stackoverflow.com/a/5474146
			 * */
			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction()) {
				
				session.CreateSQLQuery(@"
					ALTER TABLE [Tags] DROP COLUMN [Id]
					ALTER TABLE [Tags] ADD [Id] [int] IDENTITY(1,1) NOT NULL
				").ExecuteUpdate();

				tx.Commit();

			}

		}

		private static ISessionFactory BuildTestSessionFactory() {
			
			var testDatabaseConnectionString = "LocalDB";
			var config = DatabaseConfiguration.Configure(testDatabaseConnectionString);

			/* 
			 * Need to comment these out when not needed because session factory can only be created once.
			 * Schemas need to be created BEFORE NHibernate schema export.
			 * This needs to be run only once.
			*/
			/*
			var fac = DatabaseConfiguration.BuildSessionFactory(config);

			CreateSchemas(fac);*/

			config.ExposeConfiguration(cfg => {

				var export = new SchemaExport(cfg);
				//export.SetOutputFile(@"C:\Temp\vdb.sql");
				export.Drop(false, true);
				export.Create(false, true);

			});

			var fac = DatabaseConfiguration.BuildSessionFactory(config);

			FinishDatabaseConfig(fac);

			return fac;

		}

		public static IContainer BuildContainer() {

			var builder = new ContainerBuilder();

			builder.Register(x => BuildTestSessionFactory()).SingleInstance();
			builder.Register(x => x.Resolve<ISessionFactory>().OpenSession()).InstancePerLifetimeScope();
			builder.RegisterType<QuerySourceSession>().As<IQuerySource>();
			builder.RegisterType<FakePermissionContext>().As<IUserPermissionContext>();

			builder.RegisterType<AlbumNHibernateRepository>().As<IAlbumRepository>();
			builder.RegisterType<ArtistNHibernateRepository>().As<IArtistRepository>();
			builder.RegisterType<EntryReportNHibernateRepository>().As<IEntryReportRepository>();
			builder.RegisterType<SongNHibernateRepository>().As<ISongRepository>();
			builder.RegisterType<SongListNHibernateRepository>().As<ISongListRepository>();
			builder.RegisterType<TagNHibernateRepository>().As<ITagRepository>();
			builder.RegisterType<UserNHibernateRepository>().As<IUserRepository>();
			builder.RegisterType<UserMessageNHibernateRepository>().As<IUserMessageRepository>();

			var container = builder.Build();
			return container;

		}

	}

}
