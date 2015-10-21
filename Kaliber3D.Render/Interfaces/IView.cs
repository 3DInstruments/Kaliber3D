//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// 
        /// </summary>
        object DataContext { get; set; }
        /// <summary>
        /// 
        /// </summary>
        void Close();
    }
}
