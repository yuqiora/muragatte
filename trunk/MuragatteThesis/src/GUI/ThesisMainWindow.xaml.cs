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
            ThesisAboutWindow about = new ThesisAboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ThesisExperimentEditorWindow editor = new ThesisExperimentEditorWindow(this);
            editor.ShowDialog();
            //grbExperimentSummary.Visibility = System.Windows.Visibility.Visible;
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
            List<Core.Environment.SpawnSpot> spawns = new List<Core.Environment.SpawnSpot>();
            Core.Environment.SpawnSpot spawn = new Core.Environment.RectangleSpawnSpot(new Common.Vector2(70, 70), 120);
            spawns.Add(spawn);

            Core.Storage.SpeciesCollection species = new Core.Storage.SpeciesCollection();
            Core.Environment.Species boids = new Core.Environment.Species("Boids");
            Core.Environment.Species centroid = new Core.Environment.Species("Centroid");
            species.Add(boids);
            species.Add(centroid);
            species.SetDefaultCentroidSpecies(centroid);

            List<Core.Environment.AgentArchetype> archetypes = new List<Core.Environment.AgentArchetype>();
            archetypes.Add(new Core.Environment.Agents.BoidAgentArchetype("Boids", 50, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(1), boids,
                new Core.Environment.Neighbourhood(10, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.BoidAgentArgs()));

            List<Visual.Styles.Style> styles = new List<Visual.Styles.Style>();
            styles.Add(new Visual.Styles.Style(Visual.DefaultValues.AGENT_STYLE, "Boids"));
            styles.Add(new Visual.Styles.Style(Visual.DefaultValues.CENTROID_STYLE, "Centroid"));

            _experiment = new Experiment("Boids Sample", "", 5,
                new InstanceDefinition(0.25, 1000,
                    new Core.Scene(new Core.Environment.Region(140, false), spawns, new List<Core.Environment.Element>()),
                    species, archetypes),
                styles, _random.UInt());
        }

        private void PredefinedSampleThesis()
        {
            List<Core.Environment.SpawnSpot> spawns = new List<Core.Environment.SpawnSpot>();
            Core.Environment.SpawnSpot spawn = new Core.Environment.EllipseSpawnSpot(new Common.Vector2(70, 120), 30, 30);
            spawns.Add(spawn);

            Core.Storage.SpeciesCollection species = new Core.Storage.SpeciesCollection();
            Core.Environment.Species boid = new Core.Environment.Species("Boid");
            Core.Environment.Species naive = boid.CreateSubSpecies("Naive");
            Core.Environment.Species guide = boid.CreateSubSpecies("Guide");
            Core.Environment.Species intruder = boid.CreateSubSpecies("Intruder");
            Core.Environment.Species obstacle = new Core.Environment.Species("Obstacle");
            Core.Environment.Species goal = new Core.Environment.Species("Goal");
            Core.Environment.Species goalG = goal.CreateSubSpecies("gG");
            Core.Environment.Species goalI = goal.CreateSubSpecies("gI");
            Core.Environment.Species centroid = new Core.Environment.Species("Centroid");
            species.Add(boid);
            species.Add(naive);
            species.Add(guide);
            species.Add(intruder);
            species.Add(obstacle);
            species.Add(goal);
            species.Add(goalG);
            species.Add(goalI);
            species.Add(centroid);
            species.SetDefaultCentroidSpecies(centroid);

            List<Core.Environment.Element> stationaries = new List<Core.Environment.Element>();
            Core.Environment.Goal goalGuide = new Core.Environment.PositionGoal(0, null, new Common.Vector2(25, 25), goalG);
            Core.Environment.Goal goalIntruder = new Core.Environment.PositionGoal(1, null, new Common.Vector2(115, 25), goalI);
            stationaries.Add(goalGuide);
            stationaries.Add(goalIntruder);
            stationaries.Add(new Core.Environment.EllipseObstacle(2, null, new Common.Vector2(70, 55), obstacle, 20));

            List<Core.Environment.AgentArchetype> archetypes = new List<Core.Environment.AgentArchetype>();
            archetypes.Add(new Core.Environment.Agents.AdvancedBoidAgentArchetype("Naive Boids", 44, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(1.05), naive,
                new Core.Environment.Neighbourhood(7.5, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.AdvancedBoidAgentArgs(null,
                    new Core.Environment.Neighbourhood(2, new Common.Angle(135)), 0)));
            archetypes.Add(new Core.Environment.Agents.AdvancedBoidAgentArchetype("Guide Boids", 5, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(1), guide,
                new Core.Environment.Neighbourhood(7.5, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.AdvancedBoidAgentArgs(goalGuide,
                    new Core.Environment.Neighbourhood(2, new Common.Angle(135)), 0.75)));
            archetypes.Add(new Core.Environment.Agents.AdvancedBoidAgentArchetype("Intruder Boids", 1, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(0.95), intruder,
                new Core.Environment.Neighbourhood(7.5, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.AdvancedBoidAgentArgs(goalIntruder,
                    new Core.Environment.Neighbourhood(2, new Common.Angle(135)), 1)));

            List<Visual.Styles.Style> styles = new List<Visual.Styles.Style>();
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Obstacle", 20, 20, Colors.Gray, Colors.Gray, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gG", 1, 1, Colors.Blue, Colors.Blue, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gI", 1, 1, Colors.Red, Colors.Red, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Naive", 1, 1, Colors.Transparent, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightGreen, 10, new Common.Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Black), new Visual.Styles.TrailStyle(Colors.Black, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Guide", 1, 1, Colors.CornflowerBlue, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSkyBlue, 10, new Common.Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.RoyalBlue), new Visual.Styles.TrailStyle(Colors.CornflowerBlue, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Intruder", 1, 1, Colors.IndianRed, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSalmon, 10, new Common.Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Crimson), new Visual.Styles.TrailStyle(Colors.IndianRed, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Centroid", 1, 1, Colors.Silver, Colors.DimGray, null,
                new Visual.Styles.TrackStyle(Colors.DimGray), new Visual.Styles.TrailStyle(Colors.Silver, 10)));

            _experiment = new Experiment("Thesis Sample", "", 5,
                new InstanceDefinition(0.2, 1250, new Core.Scene(new Core.Environment.Region(140, true), spawns, stationaries), species, archetypes),
                styles, _random.UInt());
        }

        private void PredefinedSampleThesis2()
        {
            List<Core.Environment.SpawnSpot> spawns = new List<Core.Environment.SpawnSpot>();
            Core.Environment.SpawnSpot spawn = new Core.Environment.EllipseSpawnSpot(new Common.Vector2(125, 220), 40, 40);
            spawns.Add(spawn);

            Core.Storage.SpeciesCollection species = new Core.Storage.SpeciesCollection();
            Core.Environment.Species boid = new Core.Environment.Species("Boid");
            Core.Environment.Species naive = boid.CreateSubSpecies("Naive");
            Core.Environment.Species guide = boid.CreateSubSpecies("Guide");
            Core.Environment.Species intruder = boid.CreateSubSpecies("Intruder");
            Core.Environment.Species obstacle = new Core.Environment.Species("Obstacle");
            Core.Environment.Species goal = new Core.Environment.Species("Goal");
            Core.Environment.Species goalG = goal.CreateSubSpecies("gG");
            Core.Environment.Species goalI = goal.CreateSubSpecies("gI");
            Core.Environment.Species centroid = new Core.Environment.Species("Centroid");
            species.Add(boid);
            species.Add(naive);
            species.Add(guide);
            species.Add(intruder);
            species.Add(obstacle);
            species.Add(goal);
            species.Add(goalG);
            species.Add(goalI);
            species.Add(centroid);
            species.SetDefaultCentroidSpecies(centroid);

            List<Core.Environment.Element> stationaries = new List<Core.Environment.Element>();
            Core.Environment.Goal goalGuide = new Core.Environment.PositionGoal(0, null, new Common.Vector2(25, 25), goalG);
            Core.Environment.Goal goalIntruder = new Core.Environment.PositionGoal(1, null, new Common.Vector2(225, 25), goalI);
            stationaries.Add(goalGuide);
            stationaries.Add(goalIntruder);

            List<Core.Environment.AgentArchetype> archetypes = new List<Core.Environment.AgentArchetype>();
            archetypes.Add(new Core.Environment.Agents.AdvancedBoidAgentArchetype("Naive Boids", 89, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(1.05), naive,
                new Core.Environment.Neighbourhood(10, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.AdvancedBoidAgentArgs(null,
                    new Core.Environment.Neighbourhood(2, new Common.Angle(135)), 0)));
            archetypes.Add(new Core.Environment.Agents.AdvancedBoidAgentArchetype("Guide Boids", 10, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(1), guide,
                new Core.Environment.Neighbourhood(10, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.AdvancedBoidAgentArgs(goalGuide,
                    new Core.Environment.Neighbourhood(2, new Common.Angle(135)), 0.75)));
            archetypes.Add(new Core.Environment.Agents.AdvancedBoidAgentArchetype("Intruder Boids", 1, spawn,
                Common.Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -180, 180),
                new NoisedDouble(0.95), intruder,
                new Core.Environment.Neighbourhood(10, new Common.Angle(135)), new Common.Angle(60),
                new Core.Environment.Agents.AdvancedBoidAgentArgs(goalIntruder,
                    new Core.Environment.Neighbourhood(2, new Common.Angle(135)), 1)));

            List<Visual.Styles.Style> styles = new List<Visual.Styles.Style>();
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Obstacle", 20, 20, Colors.Gray, Colors.Gray, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gG", 1, 1, Colors.Blue, Colors.Blue, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.EllipseShape.Instance, "Goal.gI", 1, 1, Colors.Red, Colors.Red, null, null, null));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Naive", 1, 1, Colors.Transparent, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightGreen, 10, new Common.Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Black), new Visual.Styles.TrailStyle(Colors.Black, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Guide", 1, 1, Colors.CornflowerBlue, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSkyBlue, 10, new Common.Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.RoyalBlue), new Visual.Styles.TrailStyle(Colors.CornflowerBlue, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Boid.Intruder", 1, 1, Colors.IndianRed, Colors.Black,
                new Visual.Styles.NeighbourhoodStyle(Visual.Shapes.ArcShape.Instance, Colors.Transparent, Colors.LightSalmon, 10, new Common.Angle(135), 1),
                new Visual.Styles.TrackStyle(Colors.Crimson), new Visual.Styles.TrailStyle(Colors.IndianRed, 10)));
            styles.Add(new Visual.Styles.Style(Visual.Shapes.PointingCircleShape.Instance, "Centroid", 1, 1, Colors.Silver, Colors.DimGray, null,
                new Visual.Styles.TrackStyle(Colors.DimGray), new Visual.Styles.TrailStyle(Colors.Silver, 10)));

            _experiment = new Experiment("Thesis Sample", "", 5,
                new InstanceDefinition(0.2, 1250, new Core.Scene(new Core.Environment.Region(250, true), spawns, stationaries), species, archetypes),
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

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment != null)
            {
                ThesisExperimentEditorWindow editor = new ThesisExperimentEditorWindow(this);
                editor.ShowDialog();
                ShowExperimentSummary();
            }
        }

        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment.IsComplete)
            {
                ThesisExperimentResultsWindow results = new ThesisExperimentResultsWindow(_experiment);
                results.ShowDialog();
            }
        }
    }
}
