using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarboundModCreator.Core
{
    /// <summary>
    /// Root folder in zip file
    /// </summary>
    public class ModProject
    {
        public string Name { get; set; }

        public List<ModPart> Parts { get; set; }

        // TODO focus sur les fichiers => la partie "easy" sera apportée aprés via un diagramme
        // les fichiers surchargeables (ou inconnus) sont traités comme des fichiers bruts (pas de propriétés, uniquement du texte avec me "__merge" déjà présent)
    }
}
