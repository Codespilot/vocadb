using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using Autofac.Integration.WebApi;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Repositories.NHibernate;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Security;
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
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

			builder.Register(x => DatabaseConfiguration.BuildSessionFactory()).SingleInstance();

			// Other dependencies (for repositories mostly)
			builder.RegisterType<LoginManager>().As<IUserPermissionContext>();
			builder.Register(x => new EntryAnchorFactory(AppConfig.HostAddress)).As<IEntryLinkFactory>();
			builder.RegisterType<StopForumSpamClient>().As<IStopForumSpamClient>();
			builder.RegisterType<PVParser>().As<IPVParser>();
			builder.RegisterType<ServerEntryImagePersisterOld>().As<IEntryImagePersisterOld>();
			builder.RegisterType<ServerEntryThumbPersister>().As<IEntryThumbPersister>();

			// Legacy services
			builder.RegisterType<ServiceModel>().AsSelf();
			builder.RegisterType<AdminService>().AsSelf();
			builder.RegisterType<AlbumService>().AsSelf();
			builder.RegisterType<ArtistService>().AsSelf();
			builder.RegisterType<SongService>().AsSelf();
			builder.RegisterType<TagService>().AsSelf();
			builder.RegisterType<UserService>().AsSelf();

			// Repositories
			builder.RegisterType<AlbumNHibernateRepository>().As<IAlbumRepository>();
			builder.RegisterType<ArtistNHibernateRepository>().As<IArtistRepository>();
			builder.RegisterType<EntryReportNHibernateRepository>().As<IEntryReportRepository>();
			builder.RegisterType<SongNHibernateRepository>().As<ISongRepository>();
			builder.RegisterType<SongListNHibernateRepository>().As<ISongListRepository>();
			builder.RegisterType<TagNHibernateRepository>().As<ITagRepository>();
			builder.RegisterType<UserNHibernateRepository>().As<IUserRepository>();
			builder.RegisterType<UserMessageNHibernateRepository>().As<IUserMessageRepository>();
			builder.RegisterType<AlbumQueries>().AsSelf();
			builder.RegisterType<ArtistQueries>().AsSelf();
			builder.RegisterType<EntryReportQueries>().AsSelf();
			builder.RegisterType<SongQueries>().AsSelf();
			builder.RegisterType<SongListQueries>().AsSelf();
			builder.RegisterType<TagQueries>().AsSelf();
			builder.RegisterType<UserQueries>().AsSelf();
			builder.RegisterType<UserMessageQueries>().AsSelf();

			// Build container.
			var container = builder.Build();

			// Set ASP.NET MVC dependency resolver.			
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); // For ASP.NET MVC			
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container); // For Web API			
			AutofacHostFactory.Container = container; // For WCF

		}

	}

}