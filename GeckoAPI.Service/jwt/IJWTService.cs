using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.jwt
{
    public interface IJWTService
    {
        string GenerateToken(string userId, string username);
    }
}
