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
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DirectoryLabel
{

    /// <summary>
    /// A container for a DirectoryLabel in a StatusStrip.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public partial class DirectoryLabelStrip: ToolStripControlHost
    {
        #region Fields

        /// <summary>
        /// The label that is contained.
        /// </summary>
        private DirectoryLabel label;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the currently selected path.
        /// </summary>
        public string CurrentPath { get { return label.CurrentPath; } }

        /// <summary>
        /// Gets or sets the maximum width of the label. Note that this length will be ignored if it
        /// causes less than three folder layers to be displayed.
        /// </summary>
        public ushort MaxWidth { get { return label.MaxWidth; } set { label.MaxWidth = value; } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryLabelStrip"/> class.
        /// </summary>
        /// <param name="label">The path to initialize the directory label at.</param>
        public DirectoryLabelStrip(string path) : this(new DirectoryLabel(path)) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryLabelStrip"/> class.
        /// </summary>
        /// <param name="label">The path to initialize the directory label at.</param>
        /// <param name="maxWidth">The maximum width of the control.</param>
        public DirectoryLabelStrip(string path, ushort maxWidth) : this(new DirectoryLabel(path, maxWidth)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryLabelStrip"/> class.
        /// </summary>
        /// <param name="label">The <see cref="DirectoryLabel"/> to initialize it with.</param>
        private DirectoryLabelStrip(DirectoryLabel label)
            : base(label)
        {
            this.label = label;
            this.Size = this.label.Size;
            this.label.DirectoryChanged += OnDirectoryChanged;
        }

        #endregion Constructors

        #region Interaction

        /// <summary>
        /// Sets the directory to the specified path and re-layouts the control.
        /// </summary>
        /// <param name="path">The path to change the directory to.</param>
        public void ChangeDirectory(string path)
        {
            this.label.ChangeDirectory(path);
            this.Size = this.label.Size;
        }

        /// <summary>
        /// Propagates the directory changed event to the outside world.
        /// </summary>
        private void OnDirectoryChanged(object sender, EventArgs e)
        {
            var handler = DirectoryChanged;
            if (handler != null) handler(this, e);
        }
        
        #endregion Interaction

        #region Events

        /// <summary>
        /// An event that is fired whenever the directory is changed, including initially.
        /// </summary>
        public event EventHandler DirectoryChanged;

        #endregion Events

    }   
}