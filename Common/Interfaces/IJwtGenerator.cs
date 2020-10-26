using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user);
    }
}
