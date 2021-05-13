using System;
using System.Text.RegularExpressions;
using SimpleInterpolator.Actions;

namespace SimpleInterpolator
{
    public class TemplateInterpolator
    {
        private string _regexString = @"{{(\w+)}}";
        private bool _ignoreCase = true;
        private string _tplteRegex = @"\#--(\w+):(.*)--\#";
        private readonly DirectivesDict _directives = new DirectivesDict();
        private readonly TemplatesDict _templatesDict = new TemplatesDict();

        public TemplateInterpolator For(string target, Func<string> action)
        {
            _directives.Add(target, new FunctionAction(action));
            return this;
        }

        public TemplateInterpolator For(string target, string value)
        {
            return For(target, () => value);
        }

        public TemplateInterpolator For(string target, string templateName, Func<string, string> action)
        {
            _directives.Add(target, new TemplateAction(_templatesDict, templateName, action));
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
            var parsedTemplate = template;
            parsedTemplate = ScrubTemplates(parsedTemplate);
            parsedTemplate = Interpolate(parsedTemplate);
            return parsedTemplate;
        }

        private string ScrubTemplates(string template)
        {
            var scrubedTemplate = Regex.Replace(template, _tplteRegex, matches =>
            {
                var key = matches.Groups[1].Value;
                var innerTplte = matches.Groups[2].Value;
                _templatesDict.Add(key, innerTplte);
                return "";
            });
            return scrubedTemplate;
        }

        private string Interpolate(string template)
        {
            var directives = _ignoreCase ? ConvertDirectivesToCaseInsesitive(_directives) : _directives;
            return Regex.Replace(template, _regexString, match => GetNewValue(match, directives));
        }

        private DirectivesDict ConvertDirectivesToCaseInsesitive(DirectivesDict directives)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return new DirectivesDict(directives, comparer);
        }

        private string GetNewValue(Match match, DirectivesDict directives)
        {
            var fieldName = match.Groups[1].Value;
            if(directives.TryGetValue(fieldName, out var action))
                return action.Invoke();

            var originalValue = match.Groups[0].Value;
            return originalValue;
        }

    }
}