﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Separators = Cogworks.Essentials.Constants.StringConstants.Separators;

namespace Cogworks.Essentials.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string input)
            => !string.IsNullOrWhiteSpace(input);

        public static Uri ToUri(this string urlString)
            => urlString.HasValue()
                ? new Uri(urlString)
                : null;

        public static string ToCamelCase(this string original)
        {
            if (!original.HasValue())
            {
                return string.Empty;
            }

            var text = original.ToPascalCase();

            return char.ToLower(text[0]) + text.Substring(1);
        }

        public static string ToPascalCase(this string original)
        {
            if (!original.HasValue())
            {
                return string.Empty;
            }

            var invalidCharsRgx = new Regex("[^-_a-zA-Z0-9]");
            var whiteSpace = new Regex(@"(?<=\s)");
            var startsWithLowerCaseChar = new Regex("^[a-z]");
            var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            var pascalCase = invalidCharsRgx
                .Replace(whiteSpace.Replace(original, "_"), string.Empty)
                .Split(new[] { "_", "-" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static string RemoveWhitespaceAndLower(this string input)
            => input.Replace(Separators.Space, string.Empty).ToLower();

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var position = text.IndexOf(search, StringComparison.Ordinal);

            return position >= 0
                ? text.Substring(0, position) + replace + text.Substring(position + search.Length)
                : text;
        }

        public static string ReplaceLast(this string text, string search, string replace)
        {
            var position = text.LastIndexOf(search, StringComparison.Ordinal);

            return position >= 0
                ? text.Substring(0, position) + replace + text.Substring(position + search.Length)
                : text;
        }

        public static IEnumerable<string> SplitToList(this string input, string separator = Separators.Comma)
            => input.HasValue()
                ? input.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                : Enumerable.Empty<string>();

        public static string[] SplitToArray(this string input, string separator = Separators.Comma)
            => input.SplitToList(separator).ToArray();

        public static string RemoveHtmlTags(this string input)
            => new Regex("\\<[^\\>]*\\>").Replace(input, string.Empty);

        public static string EncodeQuery(this string input)
            => input.HasValue()
                ? HttpUtility.HtmlEncode(input)
                : string.Empty;

        public static string ReplaceNewLineWithBrTag(this string input)
            => input.HasValue()
                ? input.Replace("\n", "<br />")
                : string.Empty;

        public static string EscapeHyphen(this string input)
            => input.Escape(Separators.Hyphen);

        public static string Escape(this string input, string character)
            => input.HasValue()
                ? input.Replace(
                    $"{character}",
                    $@"\{character}")
                : input;

        public static string TruncateIfLonger(this string input, int length)
            => input.Length > length
                ? input.Substring(0, length)
                : input;

        public static string UsePreIfNotHtml(this string input)
            => !input.IsInTags()
                ? $"<pre>{input.Trim()}</pre>"
                : input;

        public static bool IsInTags(this string input)
        {
            var text = input.Trim();
            return text.StartsWith("<") && text.EndsWith(">");
        }

        public static bool TryParseExactOrDefaultDate(
            this string date,
            string format,
            out DateTime resultDateTime,
            DateTime? defaultDateTime = null,
            IFormatProvider formatProvider = null,
            DateTimeStyles style = DateTimeStyles.None)
        {
            var isParsed = DateTime.TryParseExact(
                date,
                format,
                formatProvider,
                style,
                out resultDateTime);

            if (!isParsed && defaultDateTime.HasValue)
            {
                resultDateTime = defaultDateTime.Value;
            }

            return isParsed;
        }

        public static DateTime ParseExactOrDefaultDate(
            this string date,
            string format,
            DateTime? defaultDateTime = null,
            IFormatProvider formatProvider = null,
            DateTimeStyles style = DateTimeStyles.None)
        {
            _ = date.TryParseExactOrDefaultDate(format, out var resultDateTime, defaultDateTime, formatProvider, style);

            return resultDateTime;
        }

        public static string GetValueOrFallback(this string text, string fallback)
            => text.HasValue()
                ? text
                : fallback;

        public static char ToChar(this string text)
            => Convert.ToChar(text);

        public static string ReplaceLineBreaksWithHtmlNewLine(this string text)
            => HttpUtility.HtmlEncode(text)
                .Replace("\r\n", Separators.HtmlNewLine)
                .Replace("\r", Separators.HtmlNewLine)
                .Replace("\n", Separators.HtmlNewLine)
                .Replace(Environment.NewLine, Separators.HtmlNewLine);

        public static string RemoveTrailingSlash(this string url)
        {
            if (!url.HasValue())
            {
                return string.Empty;
            }

            return url.LastIndexOf('/').Equals(url.Length - 1)
                ? url.Substring(0, url.Length - 1)
                : url;
        }

        public static bool IsValidEmail(this string email)
        {
            if (!email.HasValue())
            {
                return false;
            }

            try
            {
                return new System.Net.Mail.MailAddress(email).Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string HtmlDecode(this string input)
            => input.HasValue()
                ? System.Web.HttpUtility.HtmlDecode(input)
                : string.Empty;

        public static string RemoveMultipleSpaces(this string input)
            => Regex.Replace(input, "[ ]{2,}", Separators.Space);

        public static string ToBase64(this string plainText)
            => Base64UrlEncoder.Encode(plainText);

        public static string FromBase64(this string base64Encoded)
            => Base64UrlEncoder.Decode(base64Encoded);

        public static string RemoveNewLines(this string input)
            => input.HasValue()
                ? input
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace(Environment.NewLine, string.Empty)
                : string.Empty;

        public static string GetIpAddressWithoutPort(this string ipAddress)
        {
            var portIndex = ipAddress.IndexOf(':');

            return portIndex < 0
                ? ipAddress
                : ipAddress.Substring(0, portIndex);
        }

        public static string AddOrUpdateQueryParameter(this string url, string queryKey, string queryValue)
        {
            var splitted = url.Split(Separators.QuestionMark.ToCharArray());
            var queryString = HttpUtility.ParseQueryString(splitted.Skip(1).FirstOrDefault() ?? string.Empty);

            queryString[queryKey] = queryValue;

            return $"{splitted.FirstOrDefault()}?{queryString}";
        }

        /// <summary>
        /// Strips out <P> and </P> tags if they were used as a wrapper
        /// for other HTML content.
        /// </summary>
        /// <param name="text">The HTML text.</param>
        public static string RemoveParagraphWrapperTags(this string text)
        {
            if (!text.HasValue())
            {
                return text;
            }

            var trimmedText = text.Trim();
            var paragraphIndex = trimmedText.IndexOf("<p>", StringComparison.Ordinal);

            if (paragraphIndex != 0
                || paragraphIndex != trimmedText.LastIndexOf("<p>", StringComparison.Ordinal)
                || trimmedText.Substring(trimmedText.Length - 4, 4) != "</p>")
            {
                // Paragraph not used as a wrapper element
                return text;
            }

            // Remove paragraph wrapper tags
            return trimmedText.Substring(3, trimmedText.Length - 7);
        }
    }
}