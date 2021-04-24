using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
namespace WebApplication.DAO
{
    public class UserDAO
    {
        JordanEntities db = new JordanEntities();
        public long Insert(Customer c)
        {
            db.Customers.Add(c);
            db.SaveChanges();
            return c.Customer_ID;
        }
        public bool CheckUserName(string userName)
        {
            return db.Customers.Count(x => x.Username == userName) > 0;
        }
        public bool CheckUserID(int ID)
        {
            return db.Customers.Count(x => x.Customer_ID == ID) > 0;
        }
        public int Login(string userName, string password)
        {
            var result = db.Customers.SingleOrDefault(x => x.Username == userName);
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
        public Customer GetById(string userName)
        {
            return db.Customers.SingleOrDefault(x => x.Username == userName);
        }

        public long InsertForFacebook(Customer entity)
        {
            var user = db.Customers.SingleOrDefault(x => x.Username == entity.Username);
            if (user == null)
            {
                db.Customers.Add(entity);
                db.SaveChanges();
                return entity.Customer_ID;
            }
            else
            {
                return user.Customer_ID;
            }

        }
    }
}