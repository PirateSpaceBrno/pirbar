using HtmlAgilityPack;
using MMLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Controllers
{
    internal static class TextHelper
    {
        internal static string RemoveSpecialCharacters(string str)
        {
            str = str.RemoveDiacritics();
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        internal static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }


        /// <summary>
        /// Returns HTML Api page with header, css, etc.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static string GetApiPage(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}index.html");
            HtmlNode newNode = HtmlNode.CreateNode(content);
            doc.DocumentNode.SelectSingleNode("//body").AppendChild(newNode);
            return doc.DocumentNode.InnerHtml.Replace("PirBanka", new PirBankaConfig().InstanceName);
        }

        internal static List<int> GetUriIds(string request, string regex)
        {
            var match = Regex.Match(request, regex, RegexOptions.IgnoreCase);
            var result = new List<int>();

            foreach (Group x in match.Groups)
            {
                try
                {
                    result.Add(Convert.ToInt32(x.Value));
                }
                catch
                {
                    result.Add(0);
                }
            }

            return result;
        }
    }
}
