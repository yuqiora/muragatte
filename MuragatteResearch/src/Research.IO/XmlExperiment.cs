// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Muragatte.Core;
using Muragatte.Core.Storage;
using Muragatte.IO;
using Muragatte.Visual.IO;
using Muragatte.Visual.Styles;

namespace Muragatte.Research.IO
{
    public class XmlExperiment
    {
        #region Fields

        [XmlAttribute]
        public string Name = null;

        [XmlAttribute]
        public int Repeat = 1;

        [XmlAttribute]
        public int Length = 1;

        [XmlAttribute]
        public double TimePerStep = 1;

        [XmlAttribute]
        public bool KeepSubsteps = false;

        public uint Seed = 0;

        public StorageOptions Storage = StorageOptions.SimpleBruteForce;

        [XmlArrayItem("Style")]
        public Style[] Styles = null;

        public XmlSpeciesCollection KnownSpecies = null;

        [XmlElement(Type = typeof(XmlScene))]
        public Scene Scene = null;

        [XmlArrayItem("Archetype")]
        public ObservedArchetype[] Archetypes = null;

        #endregion

        #region Constructors

        public XmlExperiment() { }

        public XmlExperiment(Experiment experiment)
        {
            Name = experiment.Name;
            Repeat = experiment.RepeatCount;
            Length = experiment.Definition.Length;
            TimePerStep = experiment.Definition.TimePerStep;
            KeepSubsteps = experiment.Definition.KeepSubsteps;
            Seed = experiment.Seed;
            Storage = experiment.Definition.Storage.StorageType;
            Styles = experiment.Styles.ToArray();
            KnownSpecies = new XmlSpeciesCollection(experiment.Definition.Species);
            Scene = experiment.Definition.Scene;
            Archetypes = experiment.Definition.Archetypes.ToArray();
        }

        #endregion

        #region Methods

        public Experiment ToExperiment()
        {
            return new Experiment(Name, string.Empty, Repeat,
                new InstanceDefinition(TimePerStep, Length, KeepSubsteps, Scene, KnownSpecies, Storage.ToStorage(), Archetypes),
                new ObservableCollection<Style>(Styles), Seed);
        }

        public void ApplyToStyles(ObservableCollection<Style> collection)
        {
            foreach (Style s in Styles)
            {
                collection.Add(s);
            }
        }

        public void ApplyToArchetypes(ObservableCollection<ObservedArchetype> collection)
        {
            foreach (ObservedArchetype oa in Archetypes)
            {
                collection.Add(oa);
            }
        }

        #endregion
    }
}
