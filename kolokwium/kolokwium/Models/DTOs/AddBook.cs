using System.Collections;

namespace kolokwium.Models.DTOs;

public class AddBook
{
    public string Title { get; set; }
    public List<Author> Authors { get; set; }
}