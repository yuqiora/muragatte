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
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class VersatileAgentArchetype : AgentArchetype
    {
        #region Constructors

        public VersatileAgentArchetype() : base() { }

        public VersatileAgentArchetype(string name, int count, SpawnSpot spawnPos, NoisedDouble direction,
            NoisedDouble speed, Species species, Neighbourhood fieldOfView, Angle turningAngle, VersatileAgentArgs args)
            : base(name, count, spawnPos, direction, speed, species, fieldOfView, turningAngle, args) { }

        #endregion

        #region Properties

        [XmlElement(Type = typeof(VersatileAgentArgs))]
        public override AgentArgs Specifics
        {
            get { return _args; }
            set { if (value is VersatileAgentArgs) _args = value; }
        }

        #endregion

        #region Methods

        protected override Agent CreateOneAgent(int id, MultiAgentSystem model)
        {
            return new VersatileAgent(id, model, _spawnPosition.Respawn(model.Random),
                Vector2.X0Y1 + new Angle(_noisedDirection.GetValue(model.Random)),
                _noisedSpeed.GetValue(model.Random), _species, _fieldOfView.Clone(),
                _turningAngle, (VersatileAgentArgs)_args.Clone(model));
        }

        #endregion
    }
}
