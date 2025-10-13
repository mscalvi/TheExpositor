using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoCreator.Models
{
    public class ElementModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CardName { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string ImageName { get; set; }
        public string AddTime { get; set; }

        public bool Obsolete { get; set; }
        public int ParentId { get; set; }
        public int Version { get; set; }
    }
}
