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

namespace Muragatte.Core.Storage
{
    public enum StorageOptions
    {
        SimpleBruteForce,
        OrthantNeighbourhoodGraph
    }

    public static class StorageOptionsExtensions
    {
        public static IStorage ToStorage(this StorageOptions x)
        {
            switch (x)
            {
                case StorageOptions.SimpleBruteForce:
                    return new SimpleBruteForceStorage();
                //case StorageOptions.OrthantNeighbourhoodGraph:
                //    return new OrthantNeighbourhoodGraphStorage();
                default:
                    return new SimpleBruteForceStorage();
            }
        }
    }
}
