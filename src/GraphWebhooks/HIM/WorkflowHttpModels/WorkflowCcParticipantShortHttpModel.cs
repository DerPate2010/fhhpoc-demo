using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Reference type for participants.
    /// </summary>
    public class WorkflowCcParticipantShortHttpModel
    {
        public int ccParticipantId { get; set; }
        public bool removable { get; set; }
    }
}
