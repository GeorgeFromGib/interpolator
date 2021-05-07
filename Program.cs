﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace interpolator
{
   class Program
    {
        static void Main(string[] args)
        {
            var template = "Item '{{product}}' priced {{price}} will be shipped on the {{shippingDate}}";

            var interpolator=new TemplateInterpolator();

            interpolator.Replace("product", () => "Gaming Product");
            interpolator.Replace("price",()=>"$200");
            interpolator.Replace("shippingDate",()=>DateTime.Today.ToShortDateString());

            var result=interpolator.Interpolate(template);

            Console.WriteLine(result);

        }
    }

    public class TemplateInterpolator
    {
        private string _regexString = @"{{(\w+)}}";
        private bool _ignoreCase = true;

        private readonly Dictionary<string, Func<string>> _directives = 
            new Dictionary<string, Func<string>>();

        public TemplateInterpolator Replace(string target, Func<string> action)
        {
            _directives.Add(target, action);
            return this;
        }

        public TemplateInterpolator SetFieldsRegex(string regexString)
        {
            _regexString = regexString;
            return this;
        }

        public TemplateInterpolator IgnoreCase(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
            return this;
        }

        public string Interpolate(string template)
        {
            var regExInstance = new Regex(_regexString, RegexOptions.Compiled );
            var directives=_ignoreCase?ConvertDirectivesToCaseInsesitive(_directives):_directives;
            return regExInstance.Replace(template,match => GetNewValue(match,directives) );
        }

        private Dictionary<string,Func<string>> ConvertDirectivesToCaseInsesitive( Dictionary<string,Func<string>> directives)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return new Dictionary<string, Func<string>>(directives, comparer);
        }

        private string GetNewValue( Match match, Dictionary<string,Func<string>> directives)
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