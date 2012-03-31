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
        private const double TimePerStep = 0.5;

        private MultiAgentSystem _mas = null;
        private Visual.Canvas _canvas = null;
        private VisualCanvasWindow _view = null;
        private bool _bPlaying = false;
        private List<Goal> _goals = new List<Goal>();

        public SandboxMainWindow()
        {
            InitializeComponent();
        }

        private void btnEnvironment_Click(object sender, RoutedEventArgs e)
        {
            CreateGoals();
            _canvas.Redraw();
        }

        private void btnAgents_Click(object sender, RoutedEventArgs e)
        {
            double fovRange = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            int agentCount = int.Parse(txtAgentCount.Text);
            CreateSpecies();
            Color fovC = Colors.LightGreen;
            fovC.A = 64;
            Particle fovImg = ParticleFactory.Ellipse((int)(fovRange * 2 * _canvas.Scale), fovC, true);
            CreateBoids(agentCount, fovRange, fovImg);
            _mas.Initialize();
            _canvas.Redraw();
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
            Color fovC = Colors.LightGreen;
            fovC.A = 64;
            Particle fovImg = ParticleFactory.Ellipse((int)(fovRange * 2 * _canvas.Scale), fovC, true);
            CreateAdvancedBoids(naiveCount, guideCount, intruderCount, fovRange, fovImg, paRange);
            _mas.Initialize();
            _canvas.Redraw();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateAndRedraw();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _mas.Clear();
            _goals.Clear();
            _canvas.Clear();
            txtSteps.Text = _mas.NumberOfSteps.ToString();
        }

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            int width = int.Parse(txtWidth.Text);
            int height = int.Parse(txtHeight.Text);
            _mas = new MultiAgentSystem(new SimpleBruteForce(), new Region(
                width, height, chbHorizontal.IsChecked.Value, chbVertical.IsChecked.Value), TimePerStep);
            double scale = double.Parse(txtScale.Text);
            _canvas = new Visual.Canvas(width, height, scale);
            _canvas.Initialize(_mas);
            if (_view != null)
            {
                _view.Close();
            }
            _view = new VisualCanvasWindow(_canvas);
            _view.Show();
            txtSteps.Text = _mas.NumberOfSteps.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_view != null)
            {
                _view.Close();
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_bPlaying) {
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

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateAndRedraw();
        }

        private void UpdateAndRedraw()
        {
            _mas.Update();
            txtSteps.Text = _mas.NumberOfSteps.ToString();
            _canvas.Redraw();
        }

        private void btnScatter_Click(object sender, RoutedEventArgs e)
        {
            _mas.Scatter();
        }

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
            g1.Item = ParticleFactory.Ellipse((int)(g1.Width * _canvas.Scale), Colors.Blue, false);
            Goal g2 = new PositionGoal(_mas, Vector2.RandomUniform(_mas.Region.Width, _mas.Region.Height));
            g2.Item = ParticleFactory.Ellipse((int)(g2.Width * _canvas.Scale), Colors.Red, false);
            _goals.Add(g1);
            _goals.Add(g2);
            _mas.Elements.Add(_goals);
        }
        
        private void CreateSpecies()
        {
            SortedDictionary<int, Species> species = new SortedDictionary<int, Species>();
            Species boids = new Species("Boids");
            boids.Item = ParticleFactory.Agent((int)_canvas.Scale, Colors.Black);
            species.Add(boids.ID, boids);
            Species guides = boids.CreateSubSpecies("Guides");
            guides.Item = ParticleFactory.Agent((int)_canvas.Scale, Colors.Blue);
            species.Add(guides.ID, guides);
            Species intruders = boids.CreateSubSpecies("Intruders");
            intruders.Item = ParticleFactory.Agent((int)_canvas.Scale, Colors.Red);
            species.Add(intruders.ID, intruders);
            _mas.Species = species;
        }

        private void CreateBoids(int count, double fovRange, Particle fovImg)
        {
            for (int i = 0; i < count; i++)
            {
                Neighbourhood n = new CircularNeighbourhood(fovRange, new Angle(150));
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
        }

        private void btnGroup_Click(object sender, RoutedEventArgs e)
        {
            double fov = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            _mas.GroupStart(fov * 3);
        }
    }
}
