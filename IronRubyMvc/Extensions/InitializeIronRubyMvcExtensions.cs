﻿namespace IronRubyMvc {
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Scripting.Hosting;
    using IronRuby;
    using IronRuby.Runtime;

    public static class InitializeIronRubyMvcExtensions {
        public static void InitializeIronRubyMvc(this HttpApplication app) {
            InitializeIronRubyMvc(app, HostingEnvironment.VirtualPathProvider);
        }

        public static void InitializeIronRubyMvc(this HttpApplication app, VirtualPathProvider vpp) {
            InitializeIronRubyMvc(app, vpp, "~/routes.rb");
        }

        public static void InitializeIronRubyMvc(this HttpApplication app, VirtualPathProvider vpp, string routesPath) {
            //var langSetup = Ruby.CreateLanguageSetup();
            var setup = Ruby.CreateRuntimeSetup();
            setup.DebugMode = true;
            ScriptRuntime runtime = Ruby.CreateRuntime(setup);

            app.Application.SetScriptRuntime(runtime);

            if (vpp.FileExists(routesPath))
                ProcessRubyRoutes(runtime, vpp, routesPath);

            var factory = new RubyControllerFactory(ControllerBuilder.Current.GetControllerFactory());
            ControllerBuilder.Current.SetControllerFactory(factory);
            ViewEngines.Engines.Add(new RubyViewEngine());
        }

        static void ProcessRubyRoutes(ScriptRuntime runtime, VirtualPathProvider vpp, string routesPath) {
            var routeColection = new RubyRouteCollection(RouteTable.Routes);

            ScriptEngine rubyEngine = Ruby.GetEngine(runtime);
            RubyExecutionContext rubyContext = Ruby.GetExecutionContext(runtime);

            rubyContext.DefineReadOnlyGlobalVariable("routes", routeColection);

            // REVIEW: Should we pull this information from the loaded versions of these assemblies?
            string header = @"
require 'System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
require 'System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
require 'System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
            ";

            rubyEngine.CreateScriptSourceFromString(header).Execute();

            using (var stream = vpp.GetFile(routesPath).Open())
            using (var reader = new StreamReader(stream)) {
                string routesText = reader.ReadToEnd();
                rubyEngine.CreateScriptSourceFromString(routesText).Execute();
            }
        }
    }
}
