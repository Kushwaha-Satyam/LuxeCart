﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuxeCart.Models
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}