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
using Muragatte.Core;

namespace Muragatte.Visual
{
    public class Visualization
    {
        #region Fields

        private MultiAgentSystem _model;
        private Canvas _canvas;
        private GUI.VisualCanvasWindow _wndCanvas;
        private GUI.VisualPlaybackWindow _wndPlayback;
        private GUI.VisualOptionsWindow _wndOptions;

        #endregion

        #region Constructors

        public Visualization(MultiAgentSystem model, int width, int height, double scale, System.Windows.Window owner)
        {
            DefaultValues.Scale = scale;
            _model = model;
            _canvas = new Canvas(width, height, scale, this);
            _wndCanvas = new GUI.VisualCanvasWindow(this, _canvas);
            _wndCanvas.Owner = owner;
            _wndPlayback = new GUI.VisualPlaybackWindow(this);
            _wndPlayback.Owner = owner;
            _wndOptions = new GUI.VisualOptionsWindow(this);
            _wndOptions.Owner = owner;
        }

        #endregion

        #region Properties

        public MultiAgentSystem GetModel
        {
            get { return _model; }
        }

        public Canvas GetCanvas
        {
            get { return _canvas; }
        }

        public GUI.VisualCanvasWindow GetWindow
        {
            get { return _wndCanvas; }
        }

        public GUI.VisualPlaybackWindow GetPlayback
        {
            get { return _wndPlayback; }
        }

        public GUI.VisualOptionsWindow GetOptions
        {
            get { return _wndOptions; }
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            _canvas.Clear();
            _wndCanvas.Show();
            _wndPlayback.Show();
            _wndOptions.Show();
        }

        public void Redraw(int frame)
        {
            _canvas.Redraw(_model.History, frame);
        }

        public void Close()
        {
            CloseWindow(_wndCanvas);
            CloseWindow(_wndPlayback);
            CloseWindow(_wndOptions);
        }

        private void CloseWindow(System.Windows.Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion
    }
}
