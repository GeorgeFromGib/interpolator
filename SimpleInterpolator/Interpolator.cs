using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleInterpolator.Actions;

namespace SimpleInterpolator
{
    public class Interpolator
    {
        private string _regexString = @"{{(\w+)}}";
        private bool _ignoreCase = true;
        private string _tplteRegex = @"\#--(\w+):(.*)--\#";
        private readonly DirectivesDict _directives = new DirectivesDict();
        private readonly TemplatesDict _templatesDict = new TemplatesDict();

        public Interpolator For(string target, Func<string> action)
        {
            _directives.Add(target, new FunctionAction(action));
            return this;
        }

        public Interpolator For(string target, string value)
        {
            return For(target, () => value);
        }

        public Interpolator For(string target, string templateName, Func<string, string> action)
        {
            _directives.Add(target, new TemplateAction(_templatesDict, templateName, action));
            return this;
        }

         public Interpolator For<T>(string target, string templateName,ICollection<T> records, Func<string, T,string> action)
        {
            _directives.Add(target, new ForLoopAction<T>(_templatesDict, templateName, records, action));
            return this;
        }

        public Interpolator SetDelimiters(string preDelimiter, string postDelimiter)
        {
            _regexString = $@"{preDelimiter}(\w+){postDelimiter}";
            return this;
        }

        public Interpolator IgnoreCase(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
            return this;
        }

        public string Parse(string template)
        {
            var directives=_ignoreCase ? ConvertDirectivesToCaseInsesitive(_directives) : _directives;
            var parsedTemplate = template;
            parsedTemplate = ScrubTemplates(parsedTemplate,_templatesDict, _tplteRegex);
            parsedTemplate = Interpolate(parsedTemplate,directives, _regexString);
            return parsedTemplate;
        }

        private string ScrubTemplates(string template, TemplatesDict templatesDict, string regex)
        {
            var scrubedTemplate = Regex.Replace(template, regex, matches =>
            {
                var key = matches.Groups[1].Value;
                var innerTplte = matches.Groups[2].Value;
                templatesDict.Add(key, innerTplte);
                return "";
            },RegexOptions.Singleline);
            
            return scrubedTemplate;
        }

        private string Interpolate(string template,DirectivesDict directives, string regex)
        {
            return Regex.Replace(template, regex, match => GetNewValue(match, directives));
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