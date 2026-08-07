using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Domain.Entities.TallarEntities
{
    public class OptionsTallar
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public bool IsDeleted { get; set; }

        public int TallarId { get; set; }
        public Tallar Tallar { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class Tallar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Capability { get; set; }
        public long Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public List<OptionsTallar> Optiones { get; set; }
    }
}
