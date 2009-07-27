using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc.Dlr.Core;

namespace System.Web.Mvc.Dlr.Controllers
{
    public abstract class DlrController : Controller
    {
        protected override void Execute(RequestContext requestContext)
        {
            ActionInvoker = GetActionInvoker();
            base.Execute(requestContext);
        }

        protected abstract IActionInvoker GetActionInvoker();
    }
}
