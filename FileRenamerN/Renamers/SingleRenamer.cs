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
using System.Collections.Generic;

namespace FileRenamerN.Renamers
{
    internal class SingleRenamer : RenamerBase
    {
        public override string Name
        {
            get { return "SingleRenamer"; }
        }

        protected override void SetParameters()
        {
            this.AddParamDef("Find0", "Find", ParamType.String);
            this.AddParamDef("Replace0", "Replace with", ParamType.String);
            this.AddParamDef("Find1", "Find", ParamType.String);
            this.AddParamDef("Replace1", "Replace with", ParamType.String);
            this.AddParamDef("Find2", "Find", ParamType.String);
            this.AddParamDef("Replace2", "Replace with", ParamType.String);
            this.AddParamDef("Find3", "Find", ParamType.String);
            this.AddParamDef("Replace3", "Replace with", ParamType.String);
        }

        public override string Description
        {
            get
            {
                return string.Format("Renames the first occurrence of the found string with the specified replacements.");
            }
        }

        public override string Process(string path, string fileName, string extension, Dictionary<string, object> parameterValues)
        {
            var newFilename = fileName;
            for (int i = 0; i < 4; i++)
            {
                var find = (string)parameterValues[string.Format("Find{0}", i)];
                if (string.IsNullOrEmpty(find))
                    continue;

                var replace = (string)parameterValues[string.Format("Replace{0}", i)];

                var pos = newFilename.IndexOf(find);
                if (pos > -1)
                {
                    var length = find.Length;

                    var before = newFilename.Substring(0, pos);
                    var after = newFilename.Substring(pos + length);
                    newFilename = before + replace + after;
                }
            }

            return string.Format("{0}{1}{2}", path, newFilename, extension);
        }
    }
}
