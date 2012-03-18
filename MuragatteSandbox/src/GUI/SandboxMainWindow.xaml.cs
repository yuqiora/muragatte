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
        private MultiAgentSystem _mas = null;
        private Visual.Canvas _canvas = null;
        private VisualCanvasWindow _view = null;
        private bool _bPlaying = false;

        public SandboxMainWindow()
        {
            InitializeComponent();
        }

        private void btnEnvironment_Click(object sender, RoutedEventArgs e)
        {
            int width = int.Parse(txtWidth.Text);
            int height = int.Parse(txtHeight.Text);
            _mas = new MultiAgentSystem(new SimpleBruteForce(), new Region(
                width, height, chbHorizontal.IsChecked.Value, chbVertical.IsChecked.Value));
            double scale = double.Parse(txtScale.Text);
            _canvas = new Visual.Canvas(width, height, scale);
            if (_view != null)
            {
                _view.Close();
            }
            _view = new VisualCanvasWindow(_canvas);
            _view.Show();
        }

        private void btnAgents_Click(object sender, RoutedEventArgs e)
        {
            double fovRange = double.Parse(txtFieldOfView.Text, System.Globalization.NumberFormatInfo.InvariantInfo);
            int agentCount = int.Parse(txtAgentCount.Text);
            SortedDictionary<int, Species> species = new SortedDictionary<int, Species>();
            Species b = new Species("Boids");
            b.Item = Particle.Agent((int)_canvas.Scale, Colors.Blue);
            species.Add(b.ID, b);
            _mas.Species = species;
            Color fovC = Colors.LightGreen;
            fovC.A = 64;
            Particle fovImg = Particle.Ellipse((int)(fovRange * 2 * _canvas.Scale), fovC, true);
            for (int i = 0; i < agentCount; i++)
            {
                Neighbourhood n = new CircularNeighbourhood(fovRange);
                n.Item = fovImg;
                Agent a = new Boid(_mas, n, 180);
                a.Speed = 1;
                a.Species = b;
                _mas.Elements.Add(a);
            }
            _mas.Initialize();
            _canvas.Initialize(_mas);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateAndRedraw();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _mas.Clear();
            _canvas.Clear();
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
                _canvas.IsNeighbourhoodsEnabled = chbAgents.IsChecked.Value;
            }
        }
    }
}
