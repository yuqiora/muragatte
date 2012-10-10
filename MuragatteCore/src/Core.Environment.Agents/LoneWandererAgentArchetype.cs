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
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class LoneWandererAgentArchetype : AgentArchetype
    {
        #region Constructors

        public LoneWandererAgentArchetype(string name, int count, SpawnSpot spawnPos, NoisedDouble direction,
            NoisedDouble speed, Species species, Neighbourhood fieldOfView, Angle turningAngle, LoneWandererAgentArgs args)
            : base(name, count, spawnPos, direction, speed, species, fieldOfView, turningAngle, args) { }

        #endregion

        #region Methods

        protected override Agent CreateOneAgent(int id, MultiAgentSystem model)
        {
            return new LoneWandererAgent(id, model, _spawnPosition.Respawn(model.Random),
                Vector2.X0Y1 + new Angle(_noisedDirection.GetValue(model.Random)),
                _noisedSpeed.GetValue(model.Random), _species, _fieldOfView.Clone(),
                _turningAngle, (LoneWandererAgentArgs)_args.Clone());
        }

        #endregion
    }
}
