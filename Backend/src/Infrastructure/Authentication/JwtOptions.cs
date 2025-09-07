using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Authentication
{

    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public byte LifeTime { get; set; }
        public string SingingKey { get; set; }
    }
}
