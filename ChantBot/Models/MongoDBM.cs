using ChantBot.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChantBot.Models
{
    public class MongoDBM
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _client;
        public IMongoCollection<Player> PlayerCollection => _database.GetCollection<Player>("PlayerData");
        public IMongoCollection<Game> GameCollection => _database.GetCollection<Game>("Games");

        public MongoDBM(string connectionString, string databaseName = "ChantBot")
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public void EnsureDatabaseAndCollections()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connected to MongoDB.");
            Console.WriteLine("Database ChantBot will be created automatically on first use.");

            var collectionsToEnsure = new[] { "Games", "PlayerData", "Customer", "Group", "Admins", "Log", "Error" };
            foreach (var collectionName in collectionsToEnsure)
            {
                EnsureCollection(collectionName);
            }

            InitializeDefaultData();

            Console.ResetColor();
            Console.WriteLine("Database setup completed.");
        }

        private void EnsureCollection(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = _database.ListCollectionNames(new ListCollectionNamesOptions { Filter = filter });

            if (!collections.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Collection {collectionName} does not exist. Creating...");
                _database.CreateCollection(collectionName);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine($"Collection {collectionName} already exists.");
            }
        }

        private void InitializeDefaultData()
        {
            var adminCollection = _database.GetCollection<BsonDocument>("Admins");
            if (adminCollection.CountDocuments(new BsonDocument()) == 0)
            {
                Console.WriteLine("Initializing default admin data...");
                var defaultAdmins = new[]
                {
                    new BsonDocument
                    {
                        { "ID", "admin1" },
                        { "Name", "System Admin" }
                    }
                };
                adminCollection.InsertMany(defaultAdmins);
            }

            var groupCollection = _database.GetCollection<BsonDocument>("Group");
            if (groupCollection.CountDocuments(new BsonDocument()) == 0)
            {
                Console.WriteLine("Initializing default group data...");
                var defaultGroups = new[]
                {
                    new BsonDocument { { "Name", "Default Group" } }
                };
                groupCollection.InsertMany(defaultGroups);
            }

        }

        public List<OfficalUser> GetAllCustomers()
        {
            var collection = _database.GetCollection<BsonDocument>("Customer");
            var documents = collection.Find(new BsonDocument()).ToList();
            return documents.Select(doc => new OfficalUser
            {
                ID = doc.GetValue("ID", "").AsString,
                Name = doc.GetValue("Name", "").AsString
            }).ToList();
        }

        public List<OfficalUser> GetAllAdmins()
        {
            var collection = _database.GetCollection<BsonDocument>("Admins");
            var documents = collection.Find(new BsonDocument()).ToList();
            return documents.Select(doc => new OfficalUser
            {
                ID = doc.GetValue("ID", "").AsString,
                Name = doc.GetValue("Name", "").AsString
            }).ToList();
        }

        public List<string> GetAllGroups()
        {
            var collection = _database.GetCollection<BsonDocument>("Group");
            var documents = collection.Find(new BsonDocument()).ToList();
            return documents.Select(doc => doc.GetValue("Name", "").AsString).ToList();
        }

        public void AddCustomer(OfficalUser customer)
        {
            var collection = _database.GetCollection<BsonDocument>("Customer");
            var document = new BsonDocument
            {
                { "ID", customer.ID },
                { "Name", customer.Name }
            };
            collection.InsertOne(document);
        }

        public void AddAdmin(OfficalUser admin)
        {
            var collection = _database.GetCollection<BsonDocument>("Admins");
            var document = new BsonDocument
            {
                { "ID", admin.ID },
                { "Name", admin.Name }
            };
            collection.InsertOne(document);
        }

        public void AddGroup(string groupName)
        {
            var collection = _database.GetCollection<BsonDocument>("Group");
            var document = new BsonDocument { { "Name", groupName } };
            collection.InsertOne(document);
        }

        public void AddGame(Game game)
        {
            var collection = _database.GetCollection<BsonDocument>("Games");
            var document = new BsonDocument
            {
                { "GameID", game.GameID },
                { "StartDate", game.StartDate },
                { "Players", new BsonArray(game.Players) },
                { "GameType", game.GameType },
                { "MinAmount", game.MinAmount },
                { "GameResult", game.GameResult }
            };
            collection.InsertOne(document);
        }

        public List<Game> GetAllGames()
        {
            var collection = _database.GetCollection<BsonDocument>("Games");
            var documents = collection.Find(new BsonDocument()).ToList();
            return documents.Select(doc => new Game
            {
                GameID = doc.GetValue("GameID", 0).AsInt64,
                StartDate = doc.GetValue("StartDate", "").AsString,
                Players = doc.GetValue("Players", new BsonArray()).AsBsonArray.Select(x => x.AsString).ToArray(),
                GameType = doc.GetValue("GameType", 0).AsInt32,
                MinAmount = doc.GetValue("MinAmount", 0).AsInt32,
                GameResult = doc.GetValue("GameResult", "").AsString
            }).ToList();
        }
    }
}