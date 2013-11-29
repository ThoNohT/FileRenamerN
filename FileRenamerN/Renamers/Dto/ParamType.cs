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
    /// Lists the possible types of parameters there are.
    /// </summary>
    public enum ParamType
    {
        /// <summary>
        /// A string, uses a text field.
        /// </summary>
        String,

        /// <summary>
        /// An integer, uses a numeric up-down control.
        /// </summary>
        Integer,

        /// <summary>
        /// A boolean, uses a check box.
        /// </summary>
        Boolean,

        /// <summary>
        /// Actually not really a parameter, but wth. Just a button that can be scripted.
        /// </summary>
        Button
    }
}
