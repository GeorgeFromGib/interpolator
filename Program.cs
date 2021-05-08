using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace interpolator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var template = "Item '{{product}}' priced {{price}} will be shipped on the {{shippingDate}}";
            var template = "Item\t\tPrice\n" +
                        "-------------------------\n" +
                        "#--productList:{{product}}\t{{price}}--#\n";

            var convTmplt = template.Interpolate(o => o
                .ForTemplate("productList",(t)=>{
                    return t.Interpolate(t=>t
                    .For("product","Keyboard")
                    .For("price","$300"));
                    })
                .For("product", () => "Gaming Product")
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
            return itpl.Interpolate(template);
        }
    }

    class DirectivesDict:Dictionary<string,Func<string,string>>
    {
        public DirectivesDict() {}
        public DirectivesDict(DirectivesDict dict, StringComparer comparer ):base(dict,comparer) {}  
    }

    public class TemplateInterpolator
    {
        private string _regexString = @"{{(\w+)}}";
        private bool _ignoreCase = true;
        private string _tplteRegex = @"\#--(\w+):(.*)--\#";
        private readonly DirectivesDict _directives = new DirectivesDict();
    
        public TemplateInterpolator For(string target, Func<string> action)
        {
            _directives.Add(target, (_)=>action.Invoke());
            return this;
        }

    
        public TemplateInterpolator For(string target, string value)
        {
            return For(target,()=>value);
        }

        public TemplateInterpolator ForTemplate(string templateName, Func<string,string> action)
        {
            _directives.Add(templateName, action);
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

       


        public string Interpolate(string template)
        {
            var parsedTemplate = InterpolateTemplates(template);
            parsedTemplate = InterpolateFieldNames(parsedTemplate);
            return parsedTemplate;
        }

         private string InterpolateTemplates(string template)
        {
            var regExInstance = new Regex(_tplteRegex, RegexOptions.Compiled);
            var result = regExInstance.Replace(template, m =>
            {
                var loopName = m.Groups[1].Value;
                var template=m.Groups[2].Value;
                // var found = _loopDirectives.TryGetValue(loopName, out var actions);
                // if (found)
                // {
                //     return "";
                // }
                return "";
            });

            return result;
        }

        private string InterpolateFieldNames(string template)
        {
            var regExInstance = new Regex(_regexString, RegexOptions.Compiled);
            var directives = _ignoreCase ? ConvertDirectivesToCaseInsesitive(_directives) : _directives;
            return regExInstance.Replace(template, match => GetNewValue(match, directives, template));
        }

        private DirectivesDict ConvertDirectivesToCaseInsesitive(DirectivesDict directives)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return new DirectivesDict(directives, comparer);
        }

        private string GetNewValue(Match match, DirectivesDict directives, string template)
        {
            var fieldName = match.Groups[1].Value;
            var found = directives.TryGetValue(fieldName, out var action);
            if (found)
            {
                return action(template);
            }
            var originalValue = match.Groups[0].Value;
            return originalValue;
        }

    }
}
