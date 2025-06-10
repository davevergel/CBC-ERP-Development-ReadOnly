using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Production
{
    public class RoastProfile
    {
        public int Id { get; set; }
        public DateTime RoastDate { get; set; }
        public string BeanType { get; set; }
        public string ProfileFilePath { get; set; }
        public string Notes { get; set; }
    }
}
