using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanban.Kanbanize
{
    public enum KanbanizeCommand
    {
        #region Task Commands        
        [LimitPerHourAttribute(Value = 30)]
        create_new_task,
        [LimitPerHourAttribute(Value = 60)]
        delete_task,
        [LimitPerHourAttribute(Value = 60)]
        get_task_details,
        [LimitPerHourAttribute(Value = 60)]
        get_all_tasks,
        [LimitPerHourAttribute(Value = 30)]
        add_comment,
        [LimitPerHourAttribute(Value = 60)]
        move_task,
        [LimitPerHourAttribute(Value = 30)]
        edit_task
        #endregion
    }
}
