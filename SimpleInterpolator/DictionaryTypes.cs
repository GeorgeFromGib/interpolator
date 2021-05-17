using System;
using System.Collections.Generic;
using SimpleInterpolator.Actions;

namespace SimpleInterpolator
{
    internal class DirectivesDict : Dictionary<string, IAction>
    {
        public DirectivesDict() { }
        public DirectivesDict(DirectivesDict dict, StringComparer comparer) : base(dict, comparer) { }
    }

    internal class TemplatesDict : Dictionary<string, string>
    { }
}