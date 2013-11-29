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


using System;
using System.Collections.Generic;
using System.IO;

namespace FileRenamerN
{
    /// <summary>
    /// A class for serializing and deserializing the settings used in FileRenamerN to a file.
    /// </summary>
    class Settings
    {
        #region Methods

        /// <summary>
        /// Serializes the settings and writes them to the specified file.
        /// </summary>
        /// <param name="dictionary">The dictionary containing all settings. Settings are grouped per renamer, the general settings have key "General".</param>
        /// <param name="fileName">The filename to write the settings to.</param>
        public static void WriteDictionary(Dictionary<string, Dictionary<string, string>> dictionary, string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var outStream = new FileStream(fileName, FileMode.CreateNew))
                Settings.Serialize(dictionary, outStream);
        }

        /// <summary>
        /// Read the settings from the specified file.
        /// </summary>
        /// <param name="fileName">The filename to read the settings from.</param>
        /// <returns>The settings that were read from the file.</returns>
        public static Dictionary<string, Dictionary<string, string>> ReadDictionary(string fileName)
        {
            // Check if it exists, if not, return an empty dictionary.
            if (!File.Exists(fileName))
                return new Dictionary<string, Dictionary<string, string>>();

            using (var inStream = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    return Settings.Deserialize(inStream);
                }
                catch (Exception)
                {
                    return new Dictionary<string, Dictionary<string, string>>();
                }
            }
        }

        #endregion Methods

        #region Helpers

        /// <summary>
        /// Serializes the specified dictionary into the specified stream.
        /// </summary>
        /// <param name="dictionary">The dictionary to be serialized.</param>
        /// <param name="stream">The stream to write to.</param>
        private static void Serialize(Dictionary<string, Dictionary<string, string>> dictionary, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(dictionary.Count);
            // Loop outer
            foreach (var subDictionary in dictionary)
            {
                writer.Write(subDictionary.Key);
                writer.Write(subDictionary.Value.Count);

                // Loop inner
                foreach (var kvp in subDictionary.Value) {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
            writer.Flush();
        }

        /// <summary>
        /// Deserializes a stream into a dictionary.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The deserialized dictionary.</returns>
        private static Dictionary<string, Dictionary<string, string>> Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            int count = reader.ReadInt32();
            var dictionary = new Dictionary<string, Dictionary<string, string>>(count);
            // Loop outer
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var subCount = reader.ReadInt32();
                var subDictionary = new Dictionary<string, string>(subCount);

                // Loop inner
                for (int j = 0; j < subCount; j++)
                {
                    var key = reader.ReadString();
                    var value = reader.ReadString();
                    subDictionary.Add(key, value);
                }
                dictionary.Add(name, subDictionary);

            }
            return dictionary;
        }

        #endregion Helpers
    }
}
