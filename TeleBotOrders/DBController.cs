using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleBotOrders
{
    internal static class DBController
    {
        public static bool AddNewUser(User user)
        {
            try
            {
                ApplicationContext db;
                using (db = new ApplicationContext())
                {
                    var users = db.Users.ToList();
                    var users1 = db.Users;
                    var userFromDb =  users1.Find(user.Id);
                    if (users.Find(u => u.Id == user.Id) == null)
                    {
                        db.Users.Add(user);
                    }
                    else
                    {
                        //db.Users.Update(userFromDb);
                        db.Users.Remove(userFromDb);
                        db.Users.Add(user);
                    }
                    db.SaveChanges();
                }
                return true;    
            }
            
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return false;
            }
        }
        public static IEnumerable<long> GetAllUsersId() 
        {
            try
            {
                ApplicationContext db;
                using (db = new ApplicationContext())
                {
                    var usersId = db.Users.ToList().Select(x => x.Id);
                    return usersId;
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return new List<long>();
            }
        }
    }
}
