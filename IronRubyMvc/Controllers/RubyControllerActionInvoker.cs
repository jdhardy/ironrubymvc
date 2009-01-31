﻿using System.Web.Mvc;
using IronRubyMvc.Core;

namespace IronRubyMvc
{
    internal class RubyControllerActionInvoker : ControllerActionInvoker
    {
        public RubyControllerActionInvoker(string controllerName)
        {
            ControllerName = controllerName;
        }

        public string ControllerName { get; private set; }

        public RubyMvcEngine Engine { get; private set; }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            return new RubyControllerDescriptor(ControllerName);
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            return controllerDescriptor.FindAction(controllerContext, actionName);
        }


        protected override object GetParameterValue(ControllerContext controllerContext,
                                                    ParameterDescriptor parameterDescriptor)
        {
            return parameterDescriptor.ParameterName == "__action"
                       ? ((RubyParameterDescriptor) parameterDescriptor).Action
                       : base.GetParameterValue(controllerContext, parameterDescriptor);
        }


        private static string PascalCaseIt(string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
    }
}