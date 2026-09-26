using Biblioteca.Models;

namespace Biblioteca.Services
{
    public class LibraryService
    {
        public CatalogBooks CatalogBooks { get; }
        public CatalogCategories CatalogCategories { get; }

        public LibraryService()
        {
            CatalogBooks = new CatalogBooks();
            CatalogCategories = new CatalogCategories();
        }

        public bool CategoryExists(string name)
        {
            return CatalogCategories.Exists(name);
        }

        public bool BookExists(int isbn)
        {
            return CatalogBooks.Exists(isbn);
        }
    }
}