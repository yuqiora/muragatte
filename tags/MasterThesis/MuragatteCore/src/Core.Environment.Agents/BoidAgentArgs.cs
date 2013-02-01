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
using Muragatte.Core.Environment.SteeringUtils;
using Muragatte.IO;
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class ClassicBoidAgentArgs : SimpleBoidAgentArgs
    {
        #region Constants

        public const string NEIGH_SEPARATION_AREA = "Separation Area";
        public const string NEIGH_COHESION_AREA = "Cohesion Area";
        public const string NEIGH_ALIGNMENT_AREA = "Alignment Area";

        #endregion

        #region Fields

        protected Dictionary<string, Neighbourhood> _neighbourhoods = new Dictionary<string, Neighbourhood>();

        #endregion

        #region Constructors

        public ClassicBoidAgentArgs() : this(new Neighbourhood()) { }

        public ClassicBoidAgentArgs(Neighbourhood fieldOfView)
            : this(fieldOfView, fieldOfView, fieldOfView, 1, 1, 1, Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public ClassicBoidAgentArgs(Neighbourhood separationArea, Neighbourhood cohesionArea, Neighbourhood alignmentArea)
            : this(separationArea, cohesionArea, alignmentArea, 1, 1, 1, Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public ClassicBoidAgentArgs(Neighbourhood separationArea, Neighbourhood cohesionArea, Neighbourhood alignmentArea,
            double separation, double cohesion, double alignment, Distribution distribution, double noiseA, double noiseB)
            : base(separation, cohesion, alignment, distribution, noiseA, noiseB)
        {
            _neighbourhoods.Add(NEIGH_SEPARATION_AREA, separationArea);
            _neighbourhoods.Add(NEIGH_COHESION_AREA, cohesionArea);
            _neighbourhoods.Add(NEIGH_ALIGNMENT_AREA, alignmentArea);
        }

        protected ClassicBoidAgentArgs(ClassicBoidAgentArgs args)
            : base(args)
        {
            _neighbourhoods = GetNeigbourhoodClones(args._neighbourhoods);
        }

        #endregion

        #region Properties

        public override bool HasNeighbourhoods
        {
            get { return true; }
        }

        [XmlIgnore]
        public override Dictionary<string, Neighbourhood> Neighbourhoods
        {
            get { return _neighbourhoods; }
        }

        #endregion

        #region Methods

        public override AgentArgs Clone(MultiAgentSystem model)
        {
            return new ClassicBoidAgentArgs(this);
        }

        #endregion
    }
}
