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
    public class ClassicBoidAgent : SimpleBoidAgent
    {
        #region Constructors

        public ClassicBoidAgent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, ClassicBoidAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        public ClassicBoidAgent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, ClassicBoidAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        protected ClassicBoidAgent(ClassicBoidAgent other, MultiAgentSystem model)
            : base(other, model)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        #endregion

        #region Properties

        public Neighbourhood SeparationArea
        {
            get { return _args.Neighbourhoods[ClassicBoidAgentArgs.NEIGH_SEPARATION_AREA]; }
        }

        public Neighbourhood CohesionArea
        {
            get { return _args.Neighbourhoods[ClassicBoidAgentArgs.NEIGH_COHESION_AREA]; }
        }

        public Neighbourhood AlignmentArea
        {
            get { return _args.Neighbourhoods[ClassicBoidAgentArgs.NEIGH_ALIGNMENT_AREA]; }
        }

        #endregion

        #region Methods

        protected override IEnumerable<Element> GetLocalNeighbours()
        {
            return _fieldOfView.Within(_model.Elements.RangeSearch(this, VisibleRange));
        }

        protected override Vector2 ApplyRules(IEnumerable<Element> locals)
        {
            IEnumerable<Element> others = locals.Where(e => RelationshipWith(e) == ElementNature.Companion);
            return Separation.Steer(SeparationArea.Within(others), true)
                + Cohesion.Steer(CohesionArea.Within(others), true)
                + Alignment.Steer(AlignmentArea.Within(others), true);
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new ClassicBoidAgent(this, model);
        }

        #endregion
    }
}
