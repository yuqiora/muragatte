// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Muragatte.Visual.IO
{
    public class SnapshotSaver
    {
        #region Constants

        private const string DEFAULT_FILENAME = "Snapshot.png";

        #endregion

        #region Fields

        private SaveFileDialog _dlg = new SaveFileDialog();

        #endregion

        #region Constructors

        public SnapshotSaver()
        {
            _dlg.Title = "Save Snapshot";
            _dlg.DefaultExt = ".png";
            _dlg.Filter = "PNG Images (.png)|*.png";
        }

        #endregion

        #region Methods

        public void Save(BitmapSource image)
        {
            _dlg.FileName = DEFAULT_FILENAME;
            bool? result = _dlg.ShowDialog();
            if (result == true)
            {
                Save(_dlg.FileName, image);
            }
        }

        public void Save(string filename, BitmapSource image)
        {
            if (Directory.Exists(Path.GetDirectoryName(filename)))
            {
                using (FileStream png = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(png);
                }
            }
        }

        public void Save(Snapshot snapshot)
        {
            //redraw snapshot
            Save(snapshot.Image);
        }

        public void Save(string filename, Snapshot snapshot)
        {
            //redraw snapshot
            Save(filename, snapshot.Image);
        }

        #endregion
    }
}
