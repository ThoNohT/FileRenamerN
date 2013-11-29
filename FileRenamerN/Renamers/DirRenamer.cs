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
using FileRenamerN.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileRenamerN.Renamers
{
    internal class DirRenamer : RenamerBase
    {
        public override string Name
        {
            get { return "DirRenamer"; }
        }

        protected override void SetParameters()
        {
            this.AddParamDef("RenamePattern", "Rename pattern", ParamType.String, ":1 - :f.:e");
            this.AddParamDef("MoveUp", "Move up #dirs", ParamType.Integer, "0");
        }

        public override bool Validate(string path, Dictionary<string, object> parameterValues, out string message)
        {
            // Get input parameters
            var pattern = (string)parameterValues["RenamePattern"];
            var moveUp = Convert.ToInt32(parameterValues["MoveUp"]);
            
            if (!pattern.Contains(":f")) {
                message =  string.Format("{0}{1}",
                    "Pattern doesn't contain the filename, this means that all files would be renamed to the same filename, ",
                    "which would cause them each to overwrite each other.");
                return false;
            }

            // Check if the pattern contains any forbidden characters
            if (Regex.IsMatch(pattern, "[<>\"/\\|?*]"))
            {
                message = string.Format("{0}\n{1}",
                    "Pattern contains one of the illegal characters:",
                    "<, >, \", /, \\, |, ?, *");
                return false;
            }

            // Check how deeply this folder is nested.
            var folderDepth = path.Split('\\').Where(dir => !string.IsNullOrWhiteSpace(dir)).Count();
            if (folderDepth <= moveUp)
            {
                message = "Cannot move files up above the root folder.";
                return false;
            }
            
            // Check for invalid
            var invalidParameters = Regex.Matches(pattern, @"\:([^\dfe]|$)").OfType<Match>();
            if (invalidParameters.Count() > 0)
            {
                message = string.Format("{0}\n{1}",
                    "The following invalid parameter definitions were found:",
                    string.Join(", ", invalidParameters.Select(m => m.Value)));
                return false;
            }

            // Check the highest number used.
            var maxParam = (from match in Regex.Matches(pattern, @"\:([\dfe])").OfType<Match>()
                          let strVal = match.Groups[1].Value
                          where strVal.IsInt()
                          let intVal = int.Parse(strVal)
                          orderby intVal descending
                          select intVal).FirstOrDefault();
            if (folderDepth <= maxParam)
            {
                message = "Not enough folders in the path to apply the specified rename pattern.";
                return false;
            }

            // Everything is good to go.
            message = "";
            return true;
        }

        public override string Description
        {
            get {
                return string.Format("{0}\n{1}\n{2}\n{3}",
                    "Renames the file based on the directory structure it is in.",
                    "The pattern entered defines directory names and the file name which are then formed into a new filename.",
                    "Available parameters: :e = extension (without .), :f = filename, :0 = top level folder, :1 = second level folder, and down to the root.",
                    "Note that the extension is not automatically appended, and will need to be included in the pattern.");
            }
        }

        public override string Process(string path, string fileName, string extension, Dictionary<string, object> parameterValues)
        {
            // Get input parameters
            var pattern = (string)parameterValues["RenamePattern"];
            var moveUp = Convert.ToInt32(parameterValues["MoveUp"]);
            var pathParts = path.Split('\\').Where(dir => !string.IsNullOrWhiteSpace(dir)).Reverse().ToList();

            // Cut off moveUp folders from the path to move the files to.
            var newPath = string.Format(@"{0}\", string.Join(@"\", pathParts.Skip(moveUp).Reverse()));

            // Fill in the additional parameter values.
            pathParts.AddRange(new [] { fileName, extension.Substring(1) });
            var specialParams = new string[] { "f", "e" };

            // Try replacing all matches one by one.
            var newFilename = pattern;
            var thresh = pathParts.Count - specialParams.Count();
            for (int i = 0; i < pathParts.Count; i++)
            {
                newFilename = newFilename.Replace(
                    string.Format(":{0}", i < thresh ? i.ToString() : specialParams[i - thresh]),
                    pathParts[i]);
            }

            return string.Format("{0}{1}", newPath, newFilename);
        }
    }
}
