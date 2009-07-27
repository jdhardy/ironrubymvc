﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace System.Web.Mvc.Dlr.Core
{
    public interface IDlrEngine
    {
        ScriptRuntime Runtime { get; }
        ScriptEngine ScriptEngine { get; }
    }
}
