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
    public partial class SandboxMainWindow : Window, INotifyPropertyChanged
    {
        #region Constants

        private const double TIME_PER_STEP = 0.25;
        private const int DEFAULT_DELAY = 15;

        #endregion

        #region Fields

        private MultiAgentSystem _mas = null;
        private Visual.Canvas _canvas = null;
        private VisualCanvasWindow _view = null;
        private bool _bPlaying = false;
        private List<Goal> _goals = new List<Goal>();
        private List<Obstacle> _obstacles = new List<Obstacle>();
        private Duration replayDuration = new Duration();

        private Color _colorNeighbourhood = Colors.LightYellow;
        private Color _colorObstacle = Colors.Gray;
        private Color _colorAgentDefault = Colors.Black;
        private Color _colorAgentGuide = Colors.Blue;
        private Color _colorAgentIntruder = Colors.Red;
        private Color _colorCentroid = Colors.LightGreen;

        private Angle _boidFOVAngle = new Angle(150);

        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Properties

        public Duration ReplayDuration
        {
            get { return replayDuration; }
            set
            {
                replayDuration = value;
                this.OnPropertyChanged("ReplayDuration");
            }
        }

        #endregion

        public SandboxMainWindow()
        {
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            InitializeComponent();
            this.DataContext = this;
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            prbUpdate.Visibility = System.Windows.Visibility.Hidden;
            UpdateReplayDuration();
            Redraw();
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prbUpdate.Value = e.ProgressPercentage;
            txtSteps.Text = _mas.NumberOfSteps.ToString();
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }
        
        #region Button Events
        
        private void btnEnvironment_Click(object sender, RoutedEventArgs e)
        {
            CreateObstacles();
            CreateGoals();
            _canvas.Redraw();
        }

        private void btnAgents_Click(object sender, RoutedEventArgs e)
        {
            double fovRange = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            int agentCount = int.Parse(txtAgentCount.Text);
            CreateSpecies();
            Color fovC = _colorNeighbourhood;
            fovC.A = 64;
            //Particle fovImg = ParticleFactory.Neighbourhood((int)(fovRange * 2 * _canvas.Scale), (int)(fovRange * 2 * _canvas.Scale), fovC, _boidFOVAngle);
            Particle fovImg = ParticleFactory.Ellipse((int)(fovRange * 2 * _canvas.Scale), fovC);
            CreateBoids(agentCount, fovRange, fovImg);
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
            CreateSpecies();
            Color fovC = _colorNeighbourhood;
            fovC.A = 64;
            Particle fovImg = ParticleFactory.Ellipse((int)(fovRange * 2 * _canvas.Scale), fovC);
            CreateAdvancedBoids(naiveCount, guideCount, intruderCount, fovRange, fovImg, paRange);
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
            _canvas.Clear();
            txtSteps.Text = "0";
            sldSteps.Value = 0;
        }

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            _goals.Clear();
            _obstacles.Clear();
            txtSteps.Text = "0";
            sldSteps.Value = 0;
            int width = int.Parse(txtWidth.Text);
            int height = int.Parse(txtHeight.Text);
            _mas = new MultiAgentSystem(new SimpleBruteForce(), new Region(
                width, height, chbHorizontal.IsChecked.Value, chbVertical.IsChecked.Value), TIME_PER_STEP);
            double scale = double.Parse(txtScale.Text);
            _canvas = new Visual.Canvas(width, height, scale);
            _canvas.Initialize(_mas);
            if (_view != null)
            {
                _view.Close();
            }
            _view = new VisualCanvasWindow(_canvas);
            _view.Show();
            UpdateReplayDuration();
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_bPlaying)
            {
                _bPlaying = false;
                btnPlayPause.Content = "Play";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                UpdateReplayDuration();
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

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            //if (_mas.NumberOfSteps > 0 && !_bPlaying && !_worker.IsBusy)
            //{
            //    prbReplay.Value = 0;
            //    prbReplay.Visibility = System.Windows.Visibility.Visible;
            //    _worker.RunWorkerAsync(_mas.NumberOfSteps);
            //    //_canvas.Redraw(_mas.History);
            //}
        }

        #endregion

        #region Checkbox Events

        private void chbAgents_Checked(object sender, RoutedEventArgs e)
        {
            SetShowAgents();
        }

        private void chbAgents_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShowAgents();
        }

        private void chbNeighbourhoods_Checked(object sender, RoutedEventArgs e)
        {
            SetShowNeighbourhoods();
        }

        private void chbNeighbourhoods_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShowNeighbourhoods();
        }

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

        private void chbEnvironment_Checked(object sender, RoutedEventArgs e)
        {
            SetShowEnvironment();
        }

        private void chbEnvironment_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShowEnvironment();
        }

        private void chbTracks_Checked(object sender, RoutedEventArgs e)
        {
            SetShowTracks();
        }

        private void chbTracks_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShowTracks();
        }

        private void chbTrails_Checked(object sender, RoutedEventArgs e)
        {
            SetShowTrails();
        }

        private void chbTrails_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShowTrails();
        }

        private void chbCentroids_Checked(object sender, RoutedEventArgs e)
        {
            SetShowCentroids();
        }

        private void chbCentroids_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShowCentroids();
        }

        #endregion

        #region Other Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_view != null)
            {
                _view.Close();
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateAndRedraw();
        }

        private void sldSteps_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int step = (int)sldSteps.Value;
            if (_mas.NumberOfSteps > 0 && !_bPlaying)
            {
                //int fullStep = (int)Math.Ceiling(1 / _mas.TIME_PER_STEP);
                //if ((int)sldSteps.Value % fullStep == 0)
                //{
                //    _canvas.Redraw(_mas.History, (int)sldSteps.Value);
                //}
                //replayAnimation.Storyboard.Seek(TimeSpan.FromMilliseconds(step * int.Parse(txtDelay.Text)), System.Windows.Media.Animation.TimeSeekOrigin.BeginTime);
                _canvas.Redraw(_mas.History, step);
            }
        }

        private void txtDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateReplayDuration();
        }

        #endregion

        #region Methods

        private void UpdateAndRedraw()
        {
            _mas.Update();
            txtSteps.Text = _mas.NumberOfSteps.ToString();
            Redraw();
        }

        private void UpdateReplayDuration()
        {
            if (_mas != null)
            {
                int ms;
                if (!int.TryParse(txtDelay.Text, out ms))
                {
                    ms = DEFAULT_DELAY;
                }
                ReplayDuration = new Duration(TimeSpan.FromMilliseconds(ms * _mas.NumberOfSteps));
            }
        }

        private void SetShowAgents()
        {
            if (_canvas != null)
            {
                _canvas.IsAgentsEnabled = chbAgents.IsChecked.Value;
            }
        }

        private void SetShowNeighbourhoods()
        {
            if (_canvas != null)
            {
                _canvas.IsNeighbourhoodsEnabled = chbNeighbourhoods.IsChecked.Value;
            }
        }

        private void CreateGoals()
        {
            Goal g1 = new PositionGoal(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height));
            g1.Item = ParticleFactory.Ellipse((int)(g1.Width * _canvas.Scale), _colorAgentGuide, true);
            Goal g2 = new PositionGoal(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height));
            g2.Item = ParticleFactory.Ellipse((int)(g2.Width * _canvas.Scale), _colorAgentIntruder, true);
            _goals.Add(g1);
            _goals.Add(g2);
            _mas.Elements.Add(_goals);
        }

        private void CreateObstacles()
        {
            int obstacles = int.Parse(txtObstacles.Text);
            for (int i = 0; i < obstacles; i++)
            {
                Obstacle o = new EllipseObstacle(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height), RNGs.Uniform(10, 30));
                o.Item = ParticleFactory.Ellipse((int)(o.Radius * _canvas.Scale), _colorObstacle);
                _obstacles.Add(o);
            }
            _mas.Elements.Add(_obstacles);
        }
        
        private void CreateSpecies()
        {
            SortedDictionary<int, Species> species = new SortedDictionary<int, Species>();
            Species boids = new Species("Boids");
            boids.Item = ParticleFactory.AgentB((int)_canvas.Scale, _colorAgentDefault);
            species.Add(boids.ID, boids);
            Species guides = boids.CreateSubSpecies("Guides");
            guides.Item = ParticleFactory.AgentB((int)_canvas.Scale, _colorAgentGuide);
            species.Add(guides.ID, guides);
            Species intruders = boids.CreateSubSpecies("Intruders");
            intruders.Item = ParticleFactory.AgentB((int)_canvas.Scale, _colorAgentIntruder);
            species.Add(intruders.ID, intruders);
            Species centroids = new Species("Centroids");
            centroids.Item = ParticleFactory.AgentB((int)_canvas.Scale, _colorCentroid);
            species.Add(centroids.ID, centroids);
            Species wanderers = new Species("Wanderers");
            wanderers.Item = ParticleFactory.AgentA((int)_canvas.Scale, _colorAgentDefault);
            species.Add(wanderers.ID, wanderers);
            Species versatiles = new Species("Versatiles");
            versatiles.Item = ParticleFactory.AgentA((int)_canvas.Scale, _colorAgentGuide);
            species.Add(versatiles.ID, versatiles);
            _mas.Species = species;
        }

        private void CreateBoids(int count, double fovRange, Particle fovImg)
        {
            for (int i = 0; i < count; i++)
            {
                Neighbourhood n = new CircularNeighbourhood(fovRange, _boidFOVAngle);
                n.Item = fovImg;
                Agent a = new Boid(_mas, n, new Angle(60));
                a.Speed = 1;
                a.Species = _mas.Species[0];
                _mas.Elements.Add(a);
            }
        }

        private void CreateAdvancedBoids(int naive, int guides, int intruders, double fovRange, Particle fovImg, double paRange)
        {
            Angle turn = new Angle(60);
            Angle fovAng = new Angle(150);
            for (int i = 0; i < naive; i++)
            {
                Neighbourhood n = new CircularNeighbourhood(fovRange, fovAng);
                n.Item = fovImg;
                Neighbourhood n2 = new CircularNeighbourhood(paRange);
                Agent a = new AdvancedBoid(_mas, n, turn, null, 0, n2);
                a.Speed = 1.05;
                a.Species = _mas.Species[0];
                _mas.Elements.Add(a);
            }
            for (int i = 0; i < guides; i++)
            {
                Neighbourhood n = new CircularNeighbourhood(fovRange, fovAng);
                n.Item = fovImg;
                Neighbourhood n2 = new CircularNeighbourhood(paRange);
                Agent a = new AdvancedBoid(_mas, n, turn, _goals[0], 0.75, n2);
                a.Speed = 1;
                a.Species = _mas.Species[1];
                _mas.Elements.Add(a);
            }
            for (int i = 0; i < intruders; i++)
            {
                Neighbourhood n = new CircularNeighbourhood(fovRange, fovAng);
                n.Item = fovImg;
                Neighbourhood n2 = new CircularNeighbourhood(paRange);
                Agent a = new AdvancedBoid(_mas, n, turn, _goals[1], 1, n2);
                a.Speed = 0.95;
                a.Species = _mas.Species[2];
                _mas.Elements.Add(a);
            }
            //Neighbourhood lwn = new CircularNeighbourhood(fovRange, fovAng);
            //lwn.Item = fovImg;
            //LoneWanderer lw = new LoneWanderer(_mas, lwn, turn, 10, 2);
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

        private void SetShowEnvironment()
        {
            if (_canvas != null)
            {
                _canvas.IsEnvironmentEnabled = chbEnvironment.IsChecked.Value;
            }
        }

        private void SetShowTracks()
        {
            if (_canvas != null)
            {
                _canvas.IsTracksEnabled = chbTracks.IsChecked.Value;
            }
        }

        private void SetShowTrails()
        {
            if (_canvas != null)
            {
                _canvas.IsTrailsEnabled = chbTrails.IsChecked.Value;
            }
        }

        private void SetShowCentroids()
        {
            if (_canvas != null)
            {
                _canvas.IsCentroidsEnabled = chbCentroids.IsChecked.Value;
            }
        }

        private void Redraw()
        {
            if (chbVisualize.IsChecked.Value)
            {
                //_canvas.Redraw();
                _canvas.Redraw(_mas.History);
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
