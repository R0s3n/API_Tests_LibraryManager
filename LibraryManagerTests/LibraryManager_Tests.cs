using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace LibraryManagerTests
{
    public class LibraryManager_Tests
    {
        private RestClient client;
        private RestRequest request;
        private const string url = "http://localhost:9000/api/books";
        private const int id = 7;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(url);
        }

        [Test]
        public void Test_AddABook_InvalidData_NoTitle()
        {
            //Arrange
            this.request = new RestRequest(url);
            int rnd = new Random().Next(99);
            var num = id + rnd;
            var body = new Book
            {
                Id = num,
                Author = "TestAuthor" + num,
                Description = "TestDescription" + num
            };
            request.AddJsonBody(body);
            //Act
            var response = this.client.Execute(request, Method.Post);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void Test_GetAllBooks_CheckFirstBook()
        {
            //Arrange
            this.request = new RestRequest(url);

            //Act
            var response = this.client.Execute(request);
            var books = JsonSerializer.Deserialize<List<Book>>(response.Content);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(books, Is.Not.Empty);
            Assert.That(books[0].Title, Is.EqualTo("TestTitle1"));
        }

        [Test]
        public void Test_GetABookById_CheckResult()
        {
            //Arrange
            this.request = new RestRequest(url + "/6");

            //Act
            var response = this.client.Execute(request);
            var book = JsonSerializer.Deserialize<Book>(response.Content);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(book.Title, Is.EqualTo("TestTitle6"));
            Assert.That(book.Id, Is.EqualTo(6));
        }

        [Test]
        public void Test_AddABook_ValidData()
        {
            //Arrange
            this.request = new RestRequest(url);
            int rnd = new Random().Next(50);
            var num = id + rnd;
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
            Assert.That(lastBook.Title, Is.EqualTo(body.Title));;
        }

        [Test]
        public void Test_AddABook_ValidData_NoDescription()
        {
            //Arrange
            this.request = new RestRequest(url);
            int rnd = new Random().Next(50);
            var num = id + rnd;
            var body = new Book
            {
                Id = num,
                Title = "TestTitle" + num,
                Author = "TestAuthor" + num,
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

        [Test]
        public async Task Test_UpdateBook()
        {
            //Arrange
            this.request = new RestRequest(url + "/5");
            var body = new
            {
                Id = 5,
                Title = "TestTitleChanged",
                Author = "TestAuthorChanged",
                Description = "TestDescriptionChanged"
            };
            request.AddJsonBody(body);
            //Act
            var response = await client.ExecuteAsync(request, Method.Put);
            var book = JsonSerializer.Deserialize<Book>(response.Content);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(book.Id, Is.EqualTo(5));
            Assert.That(book.Title, Is.EqualTo(body.Title));
        }

        [Test]
        public void Test_DeleteABook()
        {
            //Arrange
            this.request = new RestRequest(url + "/2");

            //Act
            var response = this.client.Execute(request, Method.Delete);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}