namespace Biblioteca_API.DTOs
{
    public class LibroDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }
        public string Description { get; set; }

        public List<int> AutorIds { get; set; } = new();

        public List<int> CategoriaIds { get; set; } = new();
    }
}