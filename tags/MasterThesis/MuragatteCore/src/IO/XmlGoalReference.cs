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
using Muragatte.Core.Environment;

namespace Muragatte.IO
{
    public class XmlGoalReference
    {
        #region Fields

        [XmlIgnore]
        public static IEnumerable<Goal> KnownGoals = null;

        [XmlText]
        public string Id = null;

        #endregion

        #region Constructors

        public XmlGoalReference() { }

        public XmlGoalReference(Goal g)
        {
            if (g != null) Id = g.ID.ToString();
        }

        #endregion

        #region Methods

        public Goal ToGoal()
        {
            return Id == null || KnownGoals == null ? null : KnownGoals.FirstOrDefault(g => g.ID == int.Parse(Id));
        }

        #endregion

        #region Operators

        public static implicit operator Goal(XmlGoalReference x)
        {
            return x.ToGoal();
        }

        public static implicit operator XmlGoalReference(Goal g)
        {
            return new XmlGoalReference(g);
        }

        #endregion
    }
}
