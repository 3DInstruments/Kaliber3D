﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        string Serialize<T>(T value);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json);
    }
}
