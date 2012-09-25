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

namespace Muragatte.Core.Environment.SteeringUtils
{
    public class ObstacleAvoidanceSteering : ToNeighboursSteering
    {
        #region Constants

        public const string LABEL = "Obstacle Avoidance";

        #endregion

        #region Fields

        protected double _dRange;

        #endregion

        #region Constructors

        public ObstacleAvoidanceSteering(Element element, double weight, double range)
            : base(element, weight)
        {
            _dRange = range;
        }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LABEL; }
        }

        public double Range
        {
            get { return _dRange; }
            set { _dRange = value; }
        }

        #endregion

        #region Methods

        protected override Vector2 SteerToOthers(IEnumerable<Element> others, double weight)
        {
            int ytox = 0;
            Vector2 lineOfSight = _dRange * _element.Direction;
            double nearest = lineOfSight.Length;
            Vector2 nearestPos = Vector2.Zero;
            Vector2 r1 = _element.Position + Vector2.Perpendicular(_element.Direction * _element.Radius);
            Vector2 r2 = r1 + lineOfSight;
            Vector2 l1 = _element.Position - Vector2.Perpendicular(_element.Direction * _element.Radius);
            Vector2 l2 = l1 + lineOfSight;
            foreach (Element e in others)
            {
                if (Vector2.Distance(_element.Position, e.GetPosition()) > e.Radius + _dRange)
                {
                    continue;
                }
                Vector2 ip;
                if (e.IntersectsWith(r1, r2, out ip))
                {
                    double dist = Vector2.Distance(r1, ip);
                    if (dist < nearest)
                    {
                        nearest = dist;
                        nearestPos = e.GetPosition();
                        ytox = 1;
                    }
                }
                if (e.IntersectsWith(l1, l2, out ip))
                {
                    double dist = Vector2.Distance(l1, ip);
                    if (dist < nearest)
                    {
                        nearest = dist;
                        nearestPos = e.GetPosition();
                        ytox = -1;
                    }
                }
            }
            return nearest < lineOfSight.Length ? weight * ytox * Vector2.Perpendicular(_element.Position - nearestPos) : Vector2.Zero;
        }

        #endregion
    }
}
