using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Repositories.NHibernate;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Services;

namespace VocaDb.Web.App_Start {

	/// <summary>
	/// Configures AutoFac dependency injection.
	/// </summary>
	public static class ComponentConfig {

		public static void RegisterComponent() {

			var builder = new ContainerBuilder();

			// Register services.
			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterType<QueryService>();

			builder.Register(x => DatabaseConfiguration.BuildSessionFactory()).SingleInstance();

			builder.RegisterType<LoginManager>().As<IUserPermissionContext>();
			builder.RegisterType<EntryAnchorFactory>().As<IEntryLinkFactory>();

			// Legacy services
			builder.RegisterType<ServiceModel>().AsSelf();
			builder.RegisterType<AdminService>().AsSelf();
			builder.RegisterType<AlbumService>().AsSelf();
			builder.RegisterType<SongService>().AsSelf();
			builder.RegisterType<TagService>().AsSelf();
			builder.RegisterType<UserService>().AsSelf();

			// Repositories
			builder.RegisterType<AlbumNHibernateRepository>().As<IAlbumRepository>();
			builder.RegisterType<SongListNHibernateRepository>().As<ISongListRepository>();
			builder.RegisterType<TagNHibernateRepository>().As<ITagRepository>();
			builder.RegisterType<UserNHibernateRepository>().As<IUserRepository>();
			builder.RegisterType<AlbumQueries>().AsSelf();
			builder.RegisterType<SongListQueries>().AsSelf();
			builder.RegisterType<TagQueries>().AsSelf();
			builder.RegisterType<UserQueries>().AsSelf();

			// Build container.
			var container = builder.Build();

			// Set ASP.NET MVC dependency resolver.
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
			AutofacHostFactory.Container = container;

		}

	}

}