extern alias clr3;
#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Mvc.Dlr.Extensions;
using IronRuby.Builtins;
using clr3::System.Linq;
using System.Web.Mvc.Dlr.Controllers;
#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    /// <summary>
    /// The descriptor for a Ruby enabled Controller
    /// </summary>
    public class RubyControllerDescriptor : DlrControllerDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RubyControllerDescriptor"/> class.
        /// </summary>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="engine">The engine.</param>
        public RubyControllerDescriptor(RubyClass rubyClass, IRubyEngine engine)
            : base(engine, new RubyActionMethodSelector(engine, rubyClass))
        {
            RubyControllerClass = rubyClass;
        }

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <value>The name of the controller.</value>
        public override string ControllerName
        {
            get { return RubyControllerClass.Name; }
        }


        /// <summary>
        /// Gets the type of the controller.
        /// </summary>
        /// <value>The type of the controller.</value>
        public override Type ControllerType
        {
            get { return typeof (RubyController); }
        }

        /// <summary>
        /// Gets or sets the class ruby controller.
        /// </summary>
        /// <value>The ruby controller class.</value>
        public RubyClass RubyControllerClass { get; private set; }

        protected override ActionDescriptor GetActionDescriptor(string actionName)
        {
            return new RubyActionDescriptor(actionName, this, (IRubyEngine)engine);
        }
    }
}