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
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.IO;
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public abstract class AgentArchetype : INotifyPropertyChanged
    {
        #region Fields

        protected string _sName = null;
        protected int _iCount = 1;
        protected SpawnSpot _spawnPosition = null;
        protected NoisedDouble _noisedDirection = null;
        protected NoisedDouble _noisedSpeed = null;
        protected Species _species = null;
        protected Neighbourhood _fieldOfView = null;
        protected Angle _turningAngle = Angle.Zero;
        protected AgentArgs _args = null;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public AgentArchetype() { }

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

        [XmlAttribute]
        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("NameAndCount");
            }
        }

        [XmlAttribute]
        public int Count
        {
            get { return _iCount; }
            set
            {
                _iCount = value;
                NotifyPropertyChanged("Count");
                NotifyPropertyChanged("NameAndCount");
            }
        }

        public string NameAndCount
        {
            get { return string.Format("{0} ({1}x)", _sName, _iCount); }
        }

        [XmlElement(ElementName = "SpawnAt", Type = typeof(XmlSpawnSpotReference))]
        public SpawnSpot SpawnPosition
        {
            get { return _spawnPosition; }
            set { _spawnPosition = value; }
        }

        [XmlElement("Direction")]
        public NoisedDouble NoisedDirection
        {
            get { return _noisedDirection; }
            set { _noisedDirection = value; }
        }

        [XmlElement("Speed")]
        public NoisedDouble NoisedSpeed
        {
            get { return _noisedSpeed; }
            set { _noisedSpeed = value; }
        }

        [XmlElement(Type = typeof(XmlSpeciesReference))]
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

        [XmlIgnore]
        public abstract AgentArgs Specifics { get; set; }

        public string TypeName
        {
            get { return GetType().Name; }
        }

        #endregion

        #region Methods

        public IEnumerable<Agent> CreateAgents(int startID, MultiAgentSystem model)
        {
            if (startID == 0) startID++;
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
            return NameAndCount;
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
