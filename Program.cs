using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace interpolator
{
    class Program
    {
        static void Main(string[] args)
        {
            var template = "Item '{{product}}' priced {{price}} will be shipped on the {{shippingDate}}";

            var convTmplt = template.Interpolate(o => o
                .For("product", () => "Keyboard")
                .For("price", () => "$200")
                .For("shippingDate", () => DateTime.Today.ToShortDateString()));


            Console.WriteLine(convTmplt);

        }
    }

    public static class InterpolatorHelper
    {
        public static string Interpolate(this string template, Action<TemplateInterpolator> actions)
        {
            var itpl = new TemplateInterpolator();
            actions?.Invoke(itpl);
            return itpl.Parse(template);
        }
    }

    public class TemplateInterpolator
    {
        private string _regexString = @"{{(\w+)}}";
        private bool _ignoreCase = true;

        private readonly Dictionary<string, Func<string>> _directives =
            new Dictionary<string, Func<string>>();

        public TemplateInterpolator For(string target, Func<string> action)
        {
            _directives.Add(target, action);
            return this;
        }

        public TemplateInterpolator SetDelimiters(string preDelimiter, string postDelimiter)
        {
            _regexString = $@"{preDelimiter}(\w+){postDelimiter}";
            return this;
        }

        public TemplateInterpolator IgnoreCase(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
            return this;
        }

        public string Parse(string template)
        {
            var directives = _ignoreCase ? ConvertDirectivesToCaseInsesitive(_directives) : _directives;
            return Regex.Replace(template, _regexString,match => GetNewValue(match, directives));
        }

        private Dictionary<string, Func<string>> ConvertDirectivesToCaseInsesitive(Dictionary<string, Func<string>> directives)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return new Dictionary<string, Func<string>>(directives, comparer);
        }

        private string GetNewValue(Match match, Dictionary<string, Func<string>> directives)
        {
            var fieldName = match.Groups[1].Value;
            var found = directives.TryGetValue(fieldName, out var action);
            if (found)
            {
                return action();
            }
            var originalValue = match.Groups[0].Value;
            return originalValue;
        }
    }
}
