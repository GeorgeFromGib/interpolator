using System;
using System.Collections;

namespace SimpleInterpolator.Actions
{
    internal class ForLoopAction<T> : IAction
    {
        private Func<string, T, string> _action;
        private TemplatesDict _templateDict;
        private string _templateName;
        private IEnumerable _records;

        public ForLoopAction(TemplatesDict templateDict, string templateName, IEnumerable records, Func<string, T, string> action)
        {
            _action = action;
            _templateDict = templateDict;
            _templateName = templateName;
            _records = records;
        }
        public string Invoke()
        {
            var result = "";
            if (_templateDict.TryGetValue(_templateName, out var innerTemplate))
            {
                foreach (T e in _records)
                {
                    result += _action.Invoke(innerTemplate,e);
                }
            }
            return result;
        }
    }
}