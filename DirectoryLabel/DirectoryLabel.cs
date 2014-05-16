/********************************************************************************
 Copyright (C) 2014 Eric Bataille <e.c.p.bataille@gmail.com>

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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FileRenamerN.Common.Extensions;

namespace DirectoryLabel
{
    public partial class DirectoryLabel: UserControl
    {
        #region Fields

        /// <summary>
        /// The maximum width of the label.
        /// </summary>
        private ushort maxWidth = 0;

        /// <summary>
        /// A list containing all parts of the path.
        /// </summary>
        private string[] pathParts;
        
        /// <summary>
        /// The path index of the path the user last selected.
        /// </summary>
        private int activePathIndex;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The currently selected path.
        /// </summary>
        public string CurrentPath { get; private set; }

        /// <summary>
        /// Gets or sets the maximum width of the label. Note that this length will be ignored if it
        /// causes less than three folder layers to be displayed.
        /// </summary>
        public ushort MaxWidth
        {
            get
            {
                return this.maxWidth;
            }
            set
            {
                this.maxWidth = value;
                this.Relayout();
            }
        }
        #endregion Properties

        #region Constructors

        public DirectoryLabel()
            : this(@"C:\Data\VSOffline\mp3sharp\ManagedDirectsoundDemo\obj")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryLabel"/> class.
        /// </summary>
        /// <param name="path">The path to initialize the control on.</param>
        public DirectoryLabel(string path)
        {
            InitializeComponent();

            this.ChangeDirectory(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryLabel"/> class.
        /// </summary>
        /// <param name="path">The path to initialize the control on.</param>
        /// <param name="maxWidth">The maximum width of the control.</param>
        public DirectoryLabel(string path, ushort maxWidth)
        {
            InitializeComponent();
            this.maxWidth = maxWidth;
            this.ChangeDirectory(path);
        }

        #endregion Constructors

        #region Interaction
        
        /// <summary>
        /// Sets the directory to the specified path and re-layouts the control.
        /// </summary>
        /// <param name="path">The path to change the directory to.</param>
        /// <param name="recursion">A value indicating whether this function was called recursively,
        /// if so, don't try to call it again.</param>
        public void ChangeDirectory(string path, bool recursion = false)
        {
            // Check whether the path points to a directory.
            if ((File.GetAttributes(path) & FileAttributes.Directory) == 0)
                throw new ArgumentException("Specified path is not a directory.", "path");

            var oldPath = this.CurrentPath;

            if (!path.EndsWith(@"\")) path = string.Format(@"{0}\", path);

            this.CurrentPath = path;
            this.pathParts = this.CurrentPath.Split(Path.DirectorySeparatorChar)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => string.Format(@"{0}\", x))
                .ToArray();


            try
            {
                this.Relayout();

                var handler = DirectoryChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (!recursion) ChangeDirectory(oldPath, true);
                throw;
            }
        }

        /// <summary>
        /// Replaces all labels on the label and recalculates the layout based on the present labels and available
        /// size.
        /// </summary>
        private void Relayout()
        {
            this.SuspendLayout();
            this.SuspendDrawing();

            // Start with a fresh set of controls.
            this.Controls.Clear();
            bool showSubdirectoryButton;
            try
            {
                showSubdirectoryButton = new DirectoryInfo(this.CurrentPath).EnumerateDirectories().Any();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ArgumentException("Access to the specified path was denied.");
            }

            // 19 is the subdirectories button size.
            var extraControlsOverhead = showSubdirectoryButton.Ord() * 19;

            // Calculate the total size of the label strip and the labels' individual sizes.
            var i = 0;
            var sizes = pathParts.Select(part => this.AddLabel(0, part, i++, this.maxWidth > 0)).ToArray();
            var size = sizes.Sum();

            // Now clear everything and make sure we try to stay withing MaxWidth.
            var partialSize = size;
            if (this.maxWidth > 0)
            {
                this.Controls.Clear();

                int j = 0;
                var includedControls = sizes.ToDictionary(x => j++, _ => true);
                j = 0;
                while (partialSize > this.maxWidth - extraControlsOverhead
                        && includedControls.Count(x => x.Value) > 3)
                {
                    // We removed something, so the spacer is added, which has size 13.
                    if (j == 0) extraControlsOverhead += 13;

                    // Remove another control and check the new partial size
                    var includedIndexes = includedControls.Where(x => x.Value).Select(x => x.Key).ToArray();
                    var indexesCount = includedIndexes.Count();
                    int indexesIndex = Math.Min(indexesCount - 3, (int)Math.Floor((double)(indexesCount / 2)));
                    var remIndex = includedIndexes[indexesIndex];
                    includedControls[remIndex] = false;
                    partialSize -= sizes[remIndex];
                }

                size = 0;
                var lastIndex = -1;
                foreach (var index in includedControls.Where(x => x.Value).Select(x => x.Key))
                {
                    // Add a spacer if something has been left away since the last value.
                    if (index > lastIndex + 1)
                        size += this.AddLabel(size, "..", -2, true); // Width 13
                    
                    size += this.AddLabel(size, pathParts[index], index);
                    lastIndex = index;
                }
                if (showSubdirectoryButton) size += this.AddLabel(size, "=>", -1);
            }


            this.Size = new Size(size, this.Controls[0].Height);

            this.ResumeDrawing();
            this.ResumeLayout();
        }

        /// <summary>
        /// Adds a label at the specified position, with the specified text and tag.
        /// </summary>
        /// <param name="pos">The position to add the label on.</param>
        /// <param name="text">The text to place on the label.</param>
        /// <param name="index">The index to store in the tag of the label.</param>
        /// <param name="preview">A value indicating whether this label is used as a size preview and only
        /// its basic properties should be filled.</param>
        /// <returns>The width of the created label.</returns>
        private int AddLabel(int pos, string text, int index, bool preview = false)
        {
            var label = new Label();

            // Layout of the label.
            var textSize = label.CreateGraphics().MeasureString(text, label.Font);
            label.AutoSize = true;
            this.Controls.Add(label);
            label.MinimumSize = new Size(0, 0);
            label.Text = text;
            label.BorderStyle = BorderStyle.FixedSingle;
            label.Location = new Point(pos, 0);

            // When the label is not being used just for calculating the width, also add all the other
            // relevant information.
            if (!preview)
            {
                label.Tag = index;
                label.Cursor = Cursors.Hand;
                label.Height = (int)Math.Ceiling(textSize.Height);

                // Events on the label.
                label.MouseEnter += MouseEnter;
                label.MouseLeave += MouseLeave;
                label.MouseClick += MouseClick;
            }

            return label.Width - 2;
        }

        #endregion Interaction

        #region Label event handlers

        /// <summary>
        /// Handles the event when the mouse enters a label, colors the label
        /// and changes the cursor.
        /// </summary>
        private new void MouseEnter(object sender, EventArgs e)
        {
            var label = (Label)sender;
            label.ForeColor = Color.Blue;
        }

        /// <summary>
        /// Handles the event when the mouse enters a label, resets the color and cursor.
        /// </summary>
        private new void MouseLeave(object sender, EventArgs e)
        {
            var label = (Label)sender;
            label.ForeColor = Color.Black;
        }

        /// <summary>
        /// Handles the event when the mouse hovers over a label, shows the sibling directories context menu.
        /// </summary>
        private new void MouseClick(object sender, EventArgs e)
        {
            var mouseButton = ((MouseEventArgs)e).Button;
            this.activePathIndex = (int)((Label)sender).Tag;

            // -2 means it is an uninteresting button and we don't do stuff with it.
            if (this.activePathIndex == -2) return;
            
            var clickedIndex = this.activePathIndex;
            if (clickedIndex == -1) this.activePathIndex = this.pathParts.Count();

            var clickedFolder = new DirectoryInfo(string.Join(@"\", pathParts.Take(this.activePathIndex + 1)));

            // Only show the sub folders menu if the user clicked the last thingy.
            if (clickedIndex == -1)
            {
                var directoriesToList = clickedFolder.GetDirectories().Select(x => x.Name).ToArray();
                this.ShowSubfoldersMenu(mouseButton, directoriesToList);
                return;
            }

            if (mouseButton == MouseButtons.Right)
            {
                // Show a context menu with all sibling folders.
                string[] directoriesToList;

                if (clickedIndex == 0)
                    directoriesToList = DriveInfo.GetDrives().Select(x => x.RootDirectory.Name).ToArray();
                else
                    directoriesToList = clickedFolder.Parent.GetDirectories().Select(x => x.Name).ToArray();

                var contextMenu = new ContextMenu();
                foreach (var folder in directoriesToList)
                    contextMenu.MenuItems.Add(folder, new EventHandler(this.ContextMenuClicked));

                var pos = this.PointToClient(Cursor.Position);
                contextMenu.Show(this, this.PointToClient(Cursor.Position));
            }
            else if (mouseButton == MouseButtons.Left)
            {
                // Navigate to the folder.
                try
                {
                    this.ChangeDirectory(clickedFolder.FullName);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(string.Format("Error loading directory: {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// Shows the menu showing all subfolders of the current directory.
        /// </summary>
        private void ShowSubfoldersMenu(MouseButtons mouseButton, string[] directoriesToList)
        {
            if (mouseButton != MouseButtons.Right && mouseButton != MouseButtons.Left)
                return;

            // Show a context menu with all sibling folders.
            var contextMenu = new ContextMenu();
            foreach (var folder in directoriesToList.OrderBy(x => x))
                contextMenu.MenuItems.Add(folder, new EventHandler(this.ContextMenuClicked));

            var pos = this.PointToClient(Cursor.Position);
            contextMenu.Show(this, this.PointToClient(Cursor.Position));
        }

        /// <summary>
        /// An event handler for when a generated context menu is clicked.
        /// </summary>
        private void ContextMenuClicked(object sender, EventArgs e)
        {
            try
            {
                var text = ((MenuItem)sender).Text;
                if (this.activePathIndex > 0)
                    this.ChangeDirectory(string.Format(
                        @"{0}\{1}", string.Join(@"\", this.pathParts.Take(this.activePathIndex)), text));
                else
                    this.ChangeDirectory(text);
            } catch (ArgumentException ex)
            {
                MessageBox.Show(string.Format("Error loading directory: {0}", ex.Message));
            }
        }

        #endregion Label event handlers

        #region Events

        /// <summary>
        /// An event that is fired whenever the directory is changed, including initially.
        /// </summary>
        public event EventHandler DirectoryChanged;

        #endregion Events
    }
}
