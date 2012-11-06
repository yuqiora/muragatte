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
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Muragatte.IO;

namespace Muragatte.Visual.IO
{
    public abstract class XmlBaseArchiver<T, W>
        where W : System.Windows.Window
    {
        #region Constants

        protected const string DEFAULT_EXT = ".xml";
        protected const string FILTER = "XML Files (.xml)|*.xml";

        #endregion

        #region Fields

        protected XmlLoadSave<T> _xml = new XmlLoadSave<T>();
        protected OpenFileDialog _loadDlg = new OpenFileDialog();
        protected SaveFileDialog _saveDlg = new SaveFileDialog();
        protected W _owner = null;

        #endregion

        #region Constructors

        public XmlBaseArchiver(string label, W owner)
        {
            _loadDlg.Title = "Load " + label;
            _loadDlg.FileName = label;
            _loadDlg.DefaultExt = DEFAULT_EXT;
            _loadDlg.Filter = FILTER;

            _saveDlg.Title = "Save " + label;
            _saveDlg.FileName = label;
            _saveDlg.DefaultExt = DEFAULT_EXT;
            _saveDlg.Filter = FILTER;

            _owner = owner;
        }

        #endregion

        #region Methods

        public void Load()
        {
            bool? result = _loadDlg.ShowDialog(_owner);
            if (result == true)
            {
                LoadPostProcessing(_xml.Load(_loadDlg.FileName));
            }
        }

        protected abstract void LoadPostProcessing(T item);

        public void Save(T item)
        {
            bool? result = _saveDlg.ShowDialog(_owner);
            if (result == true)
            {
                _xml.Save(_saveDlg.FileName, item);
            }
        }

        #endregion
    }
}
