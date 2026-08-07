using Base.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Domain.Entities
{
    public class MissionEntity : BaseEntity
    {
        public MissionEntity(string title, string description, Guid employeeId) : base()
        {
            Title = title;
            Description = description;
            EmployeeId = employeeId;
        }

        public string Title { get;  set; }
        public string Description { get; set; }

        // رابطه با کارمند
        public Guid EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }
    }
}
