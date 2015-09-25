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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DirectoryLabel;
using FileRenamerN.Common.Extensions;
using FileRenamerN.Renamers.Base;
using FileRenamerN.Renamers.Dto;
using Microsoft.VisualBasic;

namespace FileRenamerN
{
    /// <summary>
    /// The main form.
    /// </summary>
    public partial class FileRenamerN : Form
    {
        #region Fields

        /// <summary>
        /// A dictionary mapping the names of the controls that are used by the currently loaded plugin, to their
        /// controls.
        /// </summary>
        private Dictionary<string, Control> usedControls = new Dictionary<string, Control>();
        
        /// <summary>
        /// A dictionary storing all settings, grouped by category.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> settings
            = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// The currently loaded plugin.
        /// </summary>
        private RenamerBase currentRenamer;

        /// <summary>
        /// The list of all files in the currently selected directory.
        /// </summary>
        private List<string> allFiles;
        
        /// <summary>
        /// The list of available renamer plugins.
        /// </summary>
        private List<RenamerBase> renamers = new List<RenamerBase>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRenamerN"/> class.
        /// </summary>
        public FileRenamerN()
        {
            InitializeComponent();
            LoadGeneralSettings();
            LoadFiles();
            LoadRenamers();

            // Add an event listener to the DirectoryChanged event of the directory label.
            this.dirLabel.DirectoryChanged += OnDirectoryChanged;
        }

        #endregion Constructors

        #region Settings

        /// <summary>
        /// Uses reflection to determine which different renamer classes exist.
        /// Read the settings and append them if renamer classes exist that do not
        /// yet have settings or that have parameters that do not have settings.
        /// </summary>
        private void LoadRenamers()
        {
            lstRenamers.Items.Clear();

            var renamers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "FileRenamerN.Renamers")
                .OrderBy(t => t.Name)
                .Where(t => !t.IsAbstract);

            foreach (var renamerType in renamers)
            {
                var renamer = (RenamerBase)renamerType.GetConstructor(new Type[0]).Invoke(new object[0]);
                lstRenamers.Items.Add(renamer);

                if (!this.settings.ContainsKey(renamer.Name))
                    this.settings.Add(renamer.Name, new Dictionary<string, string>());

                foreach (var param in renamer.Parameters)
                {
                    if (!this.settings[renamer.Name].ContainsKey(param.Name))
                        this.settings[renamer.Name].Add(param.Name, param.DefaultValue);
                }
            }
        }

        /// <summary>
        /// Loads the general settings for FileRenamerN from the settings dictionary.
        /// </summary>
        private void LoadGeneralSettings()
        {
            this.settings = Settings.ReadDictionary("settings.dat");

            if (!this.settings.ContainsKey("General"))
                this.settings.Add("General", new Dictionary<string, string>());
            var generalSettings = this.settings["General"];

            // Load a default value about here...
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!generalSettings.ContainsKey("Directory"))
                generalSettings.Add("Directory", currentDir);
            else
                currentDir = generalSettings["Directory"];
            try
            {
                this.dirLabel.ChangeDirectory(currentDir);
            } catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Failed to load initial directory: '{0}', reverting to application path.",
                    ex.Message));
            }

            txtFileFilter.Text =  ".*";
            if (!generalSettings.ContainsKey("FileFilter"))
                generalSettings.Add("FileFilter", txtFileFilter.Text);
            else
                txtFileFilter.Text = generalSettings["FileFilter"];
        }

        /// <summary>
        /// Stores the general settings into the settings dictionary.
        /// </summary>
        private void SaveGeneralSettings()
        {
            var generalSettings = this.settings["General"];

            generalSettings["Directory"] = this.dirLabel.CurrentPath;
            generalSettings["FileFilter"] = txtFileFilter.Text;
        }

        /// <summary>
        /// Saves the control values for the current renamer into the settings dictionary.
        /// </summary>
        private void SaveCurrentControlValues()
        {
            foreach (var item in this.usedControls)
            {
                string value = "";
                switch (this.currentRenamer.GetParameter(item.Key).Type)
                {
                    case ParamType.Boolean:
                        value = ((CheckBox)item.Value).Checked.ToString();
                        break;
                    case ParamType.Integer:
                        value = ((NumericUpDown)item.Value).Value.ToString();
                        break;
                    case ParamType.String:
                        value = ((TextBox)item.Value).Text;
                        break;
                }
                this.settings[this.currentRenamer.Name][item.Key] = value;
            }
        }

        /// <summary>
        /// Save settings upon closing of the form.
        /// </summary>
        private void FileRenamerN_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save settings
            this.SaveCurrentControlValues();
            this.SaveGeneralSettings();
            Settings.WriteDictionary(this.settings, "settings.dat");
        }

        #endregion Settings

        #region Files

        /// <summary>
        /// Browses for a new directory to perform the renaming operations in.
        /// </summary>
        private void lblBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (!string.IsNullOrWhiteSpace(this.dirLabel.CurrentPath))
            {
                dialog.SelectedPath = this.dirLabel.CurrentPath;
            }

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK) this.dirLabel.ChangeDirectory(dialog.SelectedPath);
        }

        /// <summary>
        /// Handles the event that the directory is changed from the directory label.
        /// </summary>
        private void OnDirectoryChanged(object sender, EventArgs e)
        {
            this.LoadFiles();
        }

        /// <summary>
        /// Loads the list of files from the currently specified directory.
        /// </summary>
        private void LoadFiles()
        {
            var files = Directory.GetFiles(this.dirLabel.CurrentPath);
            this.allFiles = files.Select(f => new FileInfo(f).Name).ToList();
            this.FilterFiles();
        }

        /// <summary>
        /// Filters the file list by the specified filter, which is interpreted as a regular expression.
        /// </summary>
        private void FilterFiles()
        {
            // Don't do this if there's nothing to filter.
            if (this.allFiles == null)
                return;

            lstFiles.Items.Clear();
            var regex = string.Format(@"\A{0}\z", txtFileFilter.Text);
            var invalidRegex = false;
            foreach (var file in this.allFiles)
            {
                try
                {
                    if (invalidRegex || Regex.IsMatch(file, regex))
                        lstFiles.Items.Add(file);
                }
                catch (ArgumentException)
                {
                    // If this fails, the regex is invalid, set a value to indicate
                    // that we don't need to try parsing it again.
                    invalidRegex = true;
                    lstFiles.Items.Add(file);
                }
            }
        }

        /// <summary>
        /// Allows the user to manually rename the selected file.
        /// </summary>
        private void lstFiles_DoubleClick(Object sender, EventArgs e)
        {
            var listbox = (ListBox)sender;

            if (listbox.SelectedItems.Count != 1) return;

            var path = this.dirLabel.CurrentPath;
            var filename = listbox.SelectedItem.ToString();

            if (!ValidateFile(path + filename)) return;

            var newName = Interaction.InputBox("Please enter the desired new name for this file", "Rename", filename);

            if (string.IsNullOrEmpty(newName)) return;

            FileInfo fi = new FileInfo(path + filename);

            fi.MoveTo(path + newName);
            LoadFiles();
        }

        /// <summary>
        /// Validates whether a file is actually a file and renaming operations can be done on it.
        /// </summary>
        /// <param name="filename">The filename to validate.</param>
        /// <returns>A boolean value indicating whether the file exists.</returns>
        private bool ValidateFile(string filename)
        {
            if (new FileInfo(filename).Exists) return true;
        
            MessageBox.Show(string.Format("The file '{0}' does not exist.", filename));
            return false;
        }
        
        #endregion Files

        #region Renamers

        /// <summary>
        /// Loads up a new renamer.
        /// </summary>
        private void lstRenamers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRenamers.SelectedItems.Count != 1) return;

            // First save the values filled in for these controls.
            this.SaveCurrentControlValues();

            // Then switch renamers.
            var renamer = this.currentRenamer = (RenamerBase)lstRenamers.SelectedItem;
            btnApply.Enabled = renamer != null;

            // Remove all current controls.
            argPan.SuspendLayout();
            argPan.SuspendDrawing();
            usedControls.Clear();

            argPan.Controls.Clear();

            lblToolDescription.Text = renamer.Description;

            // Set the new controls.
            argPan.RowCount = renamer.Parameters.Count();
            foreach (var param in renamer.Parameters)
            {
                // Set the label of the control
                var label = new Label();
                label.Text = string.Format("{0}:", param.Title);
                label.TextAlign = ContentAlignment.MiddleRight;
                label.Width = 130;

                // Set the actual inputs.
                switch (param.Type)
                {
                    case ParamType.Boolean:
                        var checkBox = new CheckBox();
                        var doCheck = false;
                        if (!bool.TryParse(this.settings[currentRenamer.Name][param.Name], out doCheck))
                            doCheck = bool.Parse(param.DefaultValue);
                        checkBox.Checked = doCheck;

                        usedControls.Add(param.Name, checkBox);
                        argPan.Controls.Add(label);
                        argPan.Controls.Add(checkBox);
                        break;
                    case ParamType.Integer:
                        var nUpDown = new NumericUpDown();
                        int intVal = 0;
                        if (!int.TryParse(this.settings[currentRenamer.Name][param.Name], out intVal))
                            intVal = int.Parse(param.DefaultValue);
                        nUpDown.Value = intVal;
                        intVal = int.Parse(param.IntMinValue);
                        nUpDown.Minimum = intVal;
                        intVal = int.Parse(param.IntMaxValue);
                        nUpDown.Maximum = intVal;

                        usedControls.Add(param.Name, nUpDown);
                        argPan.Controls.Add(label);
                        argPan.Controls.Add(nUpDown);
                        break;
                    case ParamType.String:
                        var textBox = new TextBox();
                        textBox.Width = 320;
                        textBox.Text = this.settings[currentRenamer.Name][param.Name];

                        usedControls.Add(param.Name, textBox);
                        argPan.Controls.Add(label);
                        argPan.Controls.Add(textBox);
                        break;
                    case ParamType.Button:
                        var button = new Button();
                        button.Text = param.Title;
                        button.Click += (a, b) => ButtonClicked(param.Name);
                        label.Text = "Action:";

                        usedControls.Add(param.Name, button);
                        argPan.Controls.Add(label);
                        argPan.Controls.Add(button);
                        break;
                }
            }
            argPan.ResumeLayout();
            argPan.ResumeDrawing();
        }

        /// <summary>
        /// Calls the renamer's <see cref="RenamerBase.RunButton"/> command when a button is clicked.
        /// </summary>
        /// <param name="name">The name of the button that was clicked.</param>
        private void ButtonClicked(string name)
        {
            progress.Maximum = lstFiles.Items.Count + 1;
            progress.Value = 0;

            var parameters = this.currentRenamer.Parameters
                             .ToDictionary(paramDef => paramDef.Name, paramDef => GetVal(paramDef));
            var path = this.dirLabel.CurrentPath;
            progress.Value++;

            // Process all files
            foreach (string item in lstFiles.Items)
            {
                string fileName;
                string extension;
                if (item.Contains("."))
                {
                    fileName = item.Substring(0, item.LastIndexOf("."));
                    extension = item.Substring(item.LastIndexOf("."));
                }
                else
                {
                    fileName = item;
                    extension = "";
                }
                this.currentRenamer.RunButton(name, path, fileName, extension, parameters);
            }

            progress.Value++;
        }

        /// <summary>
        /// Applies the main function fo the currently selected renamer.
        /// </summary>
        private void btnApply_Click(object sender, EventArgs e)
        {
            progress.Maximum = lstFiles.Items.Count + 1;
            progress.Value = 0;

            var parameters = this.currentRenamer.Parameters
                             .ToDictionary(paramDef => paramDef.Name, paramDef => GetVal(paramDef));
            var path = this.dirLabel.CurrentPath;
            progress.Value++;

            // Validate the settings
            string message;
            if (!this.currentRenamer.Validate(path, parameters, out message))
            {
                MessageBox.Show(string.Format("Renamer {0} parameter validation failed with message:\r\n\r\n{1}", this.currentRenamer.Name, message), "Unable to apply tool");
                return;
            }

            // Process all files
            foreach (string item in lstFiles.Items)
            {
                string fileName;
                string extension;
                if (item.Contains("."))
                {
                    fileName = item.Substring(0, item.LastIndexOf("."));
                    extension = item.Substring(item.LastIndexOf("."));
                }
                else
                {
                    fileName = item;
                    extension = "";
                }

                try
                {
                    var result = this.currentRenamer.Process(path, fileName, extension, parameters);

                    if (result != null)
                    {
                        FileInfo fi = new FileInfo(string.Format("{0}{1}{2}", path, fileName, extension));
                        fi.MoveTo(result);
                    }
                }
                catch (ProcessingException ex)
                {
                    var response = MessageBox.Show(
                        string.Format(
                            "Renamer {0} failed with message:\r\n\r\n{1},\r\n\r\n{2}.",
                            this.currentRenamer.Name,
                            ex.Message,
                            "Press Ok to continue with the next file, press cancel to stop processing"),
                        "Error while processing renamer.",
                        MessageBoxButtons.OKCancel);
                    if (response == DialogResult.OK)
                    {
                        progress.Value++;
                        continue;
                    }
                    if (response == DialogResult.Cancel)
                        break;
                }

                progress.Value++;
            }

            // Reload the file list to update the filenames
            LoadFiles();
        }

        /// <summary>
        /// Gets the value of the specified parameter
        /// </summary>
        /// <param name="paramDef">A <see cref="ParamDef"/> defining the parameter to get the value of.</param>
        /// <returns>The retrieved value.</returns>
        private object GetVal(ParamDef paramDef)
        {
            var obj = this.usedControls[paramDef.Name];
            object val = null;
            switch (paramDef.Type)
            {
                case ParamType.Boolean:
                    val = ((CheckBox)obj).Checked;
                    break;
                case ParamType.Integer:
                    val = ((NumericUpDown)obj).Value;
                    break;
                case ParamType.String:
                    val = ((TextBox)obj).Text;
                    break;
            }
            return val;
        }

        /// <summary>
        /// When the filter text has changed, re-apply the filter.
        /// </summary>
        private void txtFileFilter_TextChanged(object sender, EventArgs e)
        {
            this.FilterFiles();
        }

        /// <summary>
        /// Selecting a file shows the filename in a special text box so it can be copied from.
        /// </summary>
        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count == 1)
                txtFilename.Text = (string)lstFiles.SelectedItem;
        }

        #endregion Renamers
    }
}
