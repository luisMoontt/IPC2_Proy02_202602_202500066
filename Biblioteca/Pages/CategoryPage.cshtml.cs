using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteca.Services;

namespace Biblioteca.Pages;

public class CategoryModel : PageModel
{
    private readonly LibraryService _service;

    public CategoryModel(LibraryService service)
    {
        _service = service;
    }

    [BindProperty]
    public string NuevoNombre { get; set; }

    [BindProperty]
    public string NuevoPadre { get; set; }

    public string MensajeCrear { get; set; }

    [BindProperty]
    public string NombreBuscado { get; set; }

    public string MensajeAccion { get; set; }

    public string TextoArbol { get; private set; }

    public void OnGet()
    {
        CargarArbolCompleto();
    }

    public void OnPostCrear()
    {
        string error = _service.CreateCategory(NuevoNombre, NuevoPadre);

        if (error == null)
        {
            MensajeCrear = $"Categoría '{NuevoNombre}' agregada correctamente.";
            NuevoNombre = string.Empty;
            NuevoPadre = string.Empty;
        }
        else
        {
            MensajeCrear = error;
        }

        CargarArbolCompleto();
    }

    public void OnPostListarSubcategorias()
    {
        TextoArbol = _service.ListSubcategoriesText(NombreBuscado);
    }

    public void OnPostListarLibros()
    {
        TextoArbol = _service.ListBooksInSubcategoryText(NombreBuscado);
    }

    private void CargarArbolCompleto()
    {
        TextoArbol = _service.GetFullCategoryTreeText();
    }
}