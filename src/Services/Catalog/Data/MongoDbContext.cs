using Catalog.Entities;
using Catalog;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Book> _booksCollection;
    private readonly IMongoCollection<Review> _reviewsCollection;

    public MongoDbContext(IOptions<DatabaseSetting> databaseSettings)
    {
        var mongoDbClient = new MongoClient(databaseSettings.Value.ConnectionString);
        _database = mongoDbClient.GetDatabase(databaseSettings.Value.DatabaseName);

        // Get the actual collection
        _booksCollection = _database.GetCollection<Book>("book");
        _reviewsCollection = _database.GetCollection<Review>("review");

    }

    public IMongoDatabase Database => _database;
    public IMongoCollection<Book> Books => _booksCollection;
    public IMongoCollection<Review> Reviews => _reviewsCollection;
}
