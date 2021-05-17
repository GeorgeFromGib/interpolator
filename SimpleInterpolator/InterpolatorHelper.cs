using System;

namespace SimpleInterpolator
{
    public static class InterpolatorHelper
    {
        public static string Interpolate(this string template, Action<Interpolator> actions)
        {
            var itpl = new Interpolator();
            actions?.Invoke(itpl);
            return itpl.Parse(template);
        }
    }
}