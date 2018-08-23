using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace UrlShortener.Data
{
    public class Settings
    {
        public string ConnectionString;
        public string Database;
    }

    public class UrlDbContext
    {
        private readonly IMongoDatabase _database = null;

        public UrlDbContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<TheUrl> Urls
        {
            get
            {
                return _database.GetCollection<TheUrl>("TheUrl");
            }
        }

    }


}
