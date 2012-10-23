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
using System.ComponentModel;
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
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Environment.Agents;
using Muragatte.Core.Storage;
using Muragatte.Random;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisArchetypesEditorWindow.xaml
    /// </summary>
    public partial class ThesisArchetypesEditorWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<ObservedArchetype> _archetypes = null;
        private ObservableCollection<SpawnSpot> _spawnSpots = null;
        private SpeciesCollection _species = null;
        private List<Metric> _metrics = new List<Metric>() { Metric.Euclidean, Metric.Manhattan, Metric.Maximum };
        private List<Distribution> _distributions = new List<Distribution>() { Distribution.None, Distribution.Uniform, Distribution.Gaussian };
        private CollectionViewSource _goalsView = new CollectionViewSource();

        private AgentArchetype _selectedArchetype = null;
        private Neighbourhood _selectedArgsNeighbourhood = null;
        private string _selectedArgsModifierLabel = null;

        private Neighbourhood _defaultNeighbourhood = new Neighbourhood(10, new Angle(135));
        private Angle _defaultTurningAngle = new Angle(60);

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors
        public ThesisArchetypesEditorWindow(ObservableCollection<ObservedArchetype> archetypes, Scene scene, SpeciesCollection species)
        {
            InitializeComponent();
            DataContext = this;

            _archetypes = archetypes;
            _species = species;
            _spawnSpots = scene.SpawnSpots;

            _goalsView.Source = scene.StationaryElements;
            GoalsView.Filter += new Predicate<object>(o => (o as Element) is Goal);
        }

        #endregion

        #region Properties

        public ObservableCollection<ObservedArchetype> GetArchetypes
        {
            get { return _archetypes; }
        }

        public ObservableCollection<SpawnSpot> GetSpawnSpots
        {
            get { return _spawnSpots; }
        }

        public SpeciesCollection GetSpecies
        {
            get { return _species; }
        }

        public List<Metric> GetMetricOptions
        {
            get { return _metrics; }
        }

        public List<Distribution> GetDistributionOptions
        {
            get { return _distributions; }
        }

        public ICollectionView GoalsView
        {
            get { return _goalsView.View; }
        }

        public AgentArchetype SelectedArchetype
        {
            get { return _selectedArchetype; }
            set
            {
                _selectedArchetype = value;
                NotifyPropertyChanged("SelectedArchetype");
            }
        }

        public Neighbourhood SelectedArgsNeighbourhood
        {
            get { return _selectedArgsNeighbourhood; }
            set
            {
                _selectedArgsNeighbourhood = value;
                NotifyPropertyChanged("SelectedArgsNeighbourhood");
            }
        }

        public string SelectedArgsModifierLabel
        {
            get { return _selectedArgsModifierLabel; }
            set
            {
                _selectedArgsModifierLabel = value;
                NotifyPropertyChanged("SelectedArgsModifierLabel");
                NotifyPropertyChanged("SelectedArgsModifierValue");
            }
        }

        public double SelectedArgsModifierValue
        {
            get { return _selectedArgsModifierLabel != null ? _selectedArchetype.Specifics.Modifiers[_selectedArgsModifierLabel] : double.NaN; }
            set
            {
                if (_selectedArgsModifierLabel != null)
                {
                    _selectedArchetype.Specifics.Modifiers[_selectedArgsModifierLabel] = value;
                    NotifyPropertyChanged("SelectedArgsModifierValue");
                }
            }
        }

        #endregion

        #region Events

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lboArchetypes.SelectedIndex >= 0)
            {
                _archetypes.RemoveAt(lboArchetypes.SelectedIndex);
            }
        }

        private void btnNewBoid_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new BoidAgentArchetype("Boids", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new BoidAgentArgs()));
        }

        private void btnNewAdvancedBoid_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new AdvancedBoidAgentArchetype("Advanced Boids", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new AdvancedBoidAgentArgs(null, new Neighbourhood(2))));
        }

        private void btnVersatile_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new VersatileAgentArchetype("Versatiles", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new VersatileAgentArgs(null, new Neighbourhood(2))));
        }

        private void btnLoneWanderer_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new LoneWandererAgentArchetype("Lone Wanderers", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new LoneWandererAgentArgs()));
        }

        #endregion

        #region Methods

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NewArchetype(AgentArchetype a)
        {
            ddbNew.IsOpen = false;
            _archetypes.Add(new ObservedArchetype(a));
            lboArchetypes.SelectedIndex = lboArchetypes.Items.Count - 1;
        }

        #endregion
    }
}
