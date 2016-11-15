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

        public TaskTreeNode()
        {
            Children = new List<TaskTreeNode>();
        }

        public int GetLength()
        {
            int result = Task.SerializationResult?.Length ?? 0;
            foreach (var taskTreeNode in Children)
            {
                result += taskTreeNode.GetLength();
            }
            return result;
        }
    }
}
