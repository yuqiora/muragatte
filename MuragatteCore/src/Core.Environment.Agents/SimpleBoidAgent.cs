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
using Muragatte.Core.Environment.SteeringUtils;

namespace Muragatte.Core.Environment.Agents
{
    public class SimpleBoidAgent : Agent
    {
        #region Constructors

        public SimpleBoidAgent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, SimpleBoidAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args) { }

        public SimpleBoidAgent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, SimpleBoidAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args) { }

        protected SimpleBoidAgent(SimpleBoidAgent other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        public double SeparationWeight
        {
            get { return Separation.Weight; }
            set
            {
                Separation.Weight = value;
                _args.Modifiers[SeparationSteering.LABEL] = value;
            }
        }

        public double CohesionWeight
        {
            get { return Cohesion.Weight; }
            set
            {
                Cohesion.Weight = value;
                _args.Modifiers[CohesionSteering.LABEL] = value;
            }
        }

        public double AlignmentWeight
        {
            get { return Alignment.Weight; }
            set
            {
                Alignment.Weight = value;
                _args.Modifiers[AlignmentSteering.LABEL] = value;
            }
        }

        protected Steering Separation
        {
            get { return _steering[SeparationSteering.LABEL]; }
        }

        protected Steering Cohesion
        {
            get { return _steering[CohesionSteering.LABEL]; }
        }

        protected Steering Alignment
        {
            get { return _steering[AlignmentSteering.LABEL]; }
        }

        #endregion

        #region Methods

        protected override IEnumerable<Element> GetLocalNeighbours()
        {
            return _fieldOfView.Within(_model.Elements.RangeSearch<Agent>(this, VisibleRange));
        }

        protected override Vector2 ApplyRules(IEnumerable<Element> locals)
        {
            return Separation.Steer(locals) + Cohesion.Steer(locals) + Alignment.Steer(locals);
        }

        protected override void EnableSteering()
        {
            AddSteering(new SeparationSteering(this, _args.Modifiers[SeparationSteering.LABEL]));
            AddSteering(new CohesionSteering(this, _args.Modifiers[CohesionSteering.LABEL]));
            AddSteering(new AlignmentSteering(this, _args.Modifiers[AlignmentSteering.LABEL]));
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new SimpleBoidAgent(this, model);
        }

        #endregion
    }
}
