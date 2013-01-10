// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
//
// Copyright (C) 2012-2013  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Core.Storage;

namespace Muragatte.Visual
{
    public class LayeredSnapshot : Snapshot
    {
        #region Fields

        private byte _alpha = byte.MaxValue;
        private WriteableBitmap _wbL = null;

        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public LayeredSnapshot(int width, int height, Visualization visual)
            : base(width, height, visual)
        {
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
        }

        public LayeredSnapshot(Visualization visual)
            : this(visual.GetCanvas.UnitWidth, visual.GetCanvas.UnitHeight, visual) { }

        #endregion

        #region Properties

        public byte Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                NotifyPropertyChanged("Alpha");
            }
        }

        public BackgroundWorker Worker
        {
            get { return _worker; }
        }

        #endregion

        #region Methods

        protected override void Rescale()
        {
            _wbL = CreateBitmap();
            _wbL.Clear(_backgroundColor);
            base.Rescale();
        }

        private void CombineLayers(byte alpha)
        {
            ApplyAlpha(_wb, alpha);
            _wbL.Blit(_wb);
            _wb = CreateBitmap();
        }

        private void ApplyAlpha(WriteableBitmap wb, byte alpha)
        {
            wb.ForEach((x, y, color) => color.Equals(_backgroundColor) ? Colors.Transparent : color.WithA(alpha));
        }

        private void RemoveAlpha(WriteableBitmap wb)
        {
            wb.ForEach((x, y, color) => color.WithA(byte.MaxValue));
        }

        public void Redraw(IEnumerable<History> histories)
        {
            if (histories.Count() > 0)
            {
                Rescale();
                foreach (History h in histories)
                {
                    RedrawLayers(h, Step);
                    CombineLayers(_alpha);
                }
                ScaleBack();
                RemoveAlpha(_wbL);
                _wb = _wbL;
            }
        }

        public void Redraw(IEnumerable<History> histories, byte alpha)
        {
            Alpha = alpha;
            Redraw(histories);
        }

        public void Redraw(IEnumerable<History> histories, int step, byte alpha)
        {
            Step = step;
            Redraw(histories, alpha);
        }

        public void RedrawAsync(IEnumerable<History> histories)
        {
            if (histories.Count() > 0) _worker.RunWorkerAsync(histories);
        }

        public void RedrawAsync(IEnumerable<History> histories, byte alpha)
        {
            Alpha = alpha;
            RedrawAsync(histories);
        }

        public void RedrawAsync(IEnumerable<History> histories, int step, byte alpha)
        {
            Step = step;
            RedrawAsync(histories, alpha);
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IEnumerable<History> histories = (IEnumerable<History>)e.Argument;
            double progress = 0;
            double progressInc = 100d / histories.Count();
            Rescale();
            //_wbL.Clear(_backgroundColor);
            foreach (History h in histories)
            {
                RedrawLayers(h, Step);
                CombineLayers(_alpha);
                progress += progressInc;
                _worker.ReportProgress(0, progress);
            }
            ScaleBack();
            RemoveAlpha(_wbL);
            _wb = _wbL;
        }

        #endregion
    }
}
