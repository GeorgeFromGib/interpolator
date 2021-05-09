using System;

namespace SimpleInterpolator
{
    public static class InterpolatorHelper
    {
        public static string Interpolate(this string template, Action<TemplateInterpolator> actions)
        {
            var itpl = new TemplateInterpolator();
            actions?.Invoke(itpl);
            return itpl.Parse(template);
        }
    }
}