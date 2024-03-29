﻿// ------------------------------------------------------------------------
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

namespace Muragatte.Visual.GUI
{
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window
    {
        #region Fields

        private Visualization _visualization;

        #endregion

        #region Constructors

        public CanvasWindow(Visualization visualization, Canvas canvas)
        {
            InitializeComponent();
            _visualization = visualization;
            Initialize(canvas);
        }

        #endregion

        #region Methods

        public void Initialize(Canvas canvas)
        {
            if (canvas != null)
            {
                imgCanvas.Width = canvas.PixelWidth;
                imgCanvas.Height = canvas.PixelHeight;
                imgCanvas.Source = canvas.Image;
            }
        }

        #endregion

    }
}
