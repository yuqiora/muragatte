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
using System.Collections.ObjectModel;
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
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisArchetypesEditorWindow.xaml
    /// </summary>
    public partial class ThesisArchetypesEditorWindow : Window
    {
        #region Fields

        private ObservableCollection<AgentArchetype> _archetypes = null;
        private ObservableCollection<Element> _stationaries = null;
        private SpeciesCollection _species = null;

        #endregion

        #region Constructors
        public ThesisArchetypesEditorWindow(ObservableCollection<AgentArchetype> archetypes, ObservableCollection<Element> stationaries, SpeciesCollection species)
        {
            InitializeComponent();
            DataContext = this;

            _archetypes = archetypes;
            _stationaries = stationaries;
            _species = species;
        }

        #endregion

        #region Properties

        public ObservableCollection<AgentArchetype> GetArchetypes
        {
            get { return _archetypes; }
        }

        public ObservableCollection<Element> GetStationaries
        {
            get { return _stationaries; }
        }

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

        #endregion
    }
}
