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
using System.Reflection;
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
        #region Constants

        private const string LICENSE_PATH = "license.txt";

        #endregion

        #region Constructors

        public ThesisAboutWindow()
        {
            InitializeComponent();
            DataContext = this;

            txbMuragatteThesisVersion.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString();
            txbMuragatteCoreVersion.Text += Assembly.ReflectionOnlyLoadFrom(@".\MuragatteCore.dll").GetName().Version.ToString();
            txbMuragatteVisualVersion.Text += Assembly.ReflectionOnlyLoadFrom(@".\MuragatteVisual.dll").GetName().Version.ToString();
            txbRandomOpsVersion.Text += Assembly.ReflectionOnlyLoadFrom(@".\RandomOps.dll").GetName().Version.ToString();
            txbExtendedWPFToolkitVersion.Text += Assembly.ReflectionOnlyLoadFrom(@".\WPFToolkit.Extended.dll").GetName().Version.ToString();
            txbWriteableBitmapExVersion.Text += Assembly.ReflectionOnlyLoadFrom(@".\WriteableBitmapEx.Wpf.dll").GetName().Version.ToString();
        }

        #endregion

        #region Events

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

        #endregion
    }
}
