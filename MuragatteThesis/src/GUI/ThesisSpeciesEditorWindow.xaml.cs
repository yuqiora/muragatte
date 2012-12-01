// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.IO;
using Muragatte.Thesis.IO;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisSpeciesEditorWindow.xaml
    /// </summary>
    public partial class ThesisSpeciesEditorWindow : Window
    {
        #region Fields

        private SpeciesCollection _species = null;

        private XmlSpeciesArchiver _xml = null;

        #endregion

        #region Constructors

        public ThesisSpeciesEditorWindow(SpeciesCollection species)
        {
            InitializeComponent();
            DataContext = this;

            _species = species ?? new SpeciesCollection(true);

            _xml = new XmlSpeciesArchiver(this);
        }

        #endregion

        #region Properties

        public SpeciesCollection GetSpecies
        {
            get { return _species; }
        }

        #endregion

        #region Events

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Species s = new Species("New");
            _species.Add(s);
            lboSpecies.SelectedItem = s;
        }

        private void btnSub_Click(object sender, RoutedEventArgs e)
        {
            if (lboSpecies.SelectedItem != null)
            {
                Species s = ((Species)lboSpecies.SelectedItem).CreateSubSpecies("Sub");
                _species.Add(s);
                lboSpecies.SelectedItem = s;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lboSpecies.SelectedItem != null)
            {
                _species.Remove(((Species)lboSpecies.SelectedItem));
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            _xml.Load();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _xml.Save(new XmlSpeciesCollectionRoot(_species));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _species.Clear();
        }

        #endregion
    }
}
