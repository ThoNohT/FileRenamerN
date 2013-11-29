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

namespace FileRenamerN.Renamers
{
    internal class Tagger : RenamerBase
    {
        public override string Name
        {
            get { return "Tagger"; }
        }

        protected override void SetParameters()
        {
            this.AddParamDef("Pattern", "Pattern", ParamType.String, @"\A(\d+?)\. (.+)\z");
            this.AddParamDef("Artist", "Artist", ParamType.Integer, "0", "6");
            this.AddParamDef("Year", "Year", ParamType.Integer, "0", "6");
            this.AddParamDef("Album", "Album", ParamType.Integer, "0", "6");
            this.AddParamDef("Track", "Track", ParamType.Integer, "1", "6");
            this.AddParamDef("Title", "Title", ParamType.Integer, "2", "6");
            this.AddParamDef("Comments", "Comments", ParamType.Integer, "0", "6");
            this.AddParamDef("ClearTags", "Clear Tags", ParamType.Button);
            this.AddParamDef("ViewTags", "View Tags", ParamType.Button);
        }

        public override void RunButton(string name, string path, string fileName, string extension, Dictionary<string, Object> parameterValues)
        {
            if (name == "ClearTags")
            {
                var fullName = string.Format("{0}{1}{2}", path, fileName, extension);

                using (var file = TagLib.File.Create(fullName))
                {
                    file.Tag.Clear();
                    file.Save();
                }
            }
            if (name == "ViewTags")
            {
                // TODO: Implement a tag view.
                Console.WriteLine("You want to view all tags, okay. I will implement that later.", "TODO");
            }
        }

        public override string Description
        {
            get
            {
                return string.Format("{0}\n{1}\n{2}",
                    "Adds Id3 tags to files based on their filename.",
                    "Fill in a regex with up to 5 capture groups, that the files will be matched against.",
                    "In fields below, every assignable tag can be given a group number, where 0 means it is not used.",
                    "The tags will then be applied as defined by the captured groups.");
            }
        }

        public override bool Validate(string path, Dictionary<string, object> parameterValues, out string message)
        {
            // Try the regex first, to see if it is even valid.
            var regex = (string)parameterValues["Pattern"];

            try
            {
                Regex.Match("", regex);
            }
            catch (ArgumentException ex)
            {
                message = string.Format("Invalid regular expression: Error {0}", ex.Message);
                return false;
            }

            // Find the highest used group.
            int maxGroup = 0;
            maxGroup = Math.Max(maxGroup, Convert.ToInt32(parameterValues["Artist"]));
            maxGroup = Math.Max(maxGroup, Convert.ToInt32(parameterValues["Year"]));
            maxGroup = Math.Max(maxGroup, Convert.ToInt32(parameterValues["Album"]));
            maxGroup = Math.Max(maxGroup, Convert.ToInt32(parameterValues["Track"]));
            maxGroup = Math.Max(maxGroup, Convert.ToInt32(parameterValues["Title"]));
            maxGroup = Math.Max(maxGroup, Convert.ToInt32(parameterValues["Comments"]));

            if (maxGroup == 0)
            {
                message = "No tags defined. If you want to clear the tags, please use the \"Clear Tags\" button instead.";
                return false;
            }

            // This magic 'ere counts the number of capturing groups in the specified pattern.
            // Change the regex to accept an empty string, match an empty string and all capturing croups are empty.
            // Ignore the first because that's the entire match.
            var numberOfGroups = Regex.Match("", regex + "|").Groups.Count - 1;
            if (maxGroup > numberOfGroups)
            {
                message = "There are not enough capturing groups in the specified regular expression to fill in all defined parameters.";
                return false;
            }
            
            message = "";
            return true;
        }

        public override string Process(string path, string fileName, string extension, Dictionary<string, object> parameterValues)
        {
            var regex = (string)parameterValues["Pattern"];
            var artistGroup = Convert.ToInt32(parameterValues["Artist"]);
            var yearGroup = Convert.ToInt32(parameterValues["Year"]);
            var albumGroup = Convert.ToInt32(parameterValues["Album"]);
            var trackGroup = Convert.ToInt32(parameterValues["Track"]);
            var titleGroup = Convert.ToInt32(parameterValues["Title"]);
            var commentsGroup = Convert.ToInt32(parameterValues["Comments"]);

            // Fill in the things and stuff
            var match = Regex.Match(fileName, regex);
            var artist = match.Groups[artistGroup].Value;
            var year = match.Groups[yearGroup].Value;
            var album = match.Groups[albumGroup].Value;
            var track = match.Groups[trackGroup].Value;
            var title = match.Groups[titleGroup].Value;
            var comments = match.Groups[commentsGroup].Value;

            // Open the file for tag editing and stuff like that
            var fullName = string.Format("{0}{1}{2}", path, fileName, extension);

            using (var file = TagLib.File.Create(fullName))
            {
                file.Tag.Clear();

                // Do the assigning
                if (artistGroup > 0)
                    file.Tag.Performers = new[] { artist };
                if (yearGroup > 0)
                    file.Tag.Year = Convert.ToUInt32(year);
                if (albumGroup > 0)
                    file.Tag.Album = album;
                if (trackGroup > 0)
                    file.Tag.Track = Convert.ToUInt32(track);
                if (titleGroup > 0)
                    file.Tag.Title = title;
                if (commentsGroup > 0)
                    file.Tag.Comment = comments;

                file.Save();
            }
            return null;
        }
    }
}
