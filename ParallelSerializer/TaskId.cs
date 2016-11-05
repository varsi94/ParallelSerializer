using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class TaskId : IComparable<TaskId>
    {
        private readonly IReadOnlyList<int> list;

        public int CompareTo(TaskId other)
        {
            int a = list[0];
            int b = other.list[0];
            int i = 0;
            while (a == b && i < list.Count && i < other.list.Count)
            {
                a = list[i];
                b = other.list[i];
                i++;
            }

            if (a == b)
            {
                return list.Count.CompareTo(other.list.Count);
            }
            else
            {
                return a.CompareTo(b);
            }
        }

        protected TaskId(IReadOnlyList<int> list)
        {
            this.list = list;
        }

        public static TaskId CreateDefault()
        {
            return new TaskId((new List<int> { 1 }).AsReadOnly());
        }

        public TaskId CreateChild(int id)
        {
            var newList = list.ToList();
            newList.Add(id);
            return new TaskId(newList.AsReadOnly());
        }

        public override string ToString()
        {
            return string.Join("_", list.Select(x => x.ToString()));
        }
    }
}
