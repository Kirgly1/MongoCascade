using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

class Program
{
    static async Task Main()
    {
        string connectionString = "mongodb://localhost:27017";
        MongoClient client = new MongoClient(connectionString);
        IMongoDatabase database = client.GetDatabase("CatShop");

        IMongoCollection<Cat> catsCollection = database.GetCollection<Cat>("Cats");
        IMongoCollection<Comment> commentsCollection = database.GetCollection<Comment>("Comments");

        var random = new Random();
        
        // Генерация и вставка 10,000 документов для котов
        for (int i = 0; i < 10000; i++)
        {
            Cat newCat = new Cat
            {
                Id = ObjectId.GenerateNewId().ToString(),
                name = "Cat" + i,
                price = random.Next(50, 200),
                breed = "Breed" + random.Next(1, 5),
                age = random.Next(1, 10)
            };

            await catsCollection.InsertOneAsync(newCat);

            // Генерация и вставка 1 комментария для каждого кота
            Comment newComment = new Comment
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Text = "Comment for " + newCat.name,
                CatId = newCat.Id  // Устанавливаем CatId комментария равным Id кота
            };

            await commentsCollection.InsertOneAsync(newComment);
        }

        Console.WriteLine("Документы успешно добавлены в коллекции Cats и Comments.");
        

        
        string catIdToDelete = "id кота, который нужно удалить"; // Замените этот Id на реальный Id кота

        // Шаг 1: Удаление комментариев, связанных с котом
        await commentsCollection.DeleteManyAsync(comment => comment.CatId == catIdToDelete);

        // Шаг 2: Удаление самого кота
        await catsCollection.DeleteOneAsync(cat => cat.Id == catIdToDelete);
    }
}

public class Cat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string name { get; set; }

    public decimal price { get; set; }

    public string breed { get; set; }

    public int age { get; set; }
}

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Text { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string CatId { get; set; }
}
