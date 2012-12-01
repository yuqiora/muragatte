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
using Muragatte.Thesis.IO;

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
        private ObservableCollection<Element> _stationaryElements = null;
        private SpeciesCollection _species = null;
        private List<Metric> _metrics = new List<Metric>() { Metric.Euclidean, Metric.Manhattan, Metric.Maximum };
        private List<Distribution> _distributions = new List<Distribution>() { Distribution.None, Distribution.Uniform, Distribution.Gaussian };
        private CollectionViewSource _goalsView = new CollectionViewSource();

        private AgentArchetype _selectedArchetype = null;
        private Neighbourhood _selectedArgsNeighbourhood = null;
        private string _selectedArgsModifierLabel = null;

        private Neighbourhood _defaultNeighbourhood = new Neighbourhood(10, new Angle(135));
        private Angle _defaultTurningAngle = new Angle(60);

        private XmlArchetypesArchiver _xml = null;

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
            _stationaryElements = scene.StationaryElements;

            _goalsView.Source = scene.StationaryElements;
            GoalsView.Filter += new Predicate<object>(o => (o as Element) is Goal);

            _xml = new XmlArchetypesArchiver(this);
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

        public double? SelectedArgsModifierValue
        {
            get { return _selectedArgsModifierLabel != null ? _selectedArchetype.Specifics.Modifiers[_selectedArgsModifierLabel] : (double?)null; }
            set
            {
                if (_selectedArgsModifierLabel != null && value.HasValue)
                {
                    _selectedArchetype.Specifics.Modifiers[_selectedArgsModifierLabel] = value.Value;
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

        private void btnSimpleBoid_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new SimpleBoidAgentArchetype("SimpleBoids", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new SimpleBoidAgentArgs()));
        }

        private void btnClassicBoid_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new ClassicBoidAgentArchetype("ClassicBoids", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new ClassicBoidAgentArgs(_defaultNeighbourhood.Clone(), _defaultNeighbourhood.Clone(), _defaultNeighbourhood.Clone())));
        }

        private void btnAdvancedBoid_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new AdvancedBoidAgentArchetype("AdvancedBoids", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new AdvancedBoidAgentArgs(_defaultNeighbourhood.Clone(), _defaultNeighbourhood.Clone(), _defaultNeighbourhood.Clone())));
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
            NewArchetype(new LoneWandererAgentArchetype("LoneWanderers", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, _defaultNeighbourhood.Clone(), _defaultTurningAngle,
                new LoneWandererAgentArgs()));
        }

        private void btnCouzin2005_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new Couzin2005AgentArchetype("Couzin2005", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, new Neighbourhood(6), Angle.FromRadians(2),
                new FlockAndSeekBaseAgentArgs(null, new Neighbourhood(1.5), 0.5)));
        }

        private void btnConradt2009_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new Conradt2009AgentArchetype("Conradt2009", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, new Neighbourhood(20), Angle.FromRadians(2),
                new FlockAndSeekBaseAgentArgs(null, new Neighbourhood(2.5), 0.5)));
        }

        private void btnVejmola2013_Click(object sender, RoutedEventArgs e)
        {
            NewArchetype(new Vejmola2013AgentArchetype("Vejmola2013", 10,
                _spawnSpots.FirstOrDefault(), new NoisedDouble(Distribution.Uniform, -180, 180),
                new NoisedDouble(1), null, new Neighbourhood(10), Angle.FromRadians(2),
                new Vejmola2013AgentArgs(null, new Neighbourhood(1.5), 0.5, 1)));
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            _xml.Load();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _xml.Save(new XmlArchetypesRoot(_archetypes));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _archetypes.Clear();
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
