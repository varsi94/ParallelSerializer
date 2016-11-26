using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class TaskTreeNode
    {
        public object SyncRoot { get; } = new object();

        public List<TaskTreeNode> Children { get; set; }

        public ISerializationTask Task { get; set; }

        public TaskTreeNode NextSibling { get; set; }

        public TaskTreeNode Parent { get; set; }

        public TaskTreeNode()
        {
            Children = new List<TaskTreeNode>();
        }
    }
}
