using System;
using System.Collections.Generic;

namespace CMG.License.Shared.PrismHelpers
{
    public class RegionMetaData
    {
        private readonly Dictionary<int, string> registries = new Dictionary<int, string>();

        public RegionMetaData(string name)
        {
            Name = name;
            Key = $"{name.Remove(name.LastIndexOf("Region", StringComparison.Ordinal))}View";
        }

        public string Key { get; private set; }
        public string Name { get; private set; }
        public string GetView(int key)
        {
            if (registries.ContainsKey(key))
                return registries[key];
            else
                throw new ArgumentException("View not registered");
        }

        public void RegisterView(int key, string viewName)
        {
            registries[key] = $"{viewName}View";
        }
    }
}
