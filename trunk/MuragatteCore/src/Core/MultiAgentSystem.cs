﻿// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Core Library
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
using Muragatte.Common;
using Muragatte.Core.Storage;
using Muragatte.Core.Environment;

namespace Muragatte.Core
{
    public class MultiAgentSystem
    {
        #region Fields

        protected int _iCurrentStep = 0;
        protected int _iSteps = 0;
        protected double _dTimePerStep = 1;
        protected IStorage _storage = null;
        protected Region _region = null;
        protected SortedDictionary<int, Species> _species = null;
        //history

        #endregion

        #region Constructors

        public MultiAgentSystem(IStorage storage, Region region, double timePerStep = 1)
        {
            _storage = storage;
            _region = region;
            _dTimePerStep = timePerStep;
            _species = new SortedDictionary<int, Species>();
        }

        #endregion

        #region Properties
        
        public int CurrentStep
        {
            get { return _iCurrentStep; }
        }

        public int NumberOfSteps
        {
            get { return _iSteps; }
        }

        public double TimePerStep
        {
            get { return _dTimePerStep; }
            set { _dTimePerStep = value; }
        }

        public IStorage Elements
        {
            get { return _storage; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public SortedDictionary<int, Species> Species
        {
            get { return _species; }
            set { _species = value; }
        }
        
        #endregion

        #region Virtual Methods
        
        public virtual void Clear()
        {
            _iCurrentStep = 0;
            _iSteps = 0;
            _species.Clear();
            Environment.Species.ResetIDCounter();
            _storage.Clear();
            Element.ResetIDCounter();
        }

        public virtual void NextStep() { }

        public virtual void GoToStep(int i) { }

        //possibly temporary
        public virtual void Initialize()
        {
            Scatter();
        }

        public virtual void Scatter()
        {
            IEnumerable<Agent> agents = _storage.Agents;
            foreach (Agent a in agents)
            {
                a.SetMovementInfo(
                    Vector2.RandomUniform(_region.Width, _region.Height),
                    Vector2.RandomNormalized());
            }
        }

        public virtual void GroupStart(double size)
        {
            Vector2 centre = Vector2.RandomUniform(_region.Width, _region.Height);
            IEnumerable<Agent> agents = _storage.Agents;
            Vector2 direction = Vector2.RandomNormalized();
            foreach (Agent a in agents)
            {
                double x;
                double y;
                double ss;
                RNGs.Ran2.Disk(out x, out y, out ss);
                Vector2 pos = new Vector2(x, y);
                //Vector2 pos = Vector2.RandomGauss();
                pos *= size;
                pos += new Vector2(_region.Width / 2, Region.Height / 2);
                a.SetMovementInfo(pos, direction + Angle.Random(5));
            }
        }

        public virtual void Update()
        {
            foreach (Element e in _storage)
            {
                e.Update();
            }
            foreach (Element e in _storage)
            {
                e.ConfirmUpdate();
            }
            _iCurrentStep = _iSteps;
            _iSteps++;
        }
        
        #endregion
    }
}
