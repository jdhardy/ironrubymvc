using System;
using Microsoft.Scripting.Hosting;

namespace System.Web.Mvc.Dlr.Core
{
    public abstract class DlrEngine : IDlrEngine
    {
        public DlrEngine(ScriptRuntime runtime, IPathProvider pathProvider, string routesPath)
        {
            //_routesPath = routesPath;
            Runtime = runtime;
            PathProvider = pathProvider;
            //Initialize();
        }

        public DlrEngine(ScriptRuntime runtime, IPathProvider pathProvider)
            : this(runtime, pathProvider, "~/routes.rb")
        {
        }

        private ScriptScope CurrentScope { get; set; }

        private IPathProvider PathProvider { get; set; }

        #region IDlrEngine Members

        public ScriptRuntime Runtime { get; set; }

        public ScriptEngine ScriptEngine { get; set; }

        #endregion
    }
}
