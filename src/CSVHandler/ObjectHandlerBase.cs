//********************************************************************************************************************************
//
//DotYexLibary Version 12.1.6.6.2020.11.20
//
//Please read the ReadMe File and Documentation
//
//DotYexLibrary is created by YeGaSoft (YeXtaiZ Games and Software Studio)
//
//********************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.IO;
using System.Globalization;

namespace Zeiss.PublicationManager.Data.ObjectHandler
{
    public abstract class ObjectHandlerBase
    {
        #region Converter
        protected static List<string> ConvertToListxString(object convertObject)
        {
            //convertObject = new string("TEST");
            return convertObject switch
            {
                string objstr => ConvertToListxString(objstr),
                string[] objstrarr => ConvertToListxString(objstrarr),
                List<string> objstrlst => ConvertToListxString(objstrlst),

                _ => throw new InvalidCastException("Cannot convert to List<string>, because type of 'convertObject' was invalid.\n" +
                    "Only 'string', 'string[]' and 'List<string>' are accepted"),
            };
        }
        protected static List<string> ConvertToListxString(string convertObject)
        {
            return new List<string>() { convertObject };
        }
        protected static List<string> ConvertToListxString(string[] convertObject)
        {
            return new List<string>(convertObject.Select(x => x.Clone() as string));
        }
        protected static List<string> ConvertToListxString(List<string> convertObject)
        {
            return new List<string>(convertObject.Select(x => x.Clone() as string));
        }


        protected static List<List<string>> ConvertToListxListxString(object convertObject)
        {
            if ((convertObject is string)
                    || (convertObject is string[])
                    || (convertObject is List<string>))
            {
                return new List<List<string>>() { ConvertToListxString(convertObject) };
            }

            else if (convertObject is List<List<string>> objcolvallststrlst)
            {
                return ConvertToListxListxString(objcolvallststrlst);
            }
            else
                throw new InvalidCastException("Cannot convert to List<string>, because type of 'whereInColumnValues' was invalid.\n" +
                    "Only 'string', 'string[]' and 'List<string>' are accepted");
        }
        protected static List<List<string>> ConvertToListxListxString(List<List<string>> convertObject)
        {
            return new List<List<string>>
                    (
                        convertObject.Select
                        (
                            (x, index)
                            => new List<string>(convertObject[index].Select
                            (y => y.Clone() as string))
                        )
                    );
        }


        //Experimental Code

        protected static List<object> ConvertToListxObject(object convertObject)
        {
            if (convertObject is object[] coarr)
                return ConvertToListxObject(coarr);
            else if (convertObject is List<object> colis)
                return ConvertToListxObject(colis);

            return new List<object>() { convertObject };
        }
        protected static List<object> ConvertToListxObject(object[] convertObject)
        {
            List<object> copyList = new();
            foreach (var obj in convertObject)
            {
                copyList.Add(obj);
            }

            return copyList;
        }
        protected static List<object> ConvertToListxObject(List<object> convertObject)
        {
            List<object> copyList = new();
            foreach (var obj in convertObject)
            {
                copyList.Add(obj);
            }

            return copyList;
        }


        protected static List<List<object>> ConvertToListxListxObject(object convertObject)
        {
            if (convertObject is List<List<object>> objcolvallststrlst)
            {
                return ConvertToListxListxObject(objcolvallststrlst);
            }
            else if ((convertObject is object)
                    || (convertObject is object[])
                    || (convertObject is List<object>))
            {
                List<List<object>> copyList = new();
                copyList.Add(ConvertToListxObject(convertObject));

                return copyList;
            }
            else
                throw new InvalidCastException("Cannot convert to List<string>, because type of 'convertObject' was invalid.\n" +
                    "Only 'string', 'string[]' and 'List<string>' are accepted");
        }
        protected static List<List<object>> ConvertToListxListxObject(List<List<object>> convertObject)
        {
            List<List<object>> copyList = new();
            foreach (var objLst in convertObject)
            {
                copyList.Add(ConvertToListxObject(objLst));
            }

            return copyList;
        }


        protected virtual string ConvertToFormattedString(object convertObject, CultureInfo cultureFormat = null)
        {
            if (cultureFormat is null)
                cultureFormat = CultureInfo.InvariantCulture;

            switch (convertObject)
            {
                case DateTime condate:
                    return condate.ToString(cultureFormat);

                case bool conbool:
                    return conbool.ToString(cultureFormat);

                case string constr:
                    return constr;

                default:
                    if (Decimal.TryParse(convertObject.ToString(), out decimal objdec))
                        return objdec.ToString(cultureFormat);
                    else
                        throw new ArgumentException("Unable to convert entered object into a formatted string.\n" +
                            "The entered object is not a date, number, boolean or a string");
            }
        }
        //Experimental Code END
        #endregion

        #region Delegates
        public delegate T DelegateFunction<T>(params object[] oArgs);

        public delegate void DelegateFunction(params object[] oArgs);

        public static T ExecuteDelegateFunction<T>(DelegateFunction<T> function)
        {
            T result = function();
            return result;
        }
        public static void ExecuteDelegateFunction(DelegateFunction function)
        {
            function();
        }
        #endregion

        #region Exceptions
        protected virtual void RethrowInnerException(Exception outerException)
        {
            if (outerException.InnerException == null)
                throw new Exception(outerException.Message);

            else if (outerException.InnerException is UriFormatException)
                throw new UriFormatException(outerException.Message);
            else if (outerException.InnerException is SecurityException)
                throw new SecurityException(outerException.Message);
            else if (outerException.InnerException is PathTooLongException)
                throw new PathTooLongException(outerException.Message);
            else if (outerException.InnerException is FileNotFoundException)
                throw new FileNotFoundException(outerException.Message);
            else if (outerException.InnerException is EncoderFallbackException)
                throw new EncoderFallbackException(outerException.Message);
            else if (outerException.InnerException is ArgumentNullException)
                throw new ArgumentNullException(outerException.Message);
            else if (outerException.InnerException is ArgumentException)
                throw new ArgumentException(outerException.Message);
            else if (outerException.InnerException is InvalidCastException)
                throw new InvalidCastException(outerException.Message);
            else if (outerException.InnerException is InvalidOperationException)
                throw new InvalidOperationException(outerException.Message);
            else if (outerException.InnerException is IOException)
                throw new IOException(outerException.Message);
            else
                throw new Exception(outerException.Message);
        }
        #endregion

        #region IO
        //Does check, if the filepath does exist
        protected static bool CheckPathExist(ref string filepath)
        {
            CheckAndConvertLongFilePath(ref filepath);

            //If the path exists, it returns true and other functions can work further
            return (File.Exists(filepath));
        }

        protected static void CheckAndConvertLongFilePath(ref string filepath)
        {
            //Checks for longer filepaths (MAX_PATH is regularly 260)
            if (filepath.Length >= 256)
            {
                //Checks if file does not exists or/and if system cannot access it
                if (!File.Exists(filepath))
                {
                    //Adds the prefix to exceed MAX_PATH
                    filepath = @"\\?\" + filepath;

                    //Either file does not exist or prefix is unsupported if true
                    if (!File.Exists(filepath))
                        throw new PathTooLongException("The entered filepath:\n" + filepath +
                            "\nis too long (and current IO API does not support \"" + @"\\?\" + "\") or does not exist");
                }
            }
        }
        #endregion

        #region Sanitizer
        private static List<List<string>> GetMarkupCharacters()
        {
            return new List<List<string>>()
            {
                new List<string> { "'", "&apos;" },
                new List<string> { "\"", "&quot;" },
                new List<string> { "&", "&amp;" },
                new List<string> { "<", "&lt" },
                new List<string> { ">", "&gt" },
            };
        }

        private static List<List<string>> GetSpecialCharactersRiskLevel0()
        {
            //Basically undangerous characters
            return new List<List<string>>(GetSpecialCharactersRiskLevel1())
            {
                new List<string> { " ", "#x20" },
            };
        }

        private static List<List<string>> GetSpecialCharactersRiskLevel1()
        {
            //Common (regular used) (minimal dangerous) characters for databases (and Code injection) that are mostly used in regular (undangerous) context
            return new List<List<string>>(GetSpecialCharactersRiskLevel2())
            {
                new List<string> { "!", "#x21" },
                new List<string> { "%", "#x25" },
                new List<string> { "(", "#x28" },
                new List<string> { ")", "#x29" },

                new List<string> { "*", "#x2A" },
                new List<string> { "+", "#x2B" },
                new List<string> { ",", "#x2C" },
                new List<string> { "-", "#x2D" },
                new List<string> { ".", "#x2E" },
                new List<string> { "/", "#x2F" },
            };
        }

        private static List<List<string>> GetSpecialCharactersRiskLevel2()
        {
            //Dangerous characters for databases and Code injection that are often used in regular (undangerous) context
            return new List<List<string>>(GetSpecialCharactersRiskLevel3())
            {
                new List<string> { ":", "#x3A" },
                new List<string> { ";", "#x3B" },
                new List<string> { "=", "#x3D" },
                new List<string> { "?", "#x3F" },
            };
        }

        private static List<List<string>> GetSpecialCharactersRiskLevel3()
        {
            //Dangerous characters for databases and Code injection
            return new List<List<string>>(GetSpecialCharactersRiskLevel4())
            {
                new List<string> { "[", "#x5B" },
                new List<string> { "\\", "#x5C" },
                new List<string> { "]", "#x5D" },
                new List<string> { "^", "#x5E" },

                new List<string> { "{", "#x7B" },
                new List<string> { "|", "#x7C" },
                new List<string> { "}", "#x7D" },
                new List<string> { "~", "#x7E" },
            };
        }

        private static List<List<string>> GetSpecialCharactersRiskLevel4()
        {
            //Dangerous characters for databases and Markup languages
            return new List<List<string>>
            {
                new List<string> { "\"", "#x22" },
                new List<string> { "&", "#x26" },
                new List<string> { "'", "#x27" },
                new List<string> { "<", "#x3C" },
                new List<string> { ">", "#x3E" },
            };
        }

        protected static List<List<string>> GetSpecialCharactersAtRiskLevel(int riskLevel)
        {
            return riskLevel switch
            {
                0 => GetSpecialCharactersRiskLevel0(),
                1 => GetSpecialCharactersRiskLevel1(),
                2 => GetSpecialCharactersRiskLevel2(),
                3 => GetSpecialCharactersRiskLevel3(),
                4 => GetSpecialCharactersRiskLevel4(),

                _ => GetSpecialCharactersRiskLevel4(),
            };
        }

        private static List<List<string>> GetRegexCharacters()
        {
            return new List<List<string>>(GetSpecialCharactersRiskLevel4())
            {
                new List<string> { ".", "\\." },

                new List<string> { "?", "\\?" },
                new List<string> { "+", "\\+" },
                new List<string> { "*", "\\*" },

                new List<string> { "{", "\\{" },
                new List<string> { "}", "\\}" },
                new List<string> { "[", "\\[" },
                new List<string> { "]", "\\]" },
                new List<string> { "(", "\\(" },
                new List<string> { ")", "\\)" },

                new List<string> { "^", "\\^" },
                new List<string> { "$", "\\$" },

                new List<string> { "\\", "\\\\" },
            };
        }

        private const string UniqueEscapeSequence = "#U;";

        private static List<List<string>> GetSQLWildcards()
        {
            //'#U;' is a unique sequence that'll be replaced (later) with a user (unique) sequence
            return new List<List<string>>()
            {
                new List<string> { "%", UniqueEscapeSequence + "%" },
                new List<string> { "_", UniqueEscapeSequence + "_" },
                new List<string> { "[", UniqueEscapeSequence + "[" },
                new List<string> { "]", UniqueEscapeSequence + "]" },

                //Only in braces
                //new List<string> { "^", UniqueEscapeSequence + "^" },
                //new List<string> { "-", UniqueEscapeSequence + "-" },
            };
        }

        protected static string SanitizeMarkup(string unsanitizedString)
        {
            return SanitizeDefinedCharacters(unsanitizedString, function => GetMarkupCharacters());
        }

        protected static string SanitizeSpecialCharacters(string unsanitizedString, int riskLevel)
        {
            return SanitizeDefinedCharacters(unsanitizedString, function => GetSpecialCharactersAtRiskLevel(riskLevel));
        }

        protected static string SanitizeRegex(string unsanitizedString)
        {
            return SanitizeDefinedCharacters(unsanitizedString, function => GetRegexCharacters());
        }

        protected static string SanitizeSQLWildcards(string unsanitizedString, char uniqueEscapeCharacter)
        {
            return SanitizeDefinedCharacters(unsanitizedString, function => GetSQLWildcards()).Replace(UniqueEscapeSequence, uniqueEscapeCharacter.ToString());
        }

        protected static string SanitizeDefinedCharacters(string unsanitizedString, DelegateFunction<List<List<string>>> sanitizeConverter)
        {
            string sanitizedString = unsanitizedString;
            List<List<string>> converter = sanitizeConverter();
            for (int i = 0; i < converter[0].Count; i++)
                sanitizedString = sanitizedString.Replace(converter[i][0], converter[i][1]);

            return sanitizedString;
        }

        //All defiened characters will be removed
        protected static string RemoveDefinedChartacters(string unsanitizedString, DelegateFunction<List<List<string>>> sanitizeConverter)
        {
            string sanitizedString = unsanitizedString;
            List<List<string>> converter = sanitizeConverter();
            for (int i = 0; i < converter[0].Count; i++)
                sanitizedString = sanitizedString.Replace(converter[i][0], String.Empty);

            return sanitizedString;
        }

        protected static string ReplaceDefinedCharacters(string unsanitizedString, DelegateFunction<List<List<string>>> sanitizeConverter, char replaceCharacter)
        {
            string sanitizedString = unsanitizedString;
            List<List<string>> converter = sanitizeConverter();
            for (int i = 0; i < converter[0].Count; i++)
                sanitizedString = sanitizedString.Replace(converter[i][0], replaceCharacter.ToString());

            return sanitizedString;
        }
        #endregion
    }
}
