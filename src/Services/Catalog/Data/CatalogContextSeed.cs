using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Data
{
    public class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Book> bookCollection)
        {
            bool existProduct = bookCollection.Find(p => true).Any();
            if (!existProduct)
            {
                bookCollection.InsertManyAsync(GetPreconfiguredProducts());
            }
        }

        private static IEnumerable<Book> GetPreconfiguredProducts()
        {
            return new List<Book>()
            {
                new Book()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Title = "Аутсайдер",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    Genre = {"detective", "mystery", "horror" },
                    Price = 950.00M,
                    PublicationDate = new DateTime(2018, 5, 22),
                    PublisherName = "Книжковий клуб «Клуб Сімейного Дозвілля»",
                    AuthorName = "Стівен Кінг",
                    NumberOfPages = 592,
                    LanguageName = "uk",

                },
                new Book()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    Title = "Остання миля",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    Genre = {"detective", "literaly"},
                    Price = 1220.00M,
                    PublicationDate = new DateTime(2018),
                    PublisherName = "КМ-БУКС",
                    AuthorName = "Девід Балдаччі",
                    NumberOfPages = 552,
                    LanguageName = "uk",
                },
                new Book()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    Title = "Дом дивних дітей",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    Genre = {"mystery" },
                    Price = 950.00M,
                    PublicationDate = new DateTime(2011, 6, 7),
                    PublisherName = "Apple Books",
                    AuthorName = "Ренсом Рігс",
                    NumberOfPages = 612,
                    LanguageName = "eng",
                },
            };
        }
    }
}
