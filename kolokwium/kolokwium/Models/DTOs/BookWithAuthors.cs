using System.Collections;

namespace kolokwium.Models.DTOs;

public class BookWithAuthors
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Author> Authors { get; set; }
}