﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Kaliber3D.Render
{
    /// <summary>
    /// Kaliber3D.Render data context.
    /// </summary>
    public class Data : ObservableObject
    {
        private ImmutableArray<ShapeBinding> _bindings;
        private ImmutableArray<ShapeProperty> _properties;
        private Record _record;

        /// <summary>
        /// Gets or sets a colletion ShapeBinding that will be used during drawing.
        /// </summary>
        public ImmutableArray<ShapeBinding> Bindings
        {
            get { return _bindings; }
            set { Update(ref _bindings, value); }
        }

        /// <summary>
        /// Gets or sets a colletion ShapeProperty that will be used during drawing.
        /// </summary>
        public ImmutableArray<ShapeProperty> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        /// <summary>
        /// Gets or sets shape data record.
        /// </summary>
        public Record Record
        {
            get { return _record; }
            set { Update(ref _record, value); }
        }

        /// <summary>
        /// Gets or sets property Value using Name as key for Properties array values. If property with the specified key does not exist it is created.
        /// </summary>
        /// <param name="name">The property name value.</param>
        /// <returns>The property Value.</returns>
        public object this[string name]
        {
            get
            {
                var result = _properties.FirstOrDefault(p => p.Name == name);
                if (result != null)
                {
                    return result.Value;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    var result = _properties.FirstOrDefault(p => p.Name == name);
                    if (result != null)
                    {
                        result.Value = value;
                    }
                    else
                    {
                        var property = ShapeProperty.Create(name, value);
                        Properties = Properties.Add(property);
                    }
                }
            }
        }
    }
}
