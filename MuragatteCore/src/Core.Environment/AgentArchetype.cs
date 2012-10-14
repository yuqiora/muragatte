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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Common;
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public abstract class AgentArchetype : INotifyPropertyChanged
    {
        #region Fields

        protected string _sName;
        protected int _iCount;
        protected SpawnSpot _spawnPosition;
        protected NoisedDouble _noisedDirection;
        protected NoisedDouble _noisedSpeed;
        protected Species _species;
        protected Neighbourhood _fieldOfView;
        protected Angle _turningAngle;
        protected AgentArgs _args;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public AgentArchetype(string name, int count, SpawnSpot spawnPos, NoisedDouble direction,
            NoisedDouble speed, Species species, Neighbourhood fieldOfView, Angle turningAngle, AgentArgs args)
        {
            _sName = name;
            _iCount = count;
            _spawnPosition = spawnPos;
            _noisedDirection = direction;
            _noisedSpeed = speed;
            _species = species;
            _fieldOfView = fieldOfView;
            _turningAngle = turningAngle;
            _args = args;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int Count
        {
            get { return _iCount; }
            set
            {
                _iCount = value;
                NotifyPropertyChanged("Count");
            }
        }

        public SpawnSpot SpawnPosition
        {
            get { return _spawnPosition; }
            set { _spawnPosition = value; }
        }

        public NoisedDouble NoisedDirection
        {
            get { return _noisedDirection; }
            set { _noisedDirection = value; }
        }

        public NoisedDouble NoisedSpeed
        {
            get { return _noisedSpeed; }
            set { _noisedSpeed = value; }
        }

        public Species Species
        {
            get { return _species; }
            set { _species = value; }
        }

        public Neighbourhood FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; }
        }

        public Angle TurningAngle
        {
            get { return _turningAngle; }
            set { _turningAngle = value; }
        }

        public AgentArgs Specifics
        {
            get { return _args; }
            set { _args = value; }
        }

        #endregion

        #region Methods

        public IEnumerable<Agent> CreateAgents(int startID, MultiAgentSystem model)
        {
            List<Agent> agents = new List<Agent>();
            int endID = startID + _iCount;
            for (int i = startID; i < endID; i++)
            {
                agents.Add(CreateOneAgent(i, model));
            }
            return agents;
        }

        protected abstract Agent CreateOneAgent(int id, MultiAgentSystem model);

        public override string ToString()
        {
            return string.Format("{0} ({1}x)", _sName, _iCount);
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
