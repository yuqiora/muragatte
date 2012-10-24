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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Muragatte.Random;

using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Environment.Agents;
using Muragatte.Core.Storage;
using Muragatte.Visual;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisMainWindow.xaml
    /// </summary>
    public partial class ThesisMainWindow : Window
    {
        #region Fields

        private RandomMT _random = new RandomMT();
        //private Experiment _experiment = new Experiment("", "", 1, new InstanceDefinition(1, 1, new Core.Scene(new Core.Environment.Region(100, true)), new Core.Storage.SpeciesCollection(), new List<Core.Environment.AgentArchetype>()), new List<Visual.Styles.Style>(), 0);
        private Experiment _experiment = null;

        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public ThesisMainWindow()
        {
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);

            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Properties

        public Experiment Experiment
        {
            get { return _experiment; }
            set { _experiment = value; }
        }

        public RandomMT Random
        {
            get { return _random; }
        }

        #endregion

        #region Events

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            OpenDialogWindow(new ThesisAboutWindow());
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            OpenDialogWindow(new ThesisExperimentEditorWindow(this));
            ShowExperimentSummary();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment != null && !_worker.IsBusy)
            {
                UpdateProgressInfo(0, 0);
                binProgress.IsBusy = true;
                _worker.RunWorkerAsync();
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment != null)
            {
                OpenDialogWindow(new ThesisExperimentEditorWindow(this));
                ShowExperimentSummary();
            }
        }

        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment.IsComplete)
            {
                OpenDialogWindow(new ThesisExperimentResultsWindow(_experiment));
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            _experiment.Reset();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _experiment = null;
            ShowExperimentSummary();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _worker.CancelAsync();
            _experiment.Cancel();
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            binProgress.IsBusy = false;
            btnResults.IsEnabled = _experiment.IsComplete;
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<int, int> info = e.UserState as Tuple<int, int>;
            UpdateProgressInfo(info.Item1, info.Item2);
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _experiment.PreProcessing();
            for (int i = 0; i < _experiment.RepeatCount; i++)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                while (!_experiment.Instances[i].IsComplete)
                {
                    _experiment.Instances[i].Update();
                    _worker.ReportProgress(0, new Tuple<int, int>(i, _experiment.Instances[i].Model.StepCount));
                }
            }
            _experiment.PostProcessing();
        }

        #endregion

        #region Methods

        private void PredefinedSampleBoids()
        {
            List<SpawnSpot> spawns = new List<SpawnSpot>();
            SpawnSpot spawn = new RectangleSpawnSpot(new Vector2(70, 70), 120);
            spawns.Add(spawn);

            SpeciesCollection species = new SpeciesCollection();
            Species boids = new Species("Boids");
            Species centroid = new Species("Centroid");
            species.Add(boids, SpeciesCollection.DEFAULT_AGENTS_LABEL);
            species.Add(centroid, SpeciesCollection.DEFAULT_CENTROIDS_LABEL);

            List<ObservedArchetype> archetypes = new List<ObservedArchetype>();
            archetypes.Add(new ObservedArchetype(new BoidAgentArchetype("Boids", 50, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(1), boids,
                new Neighbourhood(10, new Angle(135)), new Angle(60),
                new BoidAgentArgs())));

            List<Visual.Styles.Style> styles = new List<Visual.Styles.Style>();
            styles.Add(new Visual.Styles.Style(Visual.DefaultValues.AGENT_STYLE, "Boids"));
            styles.Add(new Visual.Styles.Style(Visual.DefaultValues.CENTROID_STYLE, "Centroid"));

            _experiment = new Experiment("Boids Sample", "", 5,
                new InstanceDefinition(0.25, 1000, new Scene(new Region(140, false),
                    spawns, new List<Element>()), species, archetypes),
                styles, _random.UInt());
        }

        private void PredefinedSampleThesis()
        {
            List<SpawnSpot> spawns = new List<SpawnSpot>();
            SpawnSpot spawn = new EllipseSpawnSpot(new Vector2(70, 120), 30, 30);
            spawns.Add(spawn);

            SpeciesCollection species = new SpeciesCollection();
            Species boid = new Species("Boid");
            Species naive = boid.CreateSubSpecies("Naive");
            Species guide = boid.CreateSubSpecies("Guide");
            Species intruder = boid.CreateSubSpecies("Intruder");
            Species obstacle = new Core.Environment.Species("Obstacle");
            Species goal = new Core.Environment.Species("Goal");
            Species goalG = goal.CreateSubSpecies("gG");
            Species goalI = goal.CreateSubSpecies("gI");
            Species centroid = new Core.Environment.Species("Centroid");
            species.Add(boid, SpeciesCollection.DEFAULT_AGENTS_LABEL);
            species.Add(naive);
            species.Add(guide);
            species.Add(intruder);
            species.Add(obstacle, SpeciesCollection.DEFAULT_OBSTACLES_LABEL);
            species.Add(goal, SpeciesCollection.DEFAULT_GOALS_LABEL);
            species.Add(goalG);
            species.Add(goalI);
            species.Add(centroid, SpeciesCollection.DEFAULT_CENTROIDS_LABEL);

            List<Element> stationaries = new List<Element>();
            Goal goalGuide = new PositionGoal(0, null, new Vector2(25, 25), goalG);
            Goal goalIntruder = new PositionGoal(1, null, new Vector2(115, 25), goalI);
            stationaries.Add(goalGuide);
            stationaries.Add(goalIntruder);
            stationaries.Add(new EllipseObstacle(2, null, new Vector2(70, 55), obstacle, 20));

            List<ObservedArchetype> archetypes = new List<ObservedArchetype>();
            archetypes.Add(new ObservedArchetype(new AdvancedBoidAgentArchetype("Naive Boids", 44, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(1.05), naive,
                new Neighbourhood(7.5, new Angle(135)), new Angle(60),
                new AdvancedBoidAgentArgs(null, new Neighbourhood(2, new Angle(135)), 0))));
            archetypes.Add(new ObservedArchetype(new AdvancedBoidAgentArchetype("Guide Boids", 5, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(1), guide,
                new Neighbourhood(7.5, new Angle(135)), new Angle(60),
                new AdvancedBoidAgentArgs(goalGuide, new Neighbourhood(2, new Angle(135)), 0.75))));
            archetypes.Add(new ObservedArchetype(new AdvancedBoidAgentArchetype("Intruder Boids", 1, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(0.95), intruder,
                new Neighbourhood(7.5, new Angle(135)), new Angle(60),
                new AdvancedBoidAgentArgs(goalIntruder, new Neighbourhood(2, new Angle(135)), 1))));
            archetypes[1].IsObserved = true;
            archetypes[2].IsObserved = true;

            List<Visual.Styles.Style> styles = new List<Visual.Styles.Style>();
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Obstacle", 20, 20, Colors.Gray, Colors.Gray, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gG", 1, 1, Colors.Blue, Colors.Blue, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gI", 1, 1, Colors.Red, Colors.Red, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Naive", 1, 1, Colors.Transparent, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightGreen, 10, new Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Black), new Visual.Styles.TrailStyle(Colors.Black, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Guide", 1, 1, Colors.CornflowerBlue, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSkyBlue, 10, new Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.RoyalBlue), new Visual.Styles.TrailStyle(Colors.CornflowerBlue, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Intruder", 1, 1, Colors.IndianRed, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSalmon, 10, new Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Crimson), new Visual.Styles.TrailStyle(Colors.IndianRed, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Centroid", 1, 1, Colors.Silver, Colors.DimGray, null,
                new Visual.Styles.TrackStyle(Colors.DimGray), new Visual.Styles.TrailStyle(Colors.Silver, 10)));

            _experiment = new Experiment("Thesis Sample", "", 5,
                new InstanceDefinition(0.2, 1250, new Scene(new Region(140, true), spawns, stationaries), species, archetypes),
                styles, _random.UInt());
        }

        private void PredefinedSampleThesis2()
        {
            List<SpawnSpot> spawns = new List<SpawnSpot>();
            SpawnSpot spawn = new EllipseSpawnSpot(new Vector2(125, 205), 50, 50);
            spawns.Add(spawn);

            SpeciesCollection species = new SpeciesCollection();
            Species boid = new Species("Boid");
            Species naive = boid.CreateSubSpecies("Naive");
            Species guide = boid.CreateSubSpecies("Guide");
            Species intruder = boid.CreateSubSpecies("Intruder");
            Species obstacle = new Species("Obstacle");
            Species goal = new Core.Environment.Species("Goal");
            Species goalG = goal.CreateSubSpecies("gG");
            Species goalI = goal.CreateSubSpecies("gI");
            Species centroid = new Species("Centroid");
            species.Add(boid, SpeciesCollection.DEFAULT_AGENTS_LABEL);
            species.Add(naive);
            species.Add(guide);
            species.Add(intruder);
            species.Add(obstacle, SpeciesCollection.DEFAULT_OBSTACLES_LABEL);
            species.Add(goal, SpeciesCollection.DEFAULT_GOALS_LABEL);
            species.Add(goalG);
            species.Add(goalI);
            species.Add(centroid, SpeciesCollection.DEFAULT_CENTROIDS_LABEL);

            List<Element> stationaries = new List<Element>();
            Goal goalGuide = new PositionGoal(0, null, new Vector2(25, 25), goalG);
            Goal goalIntruder = new PositionGoal(1, null, new Vector2(225, 25), goalI);
            stationaries.Add(goalGuide);
            stationaries.Add(goalIntruder);

            List<ObservedArchetype> archetypes = new List<ObservedArchetype>();
            archetypes.Add(new ObservedArchetype(new AdvancedBoidAgentArchetype("Naive Boids", 89, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(1.05), naive,
                new Neighbourhood(10, new Angle(135)), new Angle(60),
                new AdvancedBoidAgentArgs(null, new Neighbourhood(2, new Angle(135)), 0))));
            archetypes.Add(new ObservedArchetype(new AdvancedBoidAgentArchetype("Guide Boids", 10, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(1), guide,
                new Neighbourhood(10, new Angle(135)), new Angle(60),
                new AdvancedBoidAgentArgs(goalGuide, new Neighbourhood(2, new Angle(135)), 0.75))));
            archetypes.Add(new ObservedArchetype(new AdvancedBoidAgentArchetype("Intruder Boids", 1, spawn,
                new NoisedDouble(Distribution.Uniform, _random, -180, 180), new NoisedDouble(0.95), intruder,
                new Neighbourhood(10, new Angle(135)), new Angle(60),
                new AdvancedBoidAgentArgs(goalIntruder, new Neighbourhood(2, new Angle(135)), 1))));
            archetypes[1].IsObserved = true;
            archetypes[2].IsObserved = true;

            List<Visual.Styles.Style> styles = new List<Visual.Styles.Style>();
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Obstacle", 20, 20, Colors.Gray, Colors.Gray, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gG", 1, 1, Colors.Blue, Colors.Blue, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gI", 1, 1, Colors.Red, Colors.Red, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Naive", 1, 1, Colors.Transparent, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightGreen, 10, new Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Black), new Visual.Styles.TrailStyle(Colors.Black, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Guide", 1, 1, Colors.CornflowerBlue, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSkyBlue, 10, new Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.RoyalBlue), new Visual.Styles.TrailStyle(Colors.CornflowerBlue, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Intruder", 1, 1, Colors.IndianRed, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSalmon, 10, new Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Crimson), new Visual.Styles.TrailStyle(Colors.IndianRed, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Centroid", 1, 1, Colors.Silver, Colors.DimGray, null,
                new Visual.Styles.TrackStyle(Colors.DimGray), new Visual.Styles.TrailStyle(Colors.Silver, 10)));

            _experiment = new Experiment("Thesis Sample", "", 5,
                new InstanceDefinition(0.2, 1250, new Scene(new Region(250, true), spawns, stationaries), species, archetypes),
                styles, _random.UInt());
        }

        private void UpdateProgressInfo(int cycle, int step)
        {
            int length = _experiment.Definition.Length;
            int repeat = _experiment.RepeatCount;
            txbInstanceProgress.Text = string.Format("{0} / {1}", step, length);
            prbInstance.Value = 100d * step / length;
            txbExperimentProgress.Text = string.Format("{0} / {1}", cycle, repeat);
            prbExperiment.Value = 100d * (cycle * length + step) / (repeat * length);
        }

        private void ShowExperimentSummary()
        {
            if (_experiment != null)
            {
                grbExperimentSummary.Header = _experiment.Name;
                txbPath.Text = "Location: " + _experiment.Path;
                txbTotalSteps.Text = string.Format("Cycles x Length: {0}x{1}", _experiment.RepeatCount, _experiment.Definition.Length);
                lboArchetypes.ItemsSource = _experiment.Definition.Archetypes;
                btnResults.IsEnabled = _experiment.IsComplete;
                grbExperimentSummary.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                grbExperimentSummary.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void OpenDialogWindow(Window window)
        {
            window.Owner = this;
            window.ShowDialog();
        }

        #endregion

        private void btnBoidsSample_Click(object sender, RoutedEventArgs e)
        {
            PredefinedSampleBoids();
            ShowExperimentSummary();
        }

        private void btnThesisSample_Click(object sender, RoutedEventArgs e)
        {
            PredefinedSampleThesis();
            ShowExperimentSummary();
        }

        private void btnThesisSample2_Click(object sender, RoutedEventArgs e)
        {
            PredefinedSampleThesis2();
            ShowExperimentSummary();
        }
    }
}
