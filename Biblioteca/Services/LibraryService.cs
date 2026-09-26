using Biblioteca.Models;

namespace Biblioteca.Services
{

    public class LibraryService
    {
        public CatalogBooks CatalogBooks { get; }
        public CatalogCategories CatalogCategories { get; }

        private readonly GraphService _graphService;

        public LibraryService(GraphService graphService)
        {
            CatalogBooks = new CatalogBooks();
            CatalogCategories = new CatalogCategories();
            _graphService = graphService;
        }

        public bool CategoryExists(string name) => CatalogCategories.Exists(name);
        public bool BookExists(int isbn) => CatalogBooks.Exists(isbn);
////////////////////////////////////////////////////////////////////////////////////////////
        public string CreateBook(int isbn, string title, string author, string category)
        {
            if (isbn <= 0)
                return "El ISBN debe ser un número positivo.";

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(category))
                return "Todos los campos son obligatorios.";

            if (BookExists(isbn))
                return $"Ya existe un libro con el ISBN {isbn}.";

            Book book = new Book(isbn, title.Trim(), author.Trim(), category.Trim());
            CatalogBooks.Insert(book);
            RefreshBookGraph();
            return null;
        }

        public Book SearchBook(int isbn) => CatalogBooks.Search(isbn);


        public string DeleteBook(int isbn)
        {
            if (!CatalogBooks.Delete(isbn))
                return $"No se encontró ningún libro con ISBN {isbn}.";

            RefreshBookGraph();
            return null;
        }

        public Book GetMinBook() => CatalogBooks.GetMinBook();

        public Book GetMaxBook() => CatalogBooks.GetMaxBook();


        public string GetBookTreeSvg() => _graphService.GetBookTreeSvg(CatalogBooks);

        private void RefreshBookGraph() => _graphService.SaveBookTreePdf(CatalogBooks);

//////////////////////////////////////////////////////////////////////////////////////////

        public string CreateCategory(string name, string parentName)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "El nombre de la categoría es obligatorio.";

            name = name.Trim();

            if (CategoryExists(name))
                return $"La categoría '{name}' ya existe en el sistema.";

            Category nueva = new Category(name);

            if (string.IsNullOrWhiteSpace(parentName))
            {
                CatalogCategories.addFirstLevelCategory(nueva);
            }
            else
            {
                Category padre = CatalogCategories.getRoot().findCategory(parentName.Trim());
                if (padre == null)
                    return $"La categoría padre '{parentName}' no existe.";

                padre.addSon(nueva);
            }

            RefreshCategoryGraph();
            return null;
        }

        public string ListSubcategoriesText(string name)
        {
            string resultado = CatalogCategories.GetSubtreeText(name, null, false);
            return resultado ?? $"La categoría '{name}' no fue encontrada.";
        }


        public string ListBooksInSubcategoryText(string name)
        {
            string resultado = CatalogCategories.GetSubtreeText(name, CatalogBooks, true);
            return resultado ?? $"La categoría '{name}' no fue encontrada.";
        }

        public string GetFullCategoryTreeText() => CatalogCategories.GetFullTreeText(null, false);

        private void RefreshCategoryGraph() => _graphService.SaveCategoryTreePdf(CatalogCategories);

        public void RefreshGraphs()
        {
            RefreshBookGraph();
            RefreshCategoryGraph();
        }
    }
}