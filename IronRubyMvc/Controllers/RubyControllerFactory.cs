#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;
using IronRuby.Builtins;
using System.Web.Mvc.Dlr.Controllers;
using System.Web.Mvc.Dlr.Core;
using System.Web.Mvc.Dlr.Extensions;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyControllerFactory : DlrControllerFactory
    {
        private readonly IRubyEngine _engine;

        public RubyControllerFactory(IPathProvider pathProvider, IControllerFactory innerFactory, IRubyEngine engine)
            : base(pathProvider, innerFactory, engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Loads the controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        protected override IController LoadController(RequestContext requestContext, string controllerName)
        {
            var controllerFilePath = GetControllerFilePath(controllerName);
            var controllerClassName = GetControllerClassName(controllerName);

            _engine.RemoveClassFromGlobals(controllerClassName);

            if(controllerFilePath.IsNullOrBlank())
                return null;

            _engine.RequireRubyFile(controllerFilePath, ReaderType.File);

            var controllerClass = _engine.GetRubyClass(controllerClassName);
            var controller = ConfigureController(controllerClass, requestContext);

            return controller;
        }

        /// <summary>
        /// Configures the controller.
        /// </summary>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        private RubyController ConfigureController(RubyClass rubyClass, RequestContext requestContext)
        {
            var controller = (RubyController)_engine.CreateInstance(rubyClass);
            controller.InternalInitialize(new ControllerConfiguration {Context = requestContext, Engine = _engine, RubyClass = rubyClass});
            return controller;
        }
    }
}
