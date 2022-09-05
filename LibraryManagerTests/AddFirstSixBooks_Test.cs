using LibraryManagerTests;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace AddFirstSixBooks
{
    public class AddFirstSixBooks_Test
    {
        private RestClient client;
        private RestRequest request;
        private const string url = "http://localhost:9000/api/books";

        [SetUp]
        public void Setup()
        {
            client = new RestClient(url);
        }

        [Test]
        public void Test_AddFirstSixBooks()
        {
            //Arrange
            for (int num = 1; num < 7; num++)
            {
                this.request = new RestRequest(url);

                var body = new Book
                {
                    Id = num,
                    Title = "TestTitle" + num,
                    Author = "TestAuthor" + num,
                    Description = "TestDescription" + num
                };
                request.AddJsonBody(body);
                //Act
                var response = this.client.Execute(request, Method.Post);

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                var allBooks = this.client.Execute(request, Method.Get);
                var books = JsonSerializer.Deserialize<List<Book>>(allBooks.Content);
                var lastBook = books.Last();

                //Assert  
                Assert.That(lastBook.Id, Is.EqualTo(body.Id));
                Assert.That(lastBook.Title, Is.EqualTo(body.Title)); ;
            }
        }
    }
}