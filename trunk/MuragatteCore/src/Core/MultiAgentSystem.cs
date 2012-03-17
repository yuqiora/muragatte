// ------------------------------------------------------------------------
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
        }

        public IStorage Elements
        {
            get { return _storage; }
        }

        public Region GetRegion
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
        
        public virtual void Clear() { }

        public virtual void NextStep() { }

        public virtual void GoToStep(int i) { }

        //possibly temporary
        public virtual void Initialize()
        {
            IEnumerable<Agent> agents = _storage.Agents;
            foreach (Agent a in agents)
            {
                a.SetMovementInfo(RandVec(false), RandVec(true));
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
                e.AfterUpdate();
            }
            _iCurrentStep = _iSteps;
            _iSteps++;
        }
        
        #endregion

        //temporary
        private Vector2 RandVec(bool normalize)
        {
            double x = RNGs.Ran2.Gauss(_region.Width / 2, _region.Width / 2);
            double y = RNGs.Ran2.Gauss(_region.Height / 2, _region.Height / 2);
            Vector2 v = new Vector2(x, y);
            if (normalize)
            {
                v.Normalize();
            }
            return v;
        }
    }
}
