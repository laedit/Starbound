using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarboundModCreator.Core
{
    /// <summary>
    /// Zip file
    /// </summary>
    public class ModSolution
    {
        public string Name { get; set; }

        public List<ModProject> Projects { get; set; }
    }
}
