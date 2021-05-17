using System;

namespace SimpleInterpolator.Actions
{
    internal class FunctionAction : IAction
    {
        private Func<string> _action;
        public FunctionAction(Func<string> action)
        {
            _action = action;
        }

        public string Invoke()
        {
            return _action.Invoke();
        }
    }
}