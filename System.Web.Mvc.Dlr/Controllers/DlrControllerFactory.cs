using System.Web.Routing;

using System.Web.Mvc.Dlr.Core;
using System.Web.Mvc.Dlr.Extensions;

namespace System.Web.Mvc.Dlr.Controllers
{
    public abstract class DlrControllerFactory : IControllerFactory
    {
        private readonly IControllerFactory _innerFactory;
        private readonly IPathProvider _pathProvider;

        public DlrControllerFactory(IPathProvider pathProvider, IControllerFactory innerFactory)
        {
            _pathProvider = pathProvider;
            _innerFactory = innerFactory;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            try
            {
                return _innerFactory.CreateController(requestContext, controllerName);
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
        protected abstract IController LoadController(RequestContext requestContext, string controllerName);

        /// <summary>
        /// Gets the name of the controller class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        protected static string GetControllerClassName(string controllerName)
        {
            return (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                        ? controllerName
                        : Constants.ControllerclassFormat.FormattedWith(controllerName)).Pascalize();
        }

        protected string GetControllerFilePath(string controllerName)
        {
            var fileName = Constants.ControllerPascalPathFormat.FormattedWith(controllerName.Pascalize());
            if(_pathProvider.FileExists(fileName))
                return fileName;

            fileName = Constants.ControllerUnderscorePathFormat.FormattedWith(controllerName.Underscore());

            return _pathProvider.FileExists(fileName) ? fileName : string.Empty;
        }
    }
}