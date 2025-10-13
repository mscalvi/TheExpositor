using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BingoCreator.Models
{
    public class CardSetModel
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public int ListSize { get; set; }
        public string ListName { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string End {  get; set; }
        public int Quantity { get; set; }
        public string ImageName { get; set; }
        public int CardsSize { get; set; }
        public string Theme { get; set; }
        public string Header { get; set; }
        public string Model { get; set; }
        public List<ElementModel> AllElements { get; set; }
        public List<ElementModel> GroupB { get; set; }
        public List<ElementModel> GroupI { get; set; }
        public List<ElementModel> GroupN { get; set; }
        public List<ElementModel> GroupG { get; set; }
        public List<ElementModel> GroupO { get; set; }
        public string GroupBIds { get; set; }
        public string GroupIIds { get; set; }
        public string GroupNIds { get; set; }
        public string GroupGIds { get; set; }
        public string GroupOIds { get; set; }
        public string AddDate { get; set; }
    }
}
