using Microsoft.EntityFrameworkCore;
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
        public static User FindUserByIndex(long id)
        {
            try
            {
                ApplicationContext db;
                using (db = new ApplicationContext())
                {
                    return db.Users.ToList().Find(x=> x.Id == id);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return null;
            }
        }
        public static long CountOrders()
        {
            try
            {
                ApplicationContext db;
                using (db = new ApplicationContext())
                {
                    return db.Orders.Count();
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return -1;
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
        public static IEnumerable<Cafe> GetAllCafes() 
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    return db.Cafes.Include(c => c.Menu).ToList();
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return new List<Cafe>();
            }
        }
        public static IEnumerable<Dish> GetDishes(long index)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    var menu = db.Menus.FirstOrDefault(x => x.Id == index);
                    db.Dishes.Where(d => d.MenuId == menu.Id).Load();
                    //var dishes = db.Dishes.Include(x => x.Menu).ToList();
                    //return dishes.FindAll( d => d.Menu.Id == menu.Id);
                    return menu.Dishes;
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return new List<Dish>();
            };
        }
        public static void CreateTestCafe()
        {
            var dish1 = new Dish { Name = "CHICKEN", Discount = 200, Description = "TASTY CHICKEN", Price = 100, PathImage = "https://attuale.ru/wp-content/uploads/2017/10/herb-roasted-chicken-24-56a8b9d05f9b58b7d0f4a07b.jpg" };
            var dish2 = new Dish { Name = "PIZZA", Discount = 300, Description = "TASTY PIZZA", Price = 200, PathImage = "https://proprikol.ru/wp-content/uploads/2020/11/kartinki-piczcza-40.jpeg" };
            var menu1 = new Menu
            {
                Name = " МЕНЮ",
                Dishes = new List<Dish> {dish1 }

            };
            var menu2 = new Menu
            {
                Name = "menu",
                Dishes = new List<Dish> { dish2 }
            };
            Cafe dodo = new Cafe();
            dodo.Name = "DODO";
            dodo.Menu = menu1;
            Cafe dodo1 = new Cafe();
            dodo1.Name = "CAFE 3";
            dodo1.Menu = menu2;
            try
            {
                ApplicationContext db;
                using (db = new ApplicationContext())
                {
                    db.Cafes.AddRange(dodo1,dodo);

                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }
        }
    }
}
