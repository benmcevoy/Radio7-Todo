using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Radio7.Unity.Decorators;
using Owin;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json.Serialization;

namespace Radio7.Todo.Server.Infrastructure
{
    public class Startup
    {
        public void Register(IAppBuilder appBuilder)
        {
            var config = GlobalConfiguration.Configuration;

            config.MapHttpAttributeRoutes();
            config.EnableCors();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Filters.Add(new ApiSessionAuthorizationFilter());

            var container = new UnityContainer();
            var assembliesToScan = new[] { Assembly.GetExecutingAssembly() };

            // register all types from these libraries
            // using a per-instance lifestyle.
            container.RegisterTypes(
                AllClasses.FromAssemblies(assembliesToScan),
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.PerResolve,
                null,
                true);

            container.RegisterDecorated(assembliesToScan);

            config.Services.Replace(typeof(IHttpControllerActivator), new ServiceActivator(config, container));

            config.EnsureInitialized();
        }
    }

    public class ServiceActivator : IHttpControllerActivator
    {
        private readonly IUnityContainer _container;

        public ServiceActivator(HttpConfiguration configuration, IUnityContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request
            , HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = _container.Resolve(controllerType) as IHttpController;
            return controller;
        }
    }
}