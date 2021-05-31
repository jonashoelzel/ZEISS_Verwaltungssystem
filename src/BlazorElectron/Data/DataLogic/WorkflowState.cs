using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorElectron.Data.DataLogic
{
    public class WorkflowState
    {
        public State State { get; set; } = State.Add;
    }

    public enum State
    {
        Add, Edit,
    }
}
