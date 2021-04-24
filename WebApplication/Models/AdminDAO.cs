using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AdminDAO
    {
        JordanEntities db = new JordanEntities();
        public long Insert(Administrator admin)
        {
            db.Administrators.Add(admin);
            db.SaveChanges();
            return admin.Account_ID;
        }

        public bool CheckUserName(string userName)
        {
            return db.Administrators.Count(x => x.Username == userName) > 0;
        }

        public int Login(string userName, string password)
        {
            var result = db.Administrators.SingleOrDefault(x => x.Username == userName);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (result.Password == password)
                    return 1;
                else
                    return -2;

            }
        }
        public Administrator GetById(string userName)
        {
            return db.Administrators.SingleOrDefault(x => x.Username == userName);
        }
    }
}