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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Muragatte.IO
{
    public class XmlLoadSave<T>
    {
        #region Fields

        private XmlSerializer _serializer = new XmlSerializer(typeof(T));

        #endregion

        #region Constructors

        public XmlLoadSave() { }

        #endregion

        #region Methods

        public T Load(string path)
        {
            T item;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                item = (T)_serializer.Deserialize(stream);
            }
            return item;
        }

        public void Save(string path, T item)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                _serializer.Serialize(writer, item);
            }
        }

        #endregion
    }
}
