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

namespace FileRenamerN.Renamers
{
    internal class Stripper : RenamerBase
    {
        public override string Name
        {
            get { return "Stripper"; }
        }

        protected override void SetParameters()
        {
            this.AddParamDef("StripFront", "#characters from front", ParamType.Integer, "0");
            this.AddParamDef("StripEnd", "#characters from end", ParamType.Integer, "0");
        }

        public override string Description
        {
            get { return "Removes the set amount of characters from the front or end of the filename."; }
        }

        public override string Process(string path, string fileName, string extension, Dictionary<string, object> parameterValues)
        {
            var stripFront = (int)(decimal)parameterValues["StripFront"];
            var stripEnd = (int)(decimal)parameterValues["StripEnd"];

            fileName = fileName.Substring(Math.Min(stripFront, fileName.Length));
            fileName = fileName.Substring(0, Math.Max(0, fileName.Length - stripEnd));
            return string.Format("{0}{1}{2}", path, fileName, extension);
        }
    }
}
