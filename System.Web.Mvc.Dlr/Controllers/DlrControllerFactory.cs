using System.Web.Routing;

using System.Web.Mvc.Dlr.Core;
using System.Web.Mvc.Dlr.Extensions;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;

namespace System.Web.Mvc.Dlr.Controllers
{
    public abstract class DlrControllerFactory : IControllerFactory
    {
        private readonly IControllerFactory innerFactory;
        private readonly IPathProvider pathProvider;
        protected readonly IDlrContext context;

        public DlrControllerFactory(IPathProvider pathProvider, IControllerFactory innerFactory, IDlrContext context)
        {
            this.pathProvider = pathProvider;
            this.innerFactory = innerFactory;
            this.context = context;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            try
            {
                return innerFactory.CreateController(requestContext, controllerName);
            }
            catch(InvalidOperationException)
            {
            }
            catch(HttpException)
            {
            }

            return LoadController(requestContext, controllerName);
        }


        public void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;

            if(disposable != null)
                disposable.Dispose();
        }

        #endregion

        /// <summary>
        /// Loads the controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        protected virtual IController LoadController(RequestContext requestContext, string controllerName)
        {
            string controllerFilePath = GetControllerFilePath(controllerName);
            string controllerClassName = GetControllerClassName(controllerName);

            if(controllerFilePath.IsNullOrBlank())
                return null;

            ScriptScope scope = context.ScriptEngine.ExecuteFile(requestContext.HttpContext.Server.MapPath(controllerFilePath));
            
            object controllerClass;
            return scope.TryGetVariable(controllerClassName, out controllerClass) ?
                this.CreateControllerInstance(controllerClass) :
                null;
        }

        /// <summary>
        /// Create an instance of the specified controller class.
        /// </summary>
        /// <param name="controllerClass"></param>
        /// <returns></returns>
        protected virtual IController CreateControllerInstance(object controllerClass)
        {
            return (IController)context.ScriptEngine.Operations.CreateInstance(controllerClass);
        }

        /// <summary>
        /// Gets the name of the controller class with default formatting.
        /// 
        /// By default, this is [controllerName]Controller; i.e. 'home' would become
        /// 'HomeController'.
        /// 
        /// If controllerName ends with 'Controller', it is used unchanged.
        /// 
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns>The expected controller class name.</returns>
        protected virtual string GetControllerClassName(string controllerName)
        {
            return (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                        ? controllerName
                        : "{0}Controller".FormattedWith(controllerName)).Pascalize();
        }

        /// <summary>
        /// An Enumerable of all possible paths to controller files.
        /// 
        /// By default, this is ~/Controllers/[controllerName]Controller.[ext], for all
        /// extensions supported by the current language.
        /// 
        /// For example, a controller name of 'home' in Python would return
        /// ['~/Controllers/HomeController.py'].
        /// 
        /// Language implementations should override this first if they need to customize controller lookup.
        /// 
        /// </summary>
        /// <param name="controllerName">The name of the controller, as determined by routing.</param>
        /// <returns>Enumerable of possible controller paths.</returns>
        protected virtual IEnumerable<string> GetPossibleControllerPaths(string controllerName)
        {
            foreach(string ext in context.ScriptEngine.Setup.FileExtensions)
                yield return "~/Controllers/{0}Controller{1}".FormattedWith(controllerName.Pascalize(), ext.StartsWith(".") ? ext : "." + ext);
        }

        /// <summary>
        /// The path to the controller file that should contain the specified controller.
        /// If no such file exists, returns null.
        /// 
        /// Files to be searched are determined by GetPossibleControllerPaths.
        /// 
        /// </summary>
        /// <param name="controllerName">The name of the controller, as determined by routing.</param>
        /// <returns>The path to a file that should contain the specified controller, or null if no file exists.</returns>
        protected virtual string GetControllerFilePath(string controllerName)
        {
            foreach(string path in GetPossibleControllerPaths(controllerName))
                if(pathProvider.FileExists(path))
                    return path;

            return null;
        }
    }
}