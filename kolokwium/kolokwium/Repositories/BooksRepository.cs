using System.Data.SqlClient;
using kolokwium.Models;
using kolokwium.Models.DTOs;

namespace kolokwium.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM Books WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<BookWithAuthors> GetAuthors(int id)
    {
        var query = @"SELECT 
                        Books.PK AS BookID
                        Books.Title AS Title
                        Authors.*
                      FROM Books 
                        INNER JOIN Books_Authors ON Books.PK = Books_Authors.FK_book 
                        INNER JOIN Authors ON Books_Authors.FK_author = Authors.PK 
                      WHERE Books.PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var bookIdOrdinal = reader.GetOrdinal("BookID");
        var bookTitleOrdinal = reader.GetOrdinal("Title");
        var authorIdOrdinal = reader.GetOrdinal("PK");
        var authorFirstNameOrdinal = reader.GetOrdinal("first_name");
        var authorLastNameOrdinal = reader.GetOrdinal("last_name");

        BookWithAuthors bookDTO = null;

        while (await reader.ReadAsync())
        {
            if (bookDTO is not null)
            {
                bookDTO.Authors.Add(new Author()
                {
                    Id = reader.GetInt32(bookIdOrdinal),
                    FirstName = reader.GetString(authorFirstNameOrdinal),
                    LastName = reader.GetString(authorLastNameOrdinal)
                });
            }
            else
            {
                bookDTO = new BookWithAuthors()
                {
                    Id = reader.GetInt32(bookIdOrdinal),
                    Title = reader.GetString(bookTitleOrdinal),
                    Authors = new List<Author>()
                    {
                        new Author()
                        {
                            Id = reader.GetInt32(authorIdOrdinal),
                            FirstName = reader.GetString(authorFirstNameOrdinal),
                            LastName = reader.GetString(authorLastNameOrdinal)
                        }
                    }
                };
            }
        }

        if (bookDTO is null)
            throw new Exception();

        return bookDTO;
    }

    public async Task<bool> DoesAuthorExist(int id)
    {
        var query = "SELECT 1 FROM Authors WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<int> AddBook(AddBook addBook)
    {
        var query = "INSERT INTO Book VALUES Title = @title";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@title", addBook.Title);

        await connection.OpenAsync();
        
        
        return 1;
    }

    public async Task AddAuthorToBook(int id, Author author)
    {

        
    }
}