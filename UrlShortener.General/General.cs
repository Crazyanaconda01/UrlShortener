using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UrlShortener.Data;

namespace UrlShortener.General
{
    public static class Functions
    {
        /// <summary>
        /// Generates random hash, use length to length of string to return.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomHash(int length)
        {
            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return BitConverter.ToString(bytes).Replace("-", "").ToLower().Substring(0, length);
        }

        /// <summary>
        /// Returns true if url is valid. Both http and https protocol works.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckValidUrl(string input)
        {
            //Check 1, check valid length. Shortest possible url: "u.co"
            if(input.Length <= 4)
            {
                return false;
            }

            bool isUri = false;
            string pattern = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Uri urlToTest = null;

            //Check 2
            //No scheme
            if (!input.StartsWith("http://") && !input.StartsWith("https://"))
            {
                isUri = Uri.IsWellFormedUriString(input, UriKind.Absolute);

                if (Uri.TryCreate(input, UriKind.Relative, out urlToTest))
                {
                    input = "http://" + input;
                    if (Uri.TryCreate(input, UriKind.Absolute, out urlToTest) && reg.IsMatch(input))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            //With scheme
            else if (Uri.TryCreate(input, UriKind.Absolute, out urlToTest))
            {
                if ((urlToTest.Scheme == Uri.UriSchemeHttp || urlToTest.Scheme == Uri.UriSchemeHttps) && reg.IsMatch(input))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a string list with all modelstate errors.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<string> GetModelStateErrorString(ModelStateDictionary m)
        {
            var errors = new List<string>();
            foreach (var modelErrors in m)
            {
                errors.Add(modelErrors.Key);
            }
            return errors;
        }

    }
}
