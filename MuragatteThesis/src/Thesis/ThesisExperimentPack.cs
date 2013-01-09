// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
//
// Copyright (C) 2013  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Environment.Agents;
using Muragatte.Core.Storage;
using Muragatte.Random;
using Muragatte.Research;
using Muragatte.Visual.Styles;

namespace Muragatte.Thesis
{
    public class ThesisExperimentPack
    {
        #region Constants

        private const double REPULSION_RANGE = 1.5;
        private const double SPEED = 1;

        private static readonly Angle TURNING_ANGLE = new Angle(115);

        #endregion

        #region Fields

        private Experiment _experiment = null;

        private int _iNaives = 0;
        private int _iGuides = 0;
        private int _iIntruder = 0;
        private double _dAssertN = 0;
        private double _dAssertG = 0;
        private double _dAssertI = 0;
        private double _dCredN = 0;
        private double _dCredG = 0;
        private double _dCredI = 0;
        private int _iRuns = 0;

        #endregion

        #region Constructors

        public ThesisExperimentPack(string name, int runs, int length, double timePerStep,
            Scene scene, SpeciesCollection species, IEnumerable<Style> styles, uint seed, string path,
            int nn, int ng, int ni, double an, double ag, double ai, double cn, double cg, double ci,
            SpawnSpot spawn, Goal gg, Goal gi, Species sn, Species sg, Species si, double fr, Angle fa)
        {
            _iNaives = nn;
            _iGuides = ng;
            _iIntruder = ni;
            _dAssertN = an;
            _dAssertG = ag;
            _dAssertI = ai;
            _dCredN = cn;
            _dCredG = cg;
            _dCredI = ci;
            _experiment = new Experiment(name, runs,
                new InstanceDefinition(timePerStep, length, false, scene, species, StorageOptions.SimpleBruteForce,
                    CreateArchetypes(spawn, sn, sg, si, gg, gi, fr, fa)), styles, seed);
            _experiment.ExtraSetting.Path = path;
        }


        #endregion

        #region Properties

        public Experiment Experiment
        {
            get { return _experiment; }
        }

        public int NaiveCount
        {
            get { return _iNaives; }
        }

        public int GuideCount
        {
            get { return _iGuides; }
        }

        public int IntruderCount
        {
            get { return _iIntruder; }
        }

        public double AssertivenessNaive
        {
            get { return _dAssertN; }
        }

        public double AssertivenessGuide
        {
            get { return _dAssertG; }
        }

        public double AssertivenessIntruder
        {
            get { return _dAssertI; }
        }

        public double CredibilityNaive
        {
            get { return _dCredN; }
        }

        public double CredibilityGuide
        {
            get { return _dCredG; }
        }

        public double CredibilityIntruder
        {
            get { return _dCredI; }
        }

        public int Runs
        {
            get { return _iRuns; }
        }

        #endregion

        #region Methods

        public void Cancel(int run)
        {
            _experiment.Cancel();
            _iRuns = run;
        }

        public void PostProcessing()
        {
            _experiment.PostProcessing();
            if (_experiment.IsComplete) _iRuns = _experiment.RepeatCount;
        }

        private IEnumerable<ObservedArchetype> CreateArchetypes(SpawnSpot spawn, Species sn, Species sg, Species si, Goal gg, Goal gi, double fovRange, Angle fovAngle)
        {
            List<ObservedArchetype> oas = new List<ObservedArchetype>();
            oas.Add(CreateAgents("Naives", _iNaives, spawn, sn, fovRange, fovAngle, null, _dAssertN, _dCredN, false));
            if (_iGuides > 0) oas.Add(CreateAgents("Guides", _iGuides, spawn, sg, fovRange, fovAngle, gg, _dAssertG, _dCredG, true));
            if (_iIntruder > 0) oas.Add(CreateAgents("Intruders", _iIntruder, spawn, si, fovRange, fovAngle, gi, _dAssertI, _dCredI, true));
            return oas;
        }

        private ObservedArchetype CreateAgents(string name, int count, SpawnSpot spawn, Species species,
            double fovRange, Angle fovAngle, Goal goal, double assertiveness, double credibility, bool observed)
        {
            ObservedArchetype oa = new ObservedArchetype(new Vejmola2013AgentArchetype(name, count, spawn,
                new NoisedDouble(Distribution.Uniform, -180, 180), new NoisedDouble(SPEED),
                species, new Neighbourhood(fovRange, fovAngle), TURNING_ANGLE,
                new Vejmola2013AgentArgs(goal, new Neighbourhood(REPULSION_RANGE, fovAngle), assertiveness, credibility)));
            oa.IsObserved = observed;
            return oa;
        }

        #endregion
    }
}
