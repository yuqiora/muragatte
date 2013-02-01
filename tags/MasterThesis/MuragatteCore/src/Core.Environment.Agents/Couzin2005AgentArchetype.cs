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
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class Couzin2005AgentArchetype : AgentArchetype
    {
        #region Constructors

        public Couzin2005AgentArchetype() : base() { }

        public Couzin2005AgentArchetype(string name, int count, SpawnSpot spawnPos, NoisedDouble direction,
            NoisedDouble speed, Species species, Neighbourhood fieldOfView, Angle turningAngle, FlockAndSeekBaseAgentArgs args)
            : base(name, count, spawnPos, direction, speed, species, fieldOfView, turningAngle, args) { }

        #endregion

        #region Properties

        [XmlElement(Type = typeof(FlockAndSeekBaseAgentArgs))]
        public override AgentArgs Specifics
        {
            get { return _args; }
            set { if (value is FlockAndSeekBaseAgentArgs) _args = value; }
        }

        #endregion

        #region Methods

        protected override Agent CreateOneAgent(int id, MultiAgentSystem model)
        {
            return new Couzin2005Agent(id, model, _spawnPosition.Respawn(model.Random),
                Vector2.X0Y1 + new Angle(_noisedDirection.GetValue(model.Random)),
                _noisedSpeed.GetValue(model.Random), _species, _fieldOfView.Clone(),
                _turningAngle, (FlockAndSeekBaseAgentArgs)_args.Clone(model));
        }

        #endregion
    }
}
