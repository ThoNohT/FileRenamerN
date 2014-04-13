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


namespace FileRenamerN.Common.Extensions
{
    /// <summary>
    /// A class for simple extension methods.
    /// </summary>
    public static class SimpleExtentions
    {
        /// <summary>
        /// Checks whether the specified string represents an integer value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>A Boolean value indicating whether the string represents an integer value.</returns>
        public static bool IsInt(this string value)
        {
            int dummy;
            return int.TryParse(value, out dummy);
        }

        /// <summary>
        /// Converts the specified Boolean to an integer representing its ordinal value.
        /// </summary>
        /// <param name="value">The Boolean to get the ordinal value of.</param>
        /// <returns>The ordinal value of the specified Boolean.</returns>
        public static int Ord(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
