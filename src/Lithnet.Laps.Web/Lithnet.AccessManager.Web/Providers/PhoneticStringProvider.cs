﻿using System.Collections.Generic;
using System.Text;
using Lithnet.AccessManager.Server.Configuration;
using Microsoft.Extensions.Options;

namespace Lithnet.AccessManager.Web.Internal
{
    public class PhoneticStringProvider : IPhoneticPasswordTextProvider
    {
        private static readonly Dictionary<char, string> defaultDictionary = new Dictionary<char, string>() {
            { 'a', "alpha" },
            { 'b', "bravo" },
            { 'c', "charlie" },
            { 'd', "delta" },
            { 'e', "echo" },
            { 'f', "foxtrot" },
            { 'g', "golf" },
            { 'h', "hotel" },
            { 'i', "india" },
            { 'j', "juliet" },
            { 'k', "kilo" },
            { 'l', "lima" },
            { 'm', "mike" },
            { 'n', "november" },
            { 'o', "oscar" },
            { 'p', "papa" },
            { 'q', "quebec" },
            { 'r', "romeo" },
            { 's', "sierra" },
            { 't', "tango" },
            { 'u', "uniform" },
            { 'v', "victor" },
            { 'w', "whiskey" },
            { 'x', "x-ray" },
            { 'y', "yankee" },
            { 'z', "zulu" },
            { '0', "zero" } ,
            { '1', "one" } ,
            { '2', "two" } ,
            { '3', "three" } ,
            { '4', "four" } ,
            { '5', "five" } ,
            { '6', "six" } ,
            { '7', "seven" } ,
            { '8', "eight" } ,
            { '9', "nine" },
            { ' ', "space" },
            { '!', "exclamation mark" },
            { '@', "at" },
            { '#', "hash" },
            { '$', "dollar" },
            { '%', "percent" },
            { '^', "caret" },
            { '&', "ampersand" },
            { '*', "asterisk" },
            { '(', "open parenthesis" },
            { ')', "close parenthesis" },
            { '{', "open brace" },
            { '}', "close brace" },
            { '[', "open square bracket" },
            { ']', "close square bracket" },
            { '_', "underscore" },
            { '-', "minus" },
            { '+', "plus" },
            { '=', "equals" },
            { '\\', "back slash" },
            { '/', "forward slash" },
            { ';', "semi colon" },
            { ':', "colon" },
            { '\'', "apostrophe" },
            { '\"', "quote" },
            { '<', "less than" },
            { '>', "greater than" },
            { '.', "period" },
            { ',', "comma" },
            { '|', "pipe" },
            { '~', "tilde" },
            { '`', "back tick" },
            { '£', "pound" },
        };

        private readonly Dictionary<char, string> dictionary;

        private readonly PhoneticSettings settings;

        public PhoneticStringProvider(IOptionsSnapshot<UserInterfaceOptions> options)
        {
            this.settings = options.Value.PhoneticSettings ?? new PhoneticSettings()
            {
                GroupSize = 4,
                LowerPrefix = null,
                UpperPrefix = "capital"
            };

            if (this.settings.CharacterMappings != null && this.settings.CharacterMappings.Count > 0)
            {
                this.dictionary = new Dictionary<char, string>();
                foreach (var kvp in this.settings.CharacterMappings)
                {
                    char key = kvp.Key[0];
                    if (!this.dictionary.ContainsKey(key))
                    {
                        this.dictionary.Add(key, kvp.Value);
                    }
                }

                this.dictionary.Add(':', this.settings.PhoneticNameColon ?? "colon");
            }

            foreach (var (key, value) in defaultDictionary)
            {
                if (!this.dictionary.ContainsKey(key))
                {
                    this.dictionary.Add(key, value);
                }
            }
        }

        public IEnumerable<string> GetPhoneticText(string password)
        {
            int count = 0;

            StringBuilder group = new StringBuilder();

            for (int i = 0; i < password.Length; i++)
            {
                char c = password[i];

                if (count >= this.settings.GroupSize)
                {
                    count = 0;
                    yield return group.ToString();
                    group.Clear();
                }
                else
                {
                    if (i > 0 && i < password.Length)
                    {
                        group.Append(", ");
                    }
                }

                if (char.IsUpper(c) && this.settings.UpperPrefix != null)
                {
                    group.Append(this.settings.UpperPrefix);
                    group.Append(" ");
                }

                if (char.IsLower(c) && this.settings.LowerPrefix != null)
                {
                    group.Append(this.settings.LowerPrefix);
                    group.Append(" ");
                }

                group.Append(this.GetPhoneticName(c));
                count++;
            }

            if (count > 0)
            {
                yield return group.ToString();
            }
        }

        private string GetPhoneticName(char c)
        {
            if (this.dictionary.ContainsKey(c))
            {
                return this.dictionary[c];
            }

            char lower = char.ToLower(c);
            if (this.dictionary.ContainsKey(lower))
            {
                return this.dictionary[lower];
            }

            return c.ToString();
        }
    }
}
