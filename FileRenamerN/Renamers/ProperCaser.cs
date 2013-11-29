/********************************************************************************
 Copyright (C) 2013 Eric Bataille <e.c.p.bataille@gmail.com>

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, USA.
********************************************************************************/


using FileRenamerN.Renamers.Base;
using FileRenamerN.Renamers.Dto;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FileRenamerN.Renamers
{
    internal class ProperCaser : RenamerBase
    {
        protected override void SetParameters()
        {
            this.AddParamDef("IgnoreRomanNumerals", "Ignore Roman Numerals", ParamType.Boolean, "False");
        }

        public override string Name
        {
            get { return "ProperCaser";  }
        }

        public override string Description
        {
            get { return "Changes all words in the filename to proper casing, capitalizing the first letter of each word and lowercasing the rest."; }
        }

        public override string Process(string path, string fileName, string extension, Dictionary<string, object> parameterValues)
        {
            var ignoreRomanNumerals = (bool)parameterValues["IgnoreRomanNumerals"];
            var formatted = string.Join("", this.SetCasing(fileName, this.Lex(fileName), ignoreRomanNumerals).ToArray());
            return string.Format("{0}{1}{2}", path, formatted, extension);
        }

        #region Helpers

        private List<String> SetCasing(string fileName, List<String> elements, bool ignoreRomanNumerals)
        {
            var result = new List<String>();
            foreach (var element in elements) {
                var newElement = element; 

                var wordMatch = Regex.Match(element, @"\A\w+\z", RegexOptions.IgnoreCase);
                if (wordMatch.Success)
                {
                    newElement = string.Format("{0}{1}", element.Substring(0, 1).ToUpper(), element.Substring(1).ToLower());
                    if (!ignoreRomanNumerals)
                    {
                        var romanMatch = Regex.Match(element, @"\A[IVXLCDM]+\z", RegexOptions.IgnoreCase);
                        if (romanMatch.Success)
                        {
                            if (MessageBox.Show(
                                string.Format("The following string was found:\n'{0}'\n\nIn the string:\n'{1}'\n\n Is this a roman numeral?", element, fileName),
                                    "Roman Numeral found", MessageBoxButtons.YesNo)
                                == DialogResult.Yes)
                            {
                                newElement = element.ToUpper();
                            }
                        }
                    }
                }

                result.Add(newElement);
            }

            return result;
        }

        private List<String> Lex(string text)
        {
            var result = new List<String>();

            while (text.Length > 0)
            {
                var wordMatch = Regex.Match(text, @"\w+", RegexOptions.IgnoreCase);
                if (wordMatch.Success)
                {
                    var index = wordMatch.Index;

                    if (index > 0)
                    {
                        result.Add(text.Substring(0, index));
                    }

                    result.Add(text.Substring(index, wordMatch.Length));

                    text = text.Substring(index + wordMatch.Length);
                }
                else
                {
                    result.Add(text);
                    return result;
                }
            }

            return result;
        }

        #endregion Helpers
    }
}
