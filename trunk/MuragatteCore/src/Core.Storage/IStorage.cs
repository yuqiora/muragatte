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
using Muragatte.Core.Environment;

namespace Muragatte.Core.Storage
{
    public interface IStorage : ICollection<Element>
    {
        #region Properties

        Element this[int id] { get; }

        IEnumerable<Element> Items { get; }

        IEnumerable<Agent> Agents { get; }

        IEnumerable<Obstacle> Obstacles { get; }

        IEnumerable<Goal> Goals { get; }

        IEnumerable<Extras> Extras { get; }

        IEnumerable<Element> Stationary { get; }
        
        #endregion

        #region Methods
        
        void Add(IEnumerable<Element> items);

        void Remove(IEnumerable<Element> items);

        bool Remove(int id);

        Element Nearest(Element e);

        T Nearest<T>(Element e) where T : Element;

        IEnumerable<Element> RangeSearch(Element e, double range);

        IEnumerable<T> RangeSearch<T>(Element e, double range) where T : Element;

        IEnumerable<Element> RangeSearch(Element e, double range, Predicate<Element> match);

        IEnumerable<T> RangeSearch<T>(Element e, double range, Predicate<Element> match) where T : Element;
        
        #endregion
    }
}
