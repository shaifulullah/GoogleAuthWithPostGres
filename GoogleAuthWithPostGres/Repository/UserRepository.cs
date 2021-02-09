using GoogleAuthWithPostGres.Data;
using GoogleAuthWithPostGres.Models;
using GoogleAuthWithPostGres.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAuthWithPostGres.Repository
{
    public class UserRepository : IUser
    {
        private GoogleAuthWithPostGresContext db;

        public UserRepository(GoogleAuthWithPostGresContext _db)
        {
            db = _db;
        }

        public void Add(User _user)
        {
            db.Users.Add(_user);
            db.SaveChanges();
        }

        public User GetUser(int? ID)
        {
            User dbEntity = db.Users.SingleOrDefault(i => i.Id == ID);
            return dbEntity;
        }
        public void Remove()
        {

        }
        public ICollection<User> GetUsers()
        {
            ICollection<User> dbUsers = db.Users.ToList();
            return dbUsers;
        }

        public User GetUserFromId(int UserId)
        {
            return db.Users.Where(u => u.Id == UserId).Single();
        }

        public User GetUserFromEmail(string UserEmail)
        {
            User user = db.Users.Where(u => u.Email.Equals(UserEmail)).FirstOrDefault();
            return user;
        }
    }
}
