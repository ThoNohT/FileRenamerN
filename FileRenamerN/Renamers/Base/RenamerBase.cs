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


using FileRenamerN.Renamers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileRenamerN.Renamers.Base
{
    /// <summary>
    /// A base class for a file renamer, defines all operations that are possible and adds default implementations where applicable.
    /// </summary>
    public abstract class RenamerBase
    {
        #region Fields

        /// <summary>
        /// The used parameters for this class.
        /// </summary>
        protected Dictionary<string, ParamDef> parameterDefinitions;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new renamer.
        /// </summary>
        public RenamerBase()
        {
            this.parameterDefinitions = new Dictionary<string, ParamDef>();
            this.SetParameters();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of this file renamer class.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of this file renamer class.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// A list of parameter names that this renamer accepts.
        /// </summary>
        public List<ParamDef> Parameters { get { return this.parameterDefinitions.Select(pair => pair.Value).ToList(); } }

        #endregion Properties

        #region Methods

        #region Processing

        /// <summary>
        /// Processes a file, determining the new name of the file.
        /// </summary>
        /// <param name="path">The path of the file except the filename itself, including the last forward slash.</param>
        /// <param name="fileName">The filename, without the extension or the path.</param>
        /// <param name="extension">The extension, including the period.</param>
        /// <param name="parameterValues">The parameters defined by this </param>
        /// <returns>The new full name for the file, including its path and extension.</returns>
        public abstract string Process(string path, string fileName, string extension, Dictionary<string, Object> parameterValues);

        /// <summary>
        /// Performs the action defined for the button with the specified name.
        /// </summary>
        /// <param name="name">The name of the button that was pressed.</param>
        /// <param name="path">The path of the file except the filename itself, including the last forward slash.</param>
        /// <param name="fileName">The filename, without the extension or the path.</param>
        /// <param name="extension">The extension, including the period.</param>
        /// <param name="parameterValues">The parameters defined by this </param>
        public virtual void RunButton(string name, string path, string fileName, string extension, Dictionary<string, Object> parameterValues)
        {
            // No action defined means nothing needs to happen.
        }

        /// <summary>
        /// Validates the current settings against the provided path. The default validation is to approve of everything.
        /// If operations are dependent on certain combinations of settings and/or the provided path, and renaming shouldn't
        /// be started even for one file, this is the place to check and report this.
        /// </summary>
        /// <param name="path">The path to use in validation.</param>
        /// <param name="parameterValues">The settings to validate.</param>
        /// <param name="message">The message that should be shown if the validation failed.</param>
        /// <returns>A boolean that is true if validation succeeded, and false if it failed.</returns>
        public virtual bool Validate(string path, Dictionary<string, Object> parameterValues, out string message)
        {
            message = "";
            return true;
        }

        #endregion Processing

        #region Parameters

        /// <summary>
        /// Sets the required parameters for this renamer.
        /// </summary>
        protected virtual void SetParameters()
        {
        }

        /// <summary>
        /// Gets the parameter definition with the specified name.
        /// </summary>
        /// <param name="name">The name of the parameter definition to retrieve.</param>
        /// <returns>The retrieved parameter definition.</returns>
        public ParamDef GetParameter(string name)
        {
            return this.parameterDefinitions[name];
        }

        /// <summary>
        /// Defines a new parameter for this renamer and returns the defined parameter.
        /// </summary>
        /// <param name="name">The name by which the parameter should be accessed.</param>
        /// <param name="title">The title that should be shown for this parameter.</param>
        /// <param name="type">The type of parameter this is.</param>
        /// <param name="defaultValue">The default value for this parameter. This is a string,
        /// but should be parsable to the type used for this parameter.</param>
        /// <returns>The newly created parameter definition.</returns>
        protected ParamDef AddParamDef(string name, string title, ParamType type, string defaultValue = "", string intMaxValue = "100", string intMinValue = "0")
        {
            var paramDef = new ParamDef() { Name = name, Title = title, Type = type, DefaultValue = defaultValue, IntMinValue = intMinValue, IntMaxValue = intMaxValue };
            this.parameterDefinitions.Add(name, paramDef);
            return paramDef;
        }

        #endregion Parameters

        /// <summary>
        /// Gets the string representation of this renamer.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.Name;
        }

        #endregion Methods
    }
}
