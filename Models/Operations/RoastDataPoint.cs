using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Production
{
    public class RoastDataPoint
    {
        public int Id { get; set; }
        public int RoastProfileId { get; set; }
        public double TimeSeconds { get; set; }
        public double BeanTemp { get; set; }
        public double EnvironmentTemp { get; set; }
        public double ROR { get; set; }
    }

}
