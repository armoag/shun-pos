using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shun
{
    public interface ISqLDataBase
    {
        string Server { get; set; }
        string UserId { get; set; }
        string Password { get; set; }
        string SqlDataBase { get; set; }
        string Table { get; set; }
        MySqlDatabase MySqlData { get; set; }
    }
}
