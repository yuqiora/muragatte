// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Sandbox Application
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Environment.Agents;
using Muragatte.Core.Storage;
using Muragatte.Random;
using Muragatte.Visual;

//temporary for now, mainly to test if things work as they should
//will be completely reworked later when Core and Visual are much more functional

namespace Muragatte.Sandbox.GUI
{
    /// <summary>
    /// Interaction logic for SandboxMainWindow.xaml
    /// </summary>
    public partial class SandboxMainWindow : Window
    {
        #region Constants

        private const double TIME_PER_STEP = 0.25;
        private const int DEFAULT_DELAY = 15;

        #endregion

        #region Fields

        private MultiAgentSystem _mas = null;
        private Visualization _visual = null;
        private bool _bPlaying = false;
        private List<Goal> _goals = new List<Goal>();
        private List<Obstacle> _obstacles = new List<Obstacle>();
        private RandomMT _random;

        private Angle _boidFOVAngle = new Angle(DefaultValues.NEIGHBOURHOOD_ANGLE_DEGREES);
        private Angle _turningAngle = new Angle(60);
        private Species _boids = null;
        private Species _guides = null;
        private Species _intruders = null;
        private SpawnSpot _scatteredSpawn = null;
        private SpawnSpot _groupedSpawn = null;

        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public SandboxMainWindow()
        {
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            _random = new RandomMT();
            InitializeComponent();
        }

        #endregion

        #region Button Events
        
        private void btnEnvironment_Click(object sender, RoutedEventArgs e)
        {
            CreateObstacles();
            CreateGoals();
            //_visual.GetCanvas.Redraw();
            Redraw();
        }

        private void btnAgents_Click(object sender, RoutedEventArgs e)
        {
            double fovRange = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            int agentCount = int.Parse(txtAgentCount.Text);
            CreateBoids(agentCount, fovRange);
            Initialize();
        }

        private void btnAgentsWG_Click(object sender, RoutedEventArgs e)
        {
            double fovRange = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            double paRange = double.Parse(txtPersonalArea.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            int agentCount = int.Parse(txtAgentCount.Text);
            int guideCount = int.Parse(txtGuideCount.Text);
            int intruderCount = int.Parse(txtIntruderCount.Text);
            int naiveCount = agentCount - guideCount - intruderCount;
            CreateAdvancedBoids(naiveCount, guideCount, intruderCount, fovRange, paRange);
            Initialize();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            prbUpdate.Visibility = System.Windows.Visibility.Visible;
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _mas.Clear();
            _goals.Clear();
            _obstacles.Clear();
            _visual.GetCanvas.Clear();
            txtSteps.Text = "0";
        }

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            _goals.Clear();
            _obstacles.Clear();
            txtSteps.Text = "0";
            int width = int.Parse(txtWidth.Text);
            int height = int.Parse(txtHeight.Text);
            _scatteredSpawn = new RectangleSpawnSpot(new Vector2(width / 2.0, height / 2.0), width, height);
            _groupedSpawn = new EllipseSpawnSpot(new Vector2(width / 2.0, height / 2.0), width / 5.0, height / 5.0);
            _mas = new MultiAgentSystem(/*new OrthantNeighbourhoodGraphStorage()/*/ new SimpleBruteForceStorage(), new Region(
                width, height, chbHorizontal.IsChecked.Value, chbVertical.IsChecked.Value), _random, TIME_PER_STEP);
            double scale = double.Parse(txtScale.Text);
            if (_visual != null)
            {
                _visual.Close();
            }
            _visual = new Visualization(_mas, width, height, scale, this);
            _visual.Initialize();
            CreateSpecies();
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_bPlaying)
            {
                _bPlaying = false;
                btnPlayPause.Content = "Play";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
            else
            {
                _bPlaying = true;
                btnPlayPause.Content = "Pause";
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        private void btnScatter_Click(object sender, RoutedEventArgs e)
        {
            _mas.Scatter();
        }

        private void btnGroup_Click(object sender, RoutedEventArgs e)
        {
            FormGroup();
        }

        #endregion

        #region Checkbox Events

        private void chbHorizontal_Checked(object sender, RoutedEventArgs e)
        {
            HorizontalBorders();
        }

        private void chbHorizontal_Unchecked(object sender, RoutedEventArgs e)
        {
            HorizontalBorders();
        }

        private void chbVertical_Checked(object sender, RoutedEventArgs e)
        {
            VerticalBorders();
        }

        private void chbVertical_Unchecked(object sender, RoutedEventArgs e)
        {
            VerticalBorders();
        }

        #endregion

        #region Other Events

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            prbUpdate.Visibility = System.Windows.Visibility.Hidden;
            Redraw();
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prbUpdate.Value = e.ProgressPercentage;
            txtSteps.Text = _mas.StepCount.ToString();
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int stepsToUpdate = 500;
            for (int i = 0; i < stepsToUpdate; i++)
            {
                _mas.Update();
                _worker.ReportProgress(100 * i / stepsToUpdate);
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateAndRedraw();
        }

        #endregion

        #region Methods

        private void UpdateAndRedraw()
        {
            _mas.Update();
            txtSteps.Text = _mas.StepCount.ToString();
            Redraw();
        }

        private void CreateGoals()
        {
            _goals.Add(new PositionGoal(_mas, _random.UniformVector(0, _mas.Region.Width, 0, _mas.Region.Height), null));
            _goals.Add(new PositionGoal(_mas, _random.UniformVector(0, _mas.Region.Width, 0, _mas.Region.Height), null));
            _mas.Elements.Add(_goals);
        }

        private void CreateObstacles()
        {
            int obstacles = int.Parse(txtObstacles.Text);
            for (int i = 0; i < obstacles; i++)
            {
                _obstacles.Add(new EllipseObstacle(_mas, _random.UniformVector(0, _mas.Region.Width, 0, _mas.Region.Height), null, Math.Round(_random.Uniform(10, 30), 2)));
            }
            _mas.Elements.Add(_obstacles);
        }
        
        private void CreateSpecies()
        {
            _mas.Species.Clear();
            Species.ResetIDCounter();
            _mas.Species.CreateDefaultSpecies();
            int scale = int.Parse(txtScale.Text);
            _boids = new Species("Boids");
            _mas.Species.Add(_boids.ID, _boids);
            _guides = _boids.CreateSubSpecies("Guides");
            _mas.Species.Add(_guides.ID, _guides);
            _intruders = _boids.CreateSubSpecies("Intruders");
            _mas.Species.Add(_intruders.ID, _intruders);
            //Species centroids = new Species("Centroids");
            //_mas.Species.Add(centroids.ID, centroids);
            Species wanderers = new Species("Wanderers");
            _mas.Species.Add(wanderers.ID, wanderers);
            Species versatiles = new Species("Versatiles");
            _mas.Species.Add(versatiles.ID, versatiles);
        }

        private void CreateBoids(int count, double fovRange)
        {
            BoidAgentArchetype ba = new BoidAgentArchetype(
                "Boids", count, StartingPosition(),
                Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -Angle.MaxDegree, Angle.MaxDegree),
                new NoisedDouble(1), _boids,
                new Neighbourhood(fovRange, _boidFOVAngle), _turningAngle,
                new BoidAgentArgs(1, 1, 1, Distribution.Gaussian, 0, 2));
            _mas.Elements.Add(ba.CreateAgents(_mas.Elements.Count, _mas));
        }

        private void CreateAdvancedBoids(int naive, int guides, int intruders, double fovRange, double paRange)
        {
            Neighbourhood pa = new Neighbourhood(paRange);
            AdvancedBoidAgentArchetype naiveArch = new AdvancedBoidAgentArchetype(
                "Naive", naive, StartingPosition(),
                Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -Angle.MaxDegree, Angle.MaxDegree),
                new NoisedDouble(1.05), _boids,
                new Neighbourhood(fovRange, _boidFOVAngle), _turningAngle,
                new AdvancedBoidAgentArgs(null, pa.Clone(), 0, 1, 1, 1, 1, 10, Distribution.Gaussian, 0, 2));
            _mas.Elements.Add(naiveArch.CreateAgents(_mas.Elements.Count, _mas));

            AdvancedBoidAgentArchetype guideArch = new AdvancedBoidAgentArchetype(
                "Guide", guides, StartingPosition(),
                Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -Angle.MaxDegree, Angle.MaxDegree),
                new NoisedDouble(1), _guides,
                new Neighbourhood(fovRange, _boidFOVAngle), _turningAngle,
                new AdvancedBoidAgentArgs(_goals[0], pa.Clone(), 0.75, 1, 1, 1, 1, 10, Distribution.Gaussian, 0, 2));
            _mas.Elements.Add(guideArch.CreateAgents(_mas.Elements.Count, _mas));

            AdvancedBoidAgentArchetype intruderArch = new AdvancedBoidAgentArchetype(
                "Intruder", intruders, StartingPosition(),
                Vector2.X0Y1, new NoisedDouble(Distribution.Uniform, _random, -Angle.MaxDegree, Angle.MaxDegree),
                new NoisedDouble(0.95), _intruders,
                new Neighbourhood(fovRange, _boidFOVAngle), _turningAngle,
                new AdvancedBoidAgentArgs(_goals[1], pa.Clone(), 1, 1, 1, 1, 1, 10, Distribution.Gaussian, 0, 2));
            _mas.Elements.Add(intruderArch.CreateAgents(_mas.Elements.Count, _mas));
        }

        private void HorizontalBorders()
        {
            if (_mas != null)
            {
                _mas.Region.IsBorderedHorizontally = chbHorizontal.IsChecked.Value;
            }
        }

        private void VerticalBorders()
        {
            if (_mas != null)
            {
                _mas.Region.IsBorderedVertically = chbVertical.IsChecked.Value;
            }
        }

        private void Redraw()
        {
            if (chbVisualize.IsChecked.Value)
            {
                //_visual.GetCanvas.Redraw();
                _visual.GetCanvas.Redraw(_mas.History);
            }
        }

        private void Initialize()
        {
            //if (cmbStartState.SelectedIndex == 0)
            //{ _mas.Scatter(); }
            //else
            //{ FormGroup(); }
            _mas.Initialize();
            //SetCentroidSpecies(_mas.Elements.Centroids, _mas.Species[3]);
            Redraw();
        }

        private void FormGroup()
        {
            //double fov = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            //_mas.GroupStart(fov * 3);
            double pa = double.Parse(txtPersonalArea.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            double size = Math.Sqrt(int.Parse(txtAgentCount.Text)) * pa;
            _mas.GroupStart(size);
        }

        private SpawnSpot StartingPosition()
        {
            return cmbStartState.SelectedIndex == 0 ? _scatteredSpawn : _groupedSpawn;
        }

        #endregion
    }
}
