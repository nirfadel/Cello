﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Services
{
    public interface IUserClaimsService
    {
        string GetUserId();
        string GetUserName();
        bool IsAdmin();
    }
}
