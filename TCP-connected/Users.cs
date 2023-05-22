using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_connected
{
    public class Users
    {
        private readonly User[] users;

        public Users (User[] users)
        {
            this.users = users;
        }
        public string SearchUser(string username, string password)
        {
            foreach (var user in users)
            {
                if (user.Username == username && user.Password == password)
                {
                    return "Success" + "\n";
                }
            }
            return "Fail" + "\n";
        }
    }
}
