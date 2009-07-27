using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web.Mvc.Dlr.Controllers
{
    public interface IActionMethodSelector
    {
        string FindActionMethod(ControllerContext controllerContext, string actionName);
        IEnumerable<string> GetAllActionMethods();
    }
}
