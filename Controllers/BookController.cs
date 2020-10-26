using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using asn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

public class BookController : Controller {
    const string BASE_URL = "https://www.googleapis.com/books/v1/volumes?q=harry+potter/";
    private readonly ILogger<BookController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    public ArrayList Books = new ArrayList();
    public bool GetBooksError { get; private set; }

  public BookController(ILogger<BookController> logger, IHttpClientFactory clientFactory)
  {
        _logger = logger;
        _clientFactory = clientFactory;
  }

[Authorize]
    public async Task<IActionResult> Index()
    {
        HttpClient client = new HttpClient();
        var request = BASE_URL;
        var response = await client.GetAsync(request);

        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            dynamic myList = JObject.Parse(content);
            var items = myList.items;
            foreach(var item in items)
            {
                Book b = new Book();
                b.title = item.volumeInfo.title;
                b.smallThumbnail = item.volumeInfo.imageLinks.smallThumbnail;
                b.authors = item.volumeInfo.authors.ToString();
                b.publisher = item.volumeInfo.publisher;
                b.publishedDate = item.volumeInfo.publishedDate;
                b.description = item.volumeInfo.description;
                b.ISBN10 = item.volumeInfo.industryIdentifiers[1].identifier;
                Books.Add(b);
            }
        } else {
            GetBooksError = true;
            Books.Clear();
        }
        ViewBag.Books = Books;
        return View(Books);
    }

[Authorize]
    public async Task<IActionResult> Details(string title, string smallThumbnail, string authors, string publisher, string publishedDate, string description, string ISBN10) {
        Book b = new Book();
        b.title = title;
        b.authors = authors;
        b.publisher = publisher;
        b.publishedDate = publishedDate;
        b.smallThumbnail = smallThumbnail;
        b.description = description;
        b.ISBN10 = ISBN10;
        
        ViewBag.Data = b;
        return View(Books);  
    }
}
