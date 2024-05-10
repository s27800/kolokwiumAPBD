using System.Transactions;
using kolokwium.Models.DTOs;
using kolokwium.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    
    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetAuthorsForBook(int id)
    {
        if (!await _booksRepository.DoesBookExist(id))
            return NotFound($"Book with ID = {id} does not exist");

        var authors = await _booksRepository.GetAuthors(id);

        return Ok(authors);
    }

    [HttpPost]
    [Route("with-scope")]
    public async Task<IActionResult> AddBook(AddBook addBook)
    {
        foreach (var author in addBook.Authors)
        {
            if (!await _booksRepository.DoesAuthorExist(author.Id))
                return NotFound($"Author with ID = {author.Id} does not exist");
        }

        var bookWithAuthors = new BookWithAuthors();
        int id;
        
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            id = await _booksRepository.AddBook(addBook);

            foreach (var author in addBook.Authors)
            {
                await _booksRepository.AddAuthorToBook(id, author);
            }

            scope.Complete();
        }

        bookWithAuthors.Id = id;
        bookWithAuthors.Title = addBook.Title;
        bookWithAuthors.Authors = addBook.Authors;

        return Created(Request.Path.Value ?? "api/books", bookWithAuthors);
    }
}