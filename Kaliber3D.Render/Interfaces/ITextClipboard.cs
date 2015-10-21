//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> ContainsText();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> GetText();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task SetText(string text);
    }
}
