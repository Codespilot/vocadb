﻿using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code;
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
			builder.RegisterType<ServiceModel>().AsSelf();
			builder.RegisterType<AdminService>().AsSelf();
			builder.RegisterType<AlbumService>().AsSelf();

			// Build container.
			var container = builder.Build();

			// Set ASP.NET MVC dependency resolver.
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
			AutofacHostFactory.Container = container;

		}

	}

}