using Base.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base.Domain.Entities
{
    public class EmployeeEntity : BaseEntity
    {
        public EmployeeEntity(string name, string employeeNumber) : base()
        {
            Name = name;
            EmployeeNumber = employeeNumber;
        }

        public string Name { get; set; }
        public string EmployeeNumber { get; set; }

        // رابطه با ورود و خروج‌ها
        public ICollection<AttendanceEntity> Attendances { get; set; }

        // رابطه با ماموریت‌ها
        public ICollection<MissionEntity> Mission { get; set; }
    }
}
