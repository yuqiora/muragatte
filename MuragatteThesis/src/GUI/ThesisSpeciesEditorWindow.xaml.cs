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

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisSpeciesEditorWindow.xaml
    /// </summary>
    public partial class ThesisSpeciesEditorWindow : Window
    {
        #region Fields

        private SpeciesCollection _species = null;

        #endregion

        #region Constructors

        public ThesisSpeciesEditorWindow(SpeciesCollection species)
        {
            InitializeComponent();
            DataContext = this;

            _species = species ?? new SpeciesCollection(true);
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
            _species.Add(new Species("New"));
            lboSpecies.SelectedIndex = lboSpecies.Items.Count - 1;
        }

        private void btnSub_Click(object sender, RoutedEventArgs e)
        {
            if (lboSpecies.SelectedItem != null)
            {
                _species.Add(new Species("Sub", (Species)lboSpecies.SelectedValue));
                lboSpecies.SelectedIndex = lboSpecies.Items.Count - 1;
            }
        }

        //issues with SpeciesCollection when removing
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lboSpecies.SelectedItem != null)
            {
                _species.Remove(((Species)lboSpecies.SelectedValue).ID);
            }
        }

        #endregion
    }
}
