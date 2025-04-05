using LuxeCart.DatabaseWork.AdminPanelDB;
using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuxeCart.DatabaseWork.UserPanelDB
{
    public class UserDb
    {
        private readonly LuxeCartEntities dbContext;
        public UserDb()
        {
            dbContext = new LuxeCartEntities();
        }
        public RegisterUser GetUser(int id)
        {
            return dbContext.RegisterUsers.SingleOrDefault(u => u.UserID == id);
        }

    }
}