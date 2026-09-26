using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Services;

namespace Biblioteca.Controllers
{
    [Route("[controller]")]
    public class LoadDataController : Controller
    {
        private readonly LibraryService _libraryService;

        public LoadDataController(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpPost("UploadXml")]
        public IActionResult UploadXml(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Por favor selecciona o arrastra un archivo XML válido.";
                return RedirectToPage("/LoadCatalogPage");
            }

            try
            {
                using var stream = xmlFile.OpenReadStream();
                XDocument doc = XDocument.Load(stream);

                int categoriesAdded = 0;
                int booksAdded = 0;
                int booksSkipped = 0;

                // 1. Procesar Lista de Categorías
                var categorias = doc.Root?.Element("listaCategorias")?.Elements("categoria");
                if (categorias != null)
                {
                    foreach (var catElem in categorias)
                    {
                        string categoryName = catElem.Value.Trim();
                        string parentName = catElem.Attribute("padre")?.Value?.Trim();

                        if (!_libraryService.CategoryExists(categoryName))
                        {
                            Category newCategory = new Category(categoryName);

                            if (string.IsNullOrEmpty(parentName))
                            {
                                // Agregar a la raíz principal
                                _libraryService.CatalogCategories.addFirstLevelCategory(newCategory);
                                categoriesAdded++;
                            }
                            else
                            {
                                // Buscar padre y agregar
                                Category parentCategory = _libraryService.CatalogCategories.getRoot().findCategory(parentName);
                                if (parentCategory != null)
                                {
                                    parentCategory.addSon(newCategory);
                                    categoriesAdded++;
                                }
                                else
                                {
                                    // Si no se encuentra el padre especificado, se agrega en la raíz
                                    _libraryService.CatalogCategories.addFirstLevelCategory(newCategory);
                                    categoriesAdded++;
                                }
                            }
                        }
                    }
                }

                // 2. Procesar Lista de Libros
                var libros = doc.Root?.Element("listaLibros")?.Elements("libro");
                if (libros != null)
                {
                    foreach (var libroElem in libros)
                    {
                        int isbn = int.Parse(libroElem.Element("ISBN")?.Value ?? "0");
                        string titulo = libroElem.Element("titulo")?.Value ?? "";
                        string autor = libroElem.Element("autor")?.Value ?? "";
                        string categoria = libroElem.Element("categoria")?.Value ?? "";

                        // Validación ifExists previo a insertar
                        if (!_libraryService.BookExists(isbn))
                        {
                            Book book = new Book(isbn, titulo, autor, categoria);
                            if (_libraryService.CatalogBooks.Insert(book))
                            {
                                booksAdded++;
                            }
                        }
                        else
                        {
                            booksSkipped++;
                        }
                    }
                }
                Console.WriteLine("asasaaaaaaaaaaaa");
                TempData["SuccessMessage"] = $"Proceso completado: {categoriesAdded} categorías agregadas, {booksAdded} libros agregados ({booksSkipped} duplicados omitidos).";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al procesar el archivo XML: {ex.Message}";
            }

            return RedirectToPage("/LoadCatalogPage");
        }
    }
}