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
using System.Diagnostics;
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

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisAboutWindow.xaml
    /// </summary>
    public partial class ThesisAboutWindow : Window
    {
        private const string LICENSE_PATH = "license.txt";

        public ThesisAboutWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(((Hyperlink)sender).NavigateUri.ToString());
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(LICENSE_PATH))
            {
                Process.Start(LICENSE_PATH);
            }
        }
    }
}
