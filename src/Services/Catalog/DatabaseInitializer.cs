using Catalog.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Dynamic;
using System.Globalization;
using static Catalog.DatabaseInitializer;

namespace Catalog
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeCollections(IMongoDatabase database, string booksCsvPath, string warehouseBooksCsvPath)
        {
            // Check if the "book" collection exists; if not, initialize it
            if (!CollectionExists(database, "book"))
            {
                var bookCollection = database.GetCollection<BsonDocument>("book");
                var bookDocuments = ReadCsvToBsonDocumentsCatalog(booksCsvPath);
                if (bookDocuments.Any())
                    await bookCollection.InsertManyAsync(bookDocuments);
            }

            // Check if the "warehouseBooks" collection exists; if not, initialize it
            if (!CollectionExists(database, "warehouseBooks"))
            {
                var warehouseBooksCollection = database.GetCollection<BsonDocument>("warehouseBooks");
                var warehouseDocuments = ReadCsvToBsonDocumentsWarehouse(warehouseBooksCsvPath);
                if (warehouseDocuments.Any())
                    await warehouseBooksCollection.InsertManyAsync(warehouseDocuments);
            }
        }

        private static bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }
        private static IEnumerable<BsonDocument> ReadCsvToBsonDocumentsCatalog(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null // Ignore missing fields
            });

            // Register class map for automatic mapping
            csv.Context.RegisterClassMap<BookMap>(); // Ensure you have a class map if necessary

            csv.Read();
            csv.ReadHeader();
            var records = csv.GetRecords<Book>().ToList(); // Read all records into a list

            foreach (var record in records)
            {
                var bsonDoc = new BsonDocument();

                // Map properties to BSON document, skipping the Id field because MongoDB will generate it
                foreach (var property in typeof(Book).GetProperties())
                {
                    if (property.Name != nameof(Book.Id)) // Skip Id field
                    {
                        var value = property.GetValue(record);
                        bsonDoc[property.Name] = BsonValue.Create(value);
                    }
                }

                yield return bsonDoc;
            }
        }


        private static IEnumerable<BsonDocument> ReadCsvToBsonDocumentsWarehouse(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null // Ignore missing fields
            });

            // Register class map for automatic mapping
            csv.Context.RegisterClassMap<WarehouseBookMap>(); // Ensure you have a class map if necessary

            csv.Read();
            csv.ReadHeader();
            var records = csv.GetRecords<WarehouseBook>().ToList(); // Read all records into a list

            foreach (var record in records)
            {
                var bsonDoc = new BsonDocument();

                // Map properties to BSON document, skipping the Id field because MongoDB will generate it
                foreach (var property in typeof(WarehouseBook).GetProperties())
                {
                    if (property.Name != nameof(WarehouseBook.Id)) // Skip Id field
                    {
                        var value = property.GetValue(record);
                        bsonDoc[property.Name] = BsonValue.Create(value);
                    }
                }

                yield return bsonDoc;
            }
        }

        public sealed class BookMap : ClassMap<Book>
        {
            public BookMap()
            {
                // Явно вказуємо імена колонок CSV для мапінгу
                Map(m => m.Title).Name("Title");
                Map(m => m.description).Name("description");
                Map(m => m.authors).Name("authors");
                Map(m => m.image).Name("image");
                Map(m => m.previewLink).Name("previewLink");
                Map(m => m.publisher).Name("publisher");

                // Використовуємо кастомний конвертер для PublishedDate
                Map(m => m.publishedDate)
                .Name("publishedDate")
                    .TypeConverter<CustomYearOrDateTimeConverter>(); // Тут вказуємо наш кастомний конвертер

                Map(m => m.infoLink).Name("infoLink");
                Map(m => m.ratingsCount).Name("ratingsCount");
                Map(m => m.genres).Name("genres");
                Map(m => m.Price).Name("Price");

            }
        }

        public sealed class WarehouseBookMap : ClassMap<WarehouseBook>
        {
            public WarehouseBookMap()
            {
                // Явно вказуємо імена колонок CSV для мапінгу
                Map(m => m.Title).Name("Title");
                Map(m => m.description).Name("description");
                Map(m => m.authors).Name("authors");
                Map(m => m.image).Name("image");
                Map(m => m.previewLink).Name("previewLink");
                Map(m => m.publisher).Name("publisher");

                // Використовуємо кастомний конвертер для PublishedDate
                Map(m => m.publishedDate)
                .Name("publishedDate")
                    .TypeConverter<CustomYearOrDateTimeConverter>(); // Тут вказуємо наш кастомний конвертер

                Map(m => m.infoLink).Name("infoLink");
                Map(m => m.ratingsCount).Name("ratingsCount");
                Map(m => m.Price).Name("Price");

            }
        }

        public class CustomYearOrDateTimeConverter : ITypeConverter
        {
            public object ConvertFromString(string text, IReaderRow row, MemberMapData metadata)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return DateTime.MinValue; // або null, якщо ви хочете, щоб порожнє поле було null
                }

                // Якщо це просто рік (формат "yyyy")
                if (int.TryParse(text, out var year))
                {
                    return new DateTime(year, 1, 1); // Повертаємо 1 січня цього року
                }

                // Якщо це повна дата (формат "yyyy-MM-dd")
                if (DateTime.TryParse(text, out var date))
                {
                    return new DateTime(date.Year, 1, 1); // Повертаємо 1 січня року з повної дати
                }

                return DateTime.MinValue; // або null, якщо ви хочете обробити некоректний формат
            }

            public string ConvertToString(object value, IWriterRow row, MemberMapData metadata)
            {
                if (value is DateTime date)
                {
                    return date.Year.ToString(); // Повертаємо тільки рік
                }

                return string.Empty; // Якщо значення не є DateTime, повертаємо порожній рядок
            }
        }

    }
}
