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
using Muragatte.Sandbox;
using Muragatte.Visual;

//temporary for now, mainly to test if things work as they should
//will be completely reworked later when Core and Visual are much more functional

namespace Muragatte.GUI
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

        private Angle _boidFOVAngle = new Angle(DefaultValues.NEIGHBOURHOOD_ANGLE_DEGREES);

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
            _mas = new MultiAgentSystem(/*new OrthantNeighbourhoodGraphStorage()/*/ new SimpleBruteForceStorage(), new Region(
                width, height, chbHorizontal.IsChecked.Value, chbVertical.IsChecked.Value), TIME_PER_STEP);
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
            _goals.Add(new PositionGoal(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height)));
            _goals.Add(new PositionGoal(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height)));
            _mas.Elements.Add(_goals);
        }

        private void CreateObstacles()
        {
            int obstacles = int.Parse(txtObstacles.Text);
            for (int i = 0; i < obstacles; i++)
            {
                _obstacles.Add(new EllipseObstacle(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height), Math.Round(RNGs.Uniform(10, 30), 2)));
            }
            _mas.Elements.Add(_obstacles);
        }
        
        private void CreateSpecies()
        {
            _mas.Species.Clear();
            Species.ResetIDCounter();
            int scale = int.Parse(txtScale.Text);
            Species boids = new Species("Boids");
            _mas.Species.Add(boids.ID, boids);
            Species guides = boids.CreateSubSpecies("Guides");
            _mas.Species.Add(guides.ID, guides);
            Species intruders = boids.CreateSubSpecies("Intruders");
            _mas.Species.Add(intruders.ID, intruders);
            Species centroids = new Species("Centroids");
            _mas.Species.Add(centroids.ID, centroids);
            Species wanderers = new Species("Wanderers");
            _mas.Species.Add(wanderers.ID, wanderers);
            Species versatiles = new Species("Versatiles");
            _mas.Species.Add(versatiles.ID, versatiles);
        }

        private void CreateBoids(int count, double fovRange)
        {
            for (int i = 0; i < count; i++)
            {
                Neighbourhood n = new Neighbourhood(fovRange, _boidFOVAngle);
                //Agent a = new BoidAgent(_mas, n, new Angle(60));
                Agent a = new BoidAgent(_mas, n, new Angle(60), new BoidAgentArgs(1, 1, 1));
                a.Speed = 1;
                a.Species = _mas.Species[0];
                _mas.Elements.Add(a);
            }
        }

        private void CreateAdvancedBoids(int naive, int guides, int intruders, double fovRange, double paRange)
        {
            Angle turn = new Angle(60);
            Angle fovAng = new Angle(150);
            for (int i = 0; i < naive; i++)
            {
                Neighbourhood n = new Neighbourhood(fovRange, fovAng);
                Neighbourhood n2 = new Neighbourhood(paRange);
                //Agent a = new AdvancedBoidAgent(_mas, n, turn, null, 0, n2);
                Agent a = new AdvancedBoidAgent(_mas, n, turn, new AdvancedBoidAgentArgs(null, n2, 0, 1, 1, 1, 1, 10));
                a.Speed = 1.05;
                a.Species = _mas.Species[0];
                _mas.Elements.Add(a);
            }
            for (int i = 0; i < guides; i++)
            {
                Neighbourhood n = new Neighbourhood(fovRange, fovAng);
                Neighbourhood n2 = new Neighbourhood(paRange);
                //Agent a = new AdvancedBoidAgent(_mas, n, turn, _goals[0], 0.75, n2);
                Agent a = new AdvancedBoidAgent(_mas, n, turn, new AdvancedBoidAgentArgs(_goals[0], n2, 0.75, 1, 1, 1, 1, 10));
                a.Speed = 1;
                a.Species = _mas.Species[1];
                _mas.Elements.Add(a);
            }
            for (int i = 0; i < intruders; i++)
            {
                Neighbourhood n = new Neighbourhood(fovRange, fovAng);
                Neighbourhood n2 = new Neighbourhood(paRange);
                //Agent a = new AdvancedBoidAgent(_mas, n, turn, _goals[1], 1, n2);
                Agent a = new AdvancedBoidAgent(_mas, n, turn, new AdvancedBoidAgentArgs(_goals[1], n2, 1, 1, 1, 1, 1, 10));
                a.Speed = 0.95;
                a.Species = _mas.Species[2];
                _mas.Elements.Add(a);
            }
            //Neighbourhood lwn = new CircularNeighbourhood(fovRange, fovAng);
            //lwn.Item = fovImg;
            //LoneWandererAgent lw = new LoneWandererAgent(_mas, lwn, turn, 10, 2);
            //lw.Speed = 1;
            //lw.Species = _mas.Species[4];
            //_mas.Elements.Add(lw);
        }

        private void SetCentroidSpecies(IEnumerable<Centroid> items, Species species)
        {
            foreach (Centroid c in items)
            {
                c.Species = species;
            }
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
            if (cmbStartState.SelectedIndex == 0)
            { _mas.Scatter(); }
            else
            { FormGroup(); }
            _mas.Initialize();
            SetCentroidSpecies(_mas.Elements.Centroids, _mas.Species[3]);
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

        #endregion
    }
}
