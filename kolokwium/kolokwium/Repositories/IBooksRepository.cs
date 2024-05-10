using System.Collections;
using kolokwium.Models;
using kolokwium.Models.DTOs;

namespace kolokwium.Repositories;

public interface IBooksRepository
{
    public Task<bool> DoesBookExist(int id);
    public Task<BookWithAuthors> GetAuthors(int id);
    public Task<bool> DoesAuthorExist(int id);
    public Task<int> AddBook(AddBook addBook);
    public Task AddAuthorToBook(int id, Author authors);
}