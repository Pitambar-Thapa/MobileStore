using StoreDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Services
{
    public class UsersService:GenericService<Users>
    {
        public void Save(Users entitiy)
        {
            entitiy.CreatedDate = DateTime.Now;
            entitiy.CreatedBy = 1;
            entitiy.Status = 10;
            this.Add(entitiy);
        }
        public void Edit(Users entity)
        {
            this.Update(entity);
        }
        public void Delete(object UsersId)
        {

        }
        public List<Users> ListAll()
        {
            List<Users> usersList = new List<Users>();
            var UsersObj = this.ListAll();
            foreach (var item in UsersObj)
            {
                Users users = new Users();
                users.FName = item.FName;
                usersList.Add(users);
            }
            return usersList;
        }
    }
}
