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

        public LoneWandererAgentArchetype(string name, int count, SpawnSpot spawnPos, Vector2 baseDir, NoisedDouble noisedDir,
            NoisedDouble speed, Species species, Neighbourhood fieldOfView, Angle turningAngle, LoneWandererAgentArgs args)
            : base(name, count, spawnPos, baseDir, noisedDir, speed, species, fieldOfView, turningAngle, args) { }

        #endregion

        #region Methods

        protected override Agent CreateOneAgent(int id, MultiAgentSystem model)
        {
            return new LoneWandererAgent(id, model, _spawnPosition.Respawn(model.Random),
                _baseDirection + new Angle(_noisedDirection.GetValue(model.Random)),
                _noisedSpeed.GetValue(model.Random), _species, _fieldOfView.Clone(),
                _turningAngle, (LoneWandererAgentArgs)_args);
        }

        #endregion
    }
}
