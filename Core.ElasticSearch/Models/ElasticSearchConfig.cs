using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch.Models
{
    public class ElasticSearchConfig
    {

        public string ConnectionString { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ElasticSearchConfig()
        {
            ConnectionString = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
        }

        public ElasticSearchConfig(string connectionString, string username, string password)
        {
            ConnectionString = connectionString;
            Username = username;
            Password = password;
        }
    }
}
