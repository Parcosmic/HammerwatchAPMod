using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiltedEngine.WorldObjects.WorldObjectProducers;

namespace HammerwatchAP.Game
{
    public class LootTableWrapper
    {
        public static Type _t_LootTable = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.LootTable");
        static FieldInfo _fi_entries = _t_LootTable.GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);

        public object lootTable;
        public List<Tuple<int, List<Tuple<int, IWorldItemProducer>>>> entries
        {
            get => (List<Tuple<int, List<Tuple<int, IWorldItemProducer>>>>)_fi_entries.GetValue(lootTable);
        }

        public LootTableWrapper(object lootTableObject)
        {
            lootTable = lootTableObject;
        }
    }
}
