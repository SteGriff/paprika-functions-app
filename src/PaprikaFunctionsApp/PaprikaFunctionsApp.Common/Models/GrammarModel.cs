using System;
using System.Collections.Generic;
using System.Linq;

namespace PaprikaFunctionsApp.Common.Models
{
    public class GrammarModel
    {
        public Dictionary<string, List<string>> GrammarDictionary { get; set; }

        public GrammarModel(Dictionary<string, List<string>> grammarDictionary)
        {
            GrammarDictionary = grammarDictionary;
        }
    }
}
