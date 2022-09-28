using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class Account
    {
        public string Id;
        public string Username;
        public string Password;

        public Account(string id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }
    }
}
