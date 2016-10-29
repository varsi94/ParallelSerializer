using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public class TaskDictionary : ConcurrentDictionary<Type, TaskGenerationResult>
    {
        public IEnumerable<string> GetNamespaces()
        {
            return this.Select(x => x.Key.Namespace).Distinct();
        }

        public IEnumerable<string> GetAssemblyLocations()
        {
            return this.Select(x => x.Key.Assembly.Location).Distinct();
        }
    }
}
