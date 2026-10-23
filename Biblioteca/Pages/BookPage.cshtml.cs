using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteca.Models;
using Biblioteca.Services;

namespace Biblioteca.Pages;

public class BookModel : PageModel
{
    private readonly LibraryService _service;

    public BookModel(LibraryService service)
    {
        _service = service;
    }

    [BindProperty]
    public int NuevoIsbn { get; set; }

    [BindProperty]
    public string NuevoTitulo { get; set; }

    [BindProperty]
    public string NuevoAutor { get; set; }

    [BindProperty]
    public string NuevoCategoria { get; set; }

    public string MensajeCrear { get; set; }

    [BindProperty]
    public int IsbnBuscado { get; set; }

    public string MensajeAccion { get; set; }

    public string Modo { get; private set; } = "arbol";

    public string ArbolSvg { get; private set; }

    public Book LibroMostrado { get; private set; }

    public void OnGet()
    {
        CargarArbol();
    }

    public void OnPostCrear()
    {
        string error = _service.CreateBook(NuevoIsbn, NuevoTitulo, NuevoAutor, NuevoCategoria);

        if (error == null)
        {
            MensajeCrear = $"Libro '{NuevoTitulo}' agregado correctamente.";
            NuevoIsbn = 0;
            NuevoTitulo = string.Empty;
            NuevoAutor = string.Empty;
            NuevoCategoria = string.Empty;
        }
        else
        {
            MensajeCrear = error;
        }

        CargarArbol();
    }

    public void OnPostBuscar()
    {
        LibroMostrado = _service.SearchBook(IsbnBuscado);

        if (LibroMostrado == null)
        {
            MensajeAccion = $"No se encontró ningún libro con ISBN {IsbnBuscado}.";
            CargarArbol();
        }
        else
        {
            Modo = "libro";
        }
    }

    public void OnPostEliminar()
    {
        string error = _service.DeleteBook(IsbnBuscado);
        MensajeAccion = error ?? $"Libro con ISBN {IsbnBuscado} eliminado correctamente.";
        CargarArbol();
    }

    public void OnPostMin()
    {
        LibroMostrado = _service.GetMinBook();

        if (LibroMostrado == null)
        {
            MensajeAccion = "El catálogo de libros está vacío.";
            CargarArbol();
        }
        else
        {
            Modo = "libro";
        }
    }

    public void OnPostMax()
    {
        LibroMostrado = _service.GetMaxBook();

        if (LibroMostrado == null)
        {
            MensajeAccion = "El catálogo de libros está vacío.";
            CargarArbol();
        }
        else
        {
            Modo = "libro";
        }
    }

    public void OnPostMostrarTodo()
    {
        CargarArbol();
    }

    private void CargarArbol()
    {
        Modo = "arbol";
        ArbolSvg = _service.GetBookTreeSvg();
    }
}