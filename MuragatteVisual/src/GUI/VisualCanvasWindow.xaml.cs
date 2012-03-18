// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using Muragatte.Visual;

namespace Muragatte.GUI
{
    /// <summary>
    /// Interaction logic for VisualCanvasWindow.xaml
    /// </summary>
    public partial class VisualCanvasWindow : Window
    {
        private Visual.Canvas _canvas = null;

        public VisualCanvasWindow()
        {
            InitializeComponent();
        }

        public VisualCanvasWindow(Visual.Canvas canvas)
        {
            InitializeComponent();
            SetCanvas(canvas);
        }

        public void SetCanvas(Visual.Canvas canvas)
        {
            _canvas = canvas;
            imgCanvas.Width = canvas.PixelWidth;
            imgCanvas.Height = canvas.PixelHeight;
            imgCanvas.Source = _canvas.Image;
        }
    }
}
