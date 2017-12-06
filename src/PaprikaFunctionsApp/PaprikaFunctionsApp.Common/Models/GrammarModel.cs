using System;
using System.Collections.Generic;
using System.Linq;

namespace PaprikaFunctionsApp.Common.Models
{
    public class GrammarModel
    {
        public List<Category> Entries { get; set; }

        public GrammarModel(Dictionary<string, List<string>> grammarDictionary)
        {
            Entries = new List<Category>();
            foreach(var gde in grammarDictionary)
            {
                Entries.Add(new Category(gde.Key, gde.Value));
            }
        }

        public class Category
        {
            public string Name { get; set; }
            public List<string> Items { get; set; }

            public Category(string name, List<string> items)
            {
                Name = name;
                Items = items;
            }
        }

        public Dictionary<string, List<string>> ToPaprikaDictionary()
        {
            return Entries.ToDictionary(c => c.Name, c => c.Items);
        }
    }
}
