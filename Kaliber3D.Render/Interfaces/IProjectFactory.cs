//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProjectFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Container GetTemplate(Project project, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Container GetContainer(Project project, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Document GetDocument(Project project, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Project GetProject();
    }
}
