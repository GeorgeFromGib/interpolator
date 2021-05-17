using System;

namespace SimpleInterpolator.Actions
{
    internal class TemplateAction : IAction
    {
        private Func<string, string> _action;
        private TemplatesDict _templateDict;
        private string _templateName;
        public TemplateAction(TemplatesDict templateDict, string templateName, Func<string, string> action)
        {
            _action = action;
            _templateDict = templateDict;
            _templateName = templateName;
        }
        public string Invoke()
        {
            if (_templateDict.TryGetValue(_templateName, out var innerTemplate))
                return _action.Invoke(innerTemplate);
            return "";
        }
    }
}