using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBLab
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoActions db = new MongoActions("People");

            while (true)
            {
                Console.WriteLine("Do you want to read by a key(R), write(W), search by name(S), delete(D) or exit(E)?");
                string input = Console.ReadLine();

                if (input.ToUpper() == "R")
                {
                    Console.WriteLine("Please enter a key");
                    string key = Console.ReadLine();
                    db.ReadRecordById("Users", key);
                }

                else if (input.ToUpper() == "W")
                {
                    Console.WriteLine("Please enter a key");
                    string key = Console.ReadLine();
                    Console.WriteLine("Please enter a name");
                    string value = Console.ReadLine();

                    db.InsertRecord("Users", new Person { id = key, name = value });
                }

                else if (input.ToUpper() == "S")
                {
                    Console.WriteLine("Please enter a name");
                    string name = Console.ReadLine();
                    db.ReadByName("Users", name);
                }

                else if (input.ToUpper() == "D")
                {
                    Console.WriteLine("Please enter a key");
                    string key = Console.ReadLine();
                    db.DeleteRecordById<Person>("Users", key);
                }

                else if (input.ToUpper() == "E")
                    break;
            }
        }
    }

    public class Person
    {
        [BsonId]
        public string id { get; set; }
        public string name { get; set; }
    }

    public class MongoActions
    {
        private IMongoDatabase db;

        public MongoActions(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public void InsertRecord<Person>(string table, Person record)
        {
            var collection = db.GetCollection<Person>(table);
            try
            {
                collection.InsertOne(record);
                Console.WriteLine("Write successful");
            }
            catch (Exception)
            {
                Console.WriteLine("Error, id already exists");
            }
        }

        public void ReadRecordById(string table, string id)
        {
            var collection = db.GetCollection<Person>(table);
            var filter = Builders<Person>.Filter.Eq("id", id);

            try
            {
                Person result = collection.Find(filter).First();
                Console.WriteLine(result.id + " " + result.name);
            }
            catch (Exception)
            {
                Console.WriteLine("Error, nothing found");
            }
        }

        public void DeleteRecordById<Person>(string table, string id)
        {
            var collection = db.GetCollection<Person>(table);
            var filter = Builders<Person>.Filter.Eq("id", id);

            collection.DeleteOne(filter);
            Console.WriteLine("Deleted");
        }

        public void ReadByName(string table, string name)
        {
            var collection = db.GetCollection<Person>(table);
            var filter = Builders<Person>.Filter.Eq("name", name);

            List<Person> list = collection.Find(filter).ToList();

            if (list.Count() == 0)
                Console.WriteLine("Error: nothing found");
            else
                foreach (Person p in list)
                    Console.WriteLine(p.id + " " + p.name);
        }
    }
}
