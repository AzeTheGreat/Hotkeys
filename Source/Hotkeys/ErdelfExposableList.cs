//All credit to Erdelf for this class
//https://github.com/erdelf/ExportAgency/blob/master/Source/ExportAgency/ExportAgency.cs#L272-L295

using System.Collections.Generic;
using Verse;

namespace Hotkeys
{
    public class ExposableList<T> : List<T>, IExposable
    {
        public ExposableList() : base(capacity: 1)
        {
        }

        public ExposableList(IEnumerable<T> exposables) : base(collection: exposables)
        {
        }

        public string Name { get; set; }

        public void ExposeData()
        {
            string name = this.Name;
            Scribe_Values.Look(value: ref name, label: "name");
            this.Name = name;

            List<T> list = this.ListFullCopy();
            Scribe_Collections.Look(list: ref list, label: "internalList");
            this.Clear();
            this.AddRange(collection: list);
        }
    }
}