using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanban.Kanbanize
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LimitPerHourAttribute : Attribute
    {
        public int Value { get; set; }
    }
}
