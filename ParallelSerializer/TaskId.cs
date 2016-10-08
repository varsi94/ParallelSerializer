using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class TaskId : IComparable<TaskId>
    {
        private readonly List<int> list;

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
                if (i >= list.Count && i < other.list.Count)
                {
                    return -1;
                }
                else if (i >= other.list.Count && i < list.Count)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return a.CompareTo(b);
            }
        }

        protected TaskId(List<int> list)
        {
            this.list = list;
        }

        public static TaskId CreateDefault()
        {
            return new TaskId(new List<int> {1});
        }

        public TaskId CreateChild(int id)
        {
            var result = new TaskId(list.ToList());
            result.list.Add(id);
            return result;
        }

        public override string ToString()
        {
            return string.Join("_", list.Select(x => x.ToString()));
        }
    }
}
