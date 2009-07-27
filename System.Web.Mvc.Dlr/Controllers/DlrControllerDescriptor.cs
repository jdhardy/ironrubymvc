using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc.Dlr.Core;

namespace System.Web.Mvc.Dlr.Controllers
{
    public abstract class DlrControllerDescriptor : ControllerDescriptor
    {
        protected readonly IDlrEngine engine;
        protected readonly IActionMethodSelector selector;

        public DlrControllerDescriptor(IDlrEngine engine, IActionMethodSelector selector)
        {
            this.engine = engine;
            this.selector = selector;
        }

        /// <summary>
        /// Finds the action.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>The information about the action.</returns>
        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            if(controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if(actionName == null)
                throw new ArgumentNullException("actionName");

            var selectedName = selector.FindActionMethod(controllerContext, actionName);
            return !string.IsNullOrEmpty(selectedName) ? GetActionDescriptor(selectedName) : null;
        }

        /// <summary>
        /// Gets the canonical actions.
        /// </summary>
        /// <returns>
        /// A list of action descriptors for the controller.
        /// </returns>
        public override ActionDescriptor[] GetCanonicalActions()
        {
            return selector.GetAllActionMethods().Select(method => GetActionDescriptor(method)).ToArray();
        }

        protected abstract ActionDescriptor GetActionDescriptor(string actionName);
    }
}
