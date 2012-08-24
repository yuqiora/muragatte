// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Visual.Shapes;
using Muragatte.Visual.Styles;

namespace Muragatte.Visual
{
    public class Appearance : INotifyPropertyChanged
    {
        #region Fields

        private Element _element = null;
        private Style _style = null;
        private bool _bEnabled = true;
        private bool _bNeighbourhoodEnabled = true;
        private bool _bTrackEnabled = false;
        private bool _bTrailEnabled = false;
        private bool _bHighlighted = false;
        //private Color? _primaryColor = null;
        //private Color? _secondaryColor = null;
        //private Color? _neighbourhoodPrimaryColor = null;
        //private Color? _neighbourhoodSecondaryColor = null;
        //private Color? _trackColor = null;
        //private Color? _trailColor = null;
        private int _iWidth = 1;
        private int _iHeight = 1;
        private int _iRadius = 1;
        private List<Coordinates> _elementCoordinates = null;
        private List<Coordinates> _neighbourhoodCoordinates = null;

        #endregion

        #region Constructors

        public Appearance(Element element, Style style, double scale)
        {
            _element = element;
            _style = style;
            Rescale(scale);
        }

        #endregion

        #region Properties

        public int ID
        {
            get { return _element.ID; }
        }

        public string Name
        {
            get { return _element.Name; }
        }

        public bool IsStationary
        {
            get { return _element.IsStationary; }
        }

        public bool IsAgent
        {
            get { return _element is Agent; }
        }

        public string Species
        {
            get { return _element.Species == null ? string.Empty : _element.Species.Name; }
        }

        public double UnitWidth
        {
            get { return _element.Width; }
        }

        public double UnitHeight
        {
            get { return _element.Height; }
        }

        public double UnitRadius
        {
            get { return _element is Agent ? ((Agent)_element).FieldOfView.Range : -1; }
        }

        public Angle NeighbourhoodAngle
        {
            get
            {
                if (_element is Agent)
                {
                    return ((Agent)_element).FieldOfView.Angle != _style.Neighbourhood.Angle ? ((Agent)_element).FieldOfView.Angle : _style.Neighbourhood.Angle;
                }
                else
                {
                    return Angle.Zero;
                }
            }
        }

        public int Width
        {
            get { return _iWidth != _style.Width ? _iWidth : _style.Width; }
        }

        public int Height
        {
            get { return _iHeight != _style.Height ? _iHeight : _style.Height; }
        }

        public int Radius
        {
            get { return _iRadius != _style.Neighbourhood.Radius ? _iRadius : _style.Neighbourhood.Radius; }
        }

        public Style Style
        {
            get { return _style; }
            set
            {
                _style = value;
                RecreateCoordinates(true);
                NotifyPropertyChanged("Style");
            }
        }

        public bool IsEnabled
        {
            get { return _bEnabled; }
            set
            {
                _bEnabled = value;
                NotifyPropertyChanged("IsEnabled");
            }
        }

        public bool IsNeighbourhoodEnabled
        {
            get { return _bNeighbourhoodEnabled; }
            set
            {
                _bNeighbourhoodEnabled = value;
                NotifyPropertyChanged("IsNeighbourhoodEnabled");
            }
        }

        public bool IsTrackEnabled
        {
            get { return _bTrackEnabled; }
            set
            {
                _bTrackEnabled = value;
                NotifyPropertyChanged("IsTrackEnabled");
            }
        }

        public bool IsTrailEnabled
        {
            get { return _bTrailEnabled; }
            set
            {
                _bTrailEnabled = value;
                NotifyPropertyChanged("IsTrailEnabled");
            }
        }

        public bool IsHighlighted
        {
            get { return _bHighlighted; }
            set
            {
                _bHighlighted= value;
                NotifyPropertyChanged("IsHighlighted");
            }
        }

        //public Color PrimaryColor
        //{
        //    get { return _primaryColor.GetValueOrDefault(_style.PrimaryColor); }
        //    set
        //    {
        //        _primaryColor = value;
        //        NotifyPropertyChanged("PrimaryColor");
        //    }
        //}

        //public Color SecondaryColor
        //{
        //    get { return _secondaryColor.GetValueOrDefault(_style.SecondaryColor); }
        //    set
        //    {
        //        _secondaryColor = value;
        //        NotifyPropertyChanged("SecondaryColor");
        //    }
        //}

        //public Color NeighbourhoodPrimaryColor
        //{
        //    get
        //    {
        //        return _neighbourhoodPrimaryColor.GetValueOrDefault(
        //            _style.Neighbourhood != null ? _style.Neighbourhood.PrimaryColor : PrimaryColor);
        //    }
        //    set
        //    {
        //        _neighbourhoodPrimaryColor = value;
        //        NotifyPropertyChanged("NeighbourhoodPrimaryColor");
        //    }
        //}

        //public Color NeighbourhoodSecondaryColor
        //{
        //    get
        //    {
        //        return _neighbourhoodSecondaryColor.GetValueOrDefault(
        //            _style.Neighbourhood != null ? _style.Neighbourhood.SecondaryColor : PrimaryColor);
        //    }
        //    set
        //    {
        //        _neighbourhoodSecondaryColor = value;
        //        NotifyPropertyChanged("NeighbourhoodSecondaryColor");
        //    }
        //}

        //public Color TrackColor
        //{
        //    get { return _trackColor.GetValueOrDefault(_style.Track != null ? _style.Track.Color : PrimaryColor); }
        //    set
        //    {
        //        _trackColor = value;
        //        NotifyPropertyChanged("TrackColor");
        //    }
        //}

        //public Color TrailColor
        //{
        //    get { return _trailColor.GetValueOrDefault(_style.Trail != null ? _style.Trail.Color : PrimaryColor); }
        //    set
        //    {
        //        _trailColor = value;
        //        NotifyPropertyChanged("TrailColor");
        //    }
        //}

        #endregion

        #region Methods

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction)
        {
            _style.Draw(target, position, direction, _elementCoordinates);
        }

        public void DrawNeighbourhood(WriteableBitmap target, Vector2 position, Vector2 direction)
        {
            if (_style.HasNeighbourhood)
            {
                _style.Neighbourhood.Draw(target, position, direction, _neighbourhoodCoordinates);
            }
        }

        public void DrawTrail(WriteableBitmap target, Vector2 position, Vector2 direction, byte alpha = byte.MaxValue)
        {
            if (_style.HasTrail)
            {
                _style.Draw(target, position, direction, Style.Trail.Color.WithA(alpha), _elementCoordinates);
            }
        }

        public bool IsType<T>() where T : Element
        {
            return _element is T;
        }

        public void Rescale(double value)
        {
            _iWidth = (int)(_element.Width * value);
            _iHeight = (int)(_element.Height * value);
            if (UnitRadius >= 0) _iRadius = (int)(UnitRadius * value);
            RecreateCoordinates(false);
        }

        public void RecreateCoordinates(bool forced)
        {
            if ((forced && _elementCoordinates != null) || _element.Width != _style.UnitWidth || _element.Height != _style.UnitHeight)
                _elementCoordinates = _style.Shape.CreateCoordinates(_iWidth, _iHeight);
            else _elementCoordinates = null;
            if (IsAgent && _style.HasNeighbourhood && ((forced && _neighbourhoodCoordinates != null) || ((Agent)_element).FieldOfView.Range != _style.Neighbourhood.UnitRadius || ((Agent)_element).FieldOfView.Angle != _style.Neighbourhood.Angle))
                _neighbourhoodCoordinates = _style.Neighbourhood.Shape.CreateCoordinates(_iRadius * 2, _iRadius * 2, NeighbourhoodAngle);
            else _neighbourhoodCoordinates = null;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
