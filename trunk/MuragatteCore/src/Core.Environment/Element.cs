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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.Core.Storage;

namespace Muragatte.Core.Environment
{
    public abstract class Element : INotifyPropertyChanged
    {
        #region Constants

        public const double DEFAULT_RADIUS = 0.5;

        #endregion

        #region Fields

        protected int _iElementID = 0;
        protected MultiAgentSystem _model = null;
        protected Vector2 _position = Vector2.Zero;
        protected bool _bEnabled = true;
        protected Species _species = null;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Element() { }

        public Element(int id, MultiAgentSystem model)
        {
            _iElementID = id;
            _model = model;
        }

        public Element(int id, MultiAgentSystem model, Vector2 position)
            : this(id, model)
        {
            _position = position;
        }

        protected Element(Element other, MultiAgentSystem model)
            : this(other._iElementID, model, other._position) { }

        #endregion

        #region Properties

        [XmlAttribute]
        public int ID
        {
            get { return _iElementID; }
            set { if (_model == null) _iElementID = value; }
        }

        [XmlIgnore]
        public MultiAgentSystem Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("PositionX");
                NotifyPropertyChanged("PositionY");
            }
        }

        [XmlIgnore]
        public double PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("PositionX");
            }
        }

        [XmlIgnore]
        public double PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("PositionY");
            }
        }

        [XmlIgnore]
        public bool IsEnabled
        {
            get { return _bEnabled; }
            set { _bEnabled = value; }
        }

        [XmlElement(Type = typeof(IO.XmlSpeciesReference), IsNullable = false)]
        public Species Species
        {
            get { return _species; }
            set { _species = value; }
        }

        [XmlIgnore]
        public virtual Group Group
        {
            get { return null; }
            set { }
        }

        #endregion

        #region Abstract Properties

        [XmlIgnore]
        public abstract Vector2 Direction { get; set; }

        [XmlIgnore]
        public abstract double Speed { get; set; }

        [XmlIgnore]
        public abstract double Width { get; set; }

        [XmlIgnore]
        public abstract double Height { get; set; }

        public abstract double Radius { get; }

        public abstract ElementNature DefaultNature { get; }

        public abstract string Name { get; }

        public abstract bool IsStationary { get; }

        public abstract bool IsDirectable { get; }

        public abstract bool IsResizeable { get; }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Element)
            {
                Element e = (Element)obj;
                return _iElementID == e._iElementID;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return _iElementID;
        }

        public override string ToString()
        {
            return ShortDescription();
        }

        protected string ShortDescription()
        {
            return string.Format("{0}{1}", _iElementID, IsStationary ? "s" : "");
        }

        protected string LongDescription()
        {
            return string.Format("{0}{1}{2} @({3})",
                _iElementID, IsStationary ? "s" : "", _bEnabled ? "" : "d", _position);
        }

        protected string ToString(string prefix)
        {
            return string.Format("{0}-{1}", prefix, LongDescription());
        }

        protected string CreateName(string prefix)
        {
            return string.Format("{0}-{1}", prefix, ShortDescription());
        }

        public Vector2 PredictPositionAfter()
        {
            return PredictPositionAfter((int)Math.Ceiling(1 / _model.TimePerStep));
        }

        public Vector2 PredictPositionAfter(int steps)
        {
            return IsStationary || steps == 0 ? GetPosition() : GetPosition() + Speed * _model.TimePerStep * steps * Direction;
        }

        //protected ElementStatus ReportStatus(IEnumerable<double> modifiers)
        //{
        //    return new ElementStatus(_iElementID, _position, Direction, Speed, _bEnabled,
        //        _species == null ? null : _species.FullName, Group == null ? -1 : Group.ID, modifiers);
        //}

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void SetSpecies(Species s, string name)
        {
            _species = s ?? (_model == null ? null : _model.Species.GetDefault(name));
        }

        #endregion

        #region Virtual Methods

        public virtual bool Equals(Element e)
        {
            return _iElementID == e._iElementID;
        }

        public virtual bool IntersectsWith(Vector2 p1, Vector2 p2, out Vector2 ip)
        {
            //inspiration from http://paulbourke.net/geometry/sphereline/
            Vector2 p2mp1 = p2 - p1;
            double u = ((_position - p1) * p2mp1) / (p2mp1 * p2mp1);
            ip = p1 + u * p2mp1;
            if (u < 0 || Vector2.Distance(_position, ip) > Radius)
            {
                return false;
            }
            double a = p2mp1.LengthSquared;
            double b = 2 * p2mp1 * (p1 - _position);
            double c = (_position.LengthSquared + p1.LengthSquared) - (2 * _position * p1) - (Radius * Radius);
            double bb4ac = b * b - 4 * a * c;
            if (bb4ac < 0)
            {
                ip = p2;
                return false;
            }
            if (bb4ac == 0)
            {
                double u0 = -b / (2 * a);
                ip = p1 + u0 * p2mp1;
                return true;
            }
            double u1 = (-b + Math.Sqrt(bb4ac)) / (2 * a);
            double u2 = (-b - Math.Sqrt(bb4ac)) / (2 * a);
            ip = p1 + Math.Min(u1, u2) * p2mp1;
            return true;
        }

        public virtual Vector2 GetPosition()
        {
            return Position;
        }

        public virtual Vector2 GetDirection()
        {
            return Direction;
        }

        public virtual double GetSpeed()
        {
            return Speed;
        }

        public virtual ElementStatus ReportStatus()
        {
            return new ElementStatus(_iElementID, _position, Direction, Speed, _bEnabled,
                _species == null ? null : _species.FullName, Group == null ? -1 : Group.ID);
        }

        public virtual void LoadStatus(ElementStatus status)
        {
            Position = status.Position;
            Direction = status.Direction;
            Speed = status.Speed;
            IsEnabled = status.IsEnabled;
            Species = status.SpeciesName == null ? null : _model.Species[status.SpeciesName];
        }

        #endregion

        #region Abstract Methods

        public abstract void Update();

        public abstract void ConfirmUpdate();

        public abstract ElementNature RelationshipWith(Element e);

        public abstract Element CloneTo(MultiAgentSystem model);

        #endregion
    }
}
