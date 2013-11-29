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


namespace FileRenamerN.Renamers.Dto
{
    /// <summary>
    /// A parameter definition.
    /// </summary>
    public class ParamDef
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The title of the parameter, as it is presented to the user.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The data type of this parameter.
        /// </summary>
        public ParamType Type { get; set; }

        /// <summary>
        /// The default value for this parameter.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The maximum value of the parameter, only applicable if <see cref="Type"/> is <see cref="ParamType.Integer"/>.
        /// </summary>
        public string IntMaxValue { get; set; }

        /// <summary>
        /// The minimum value of the parameter, only applicable if <see cref="Type"/> is <see cref="ParamType.Integer"/>.
        /// </summary>
        public string IntMinValue { get; set; }
    }
}
