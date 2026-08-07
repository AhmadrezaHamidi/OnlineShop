using Base.Domain.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Domain.Entities
{
    public class AttendanceEntity : BaseEntity
    {
        public AttendanceEntity(DateTime entryTime, DateTime? exitTime, Guid employeeId) : base()
        {
            EntryTime = entryTime;
            ExitTime = exitTime;
            EmployeeId = employeeId;
        }

        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        // رابطه با کارمند
        public Guid EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }
    }

    
}


