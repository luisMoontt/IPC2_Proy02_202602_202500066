using System;

namespace Biblioteca.Models
{
    public class BookNode
    {
        public Book Book { get; set; }
        public BookNode Left { get; set; }
        public BookNode Right { get; set; }
        public int Height { get; set; }

        public BookNode(Book book)
        {
            this.Book = book;
            this.Left = null;
            this.Right = null;
            this.Height = 1; // Un nodo hoja nuevo inicia con altura 1
        }
    }

    public class CatalogBooks
    {
        private BookNode root;

        public CatalogBooks()
        {
            this.root = null;
        }

        // ==========================================
        // MÉTODOS AUXILIARES DE BALANCEO AVL
        // ==========================================
///////////////////////////////////////////////////////////////////////
        private int GetHeight(BookNode node)
        {
            return node == null ? 0 : node.Height;
        }

        private int GetBalanceFactor(BookNode node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        private int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        private BookNode RotateRight(BookNode y)
        {
            BookNode x = y.Left;
            BookNode T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        private BookNode RotateLeft(BookNode x)
        {
            BookNode y = x.Right;
            BookNode T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        // ==========================================
        // 1. INSERCIÓN DE LIBROS (ISBN ÚNICO)
        // ==========================================

        public bool Insert(Book book)
        {
            if (Search(book.getISBN()) != null)
            {
                Console.WriteLine($"[Error] El ISBN {book.getISBN()} ya existe en el sistema.");
                return false;
            }

            root = InsertRecursive(root, book);
            return true;
        }

        private BookNode InsertRecursive(BookNode node, Book book)
        {
            if (node == null)
            {
                return new BookNode(book);
            }

            if (book.getISBN() < node.Book.getISBN())
            {
                node.Left = InsertRecursive(node.Left, book);
            }
            else if (book.getISBN() > node.Book.getISBN())
            {
                node.Right = InsertRecursive(node.Right, book);
            }
            else
            {
                return node; 
            }

            node.Height = 1 + Max(GetHeight(node.Left), GetHeight(node.Right));

            int balance = GetBalanceFactor(node);

            // Caso Izquierda-Izquierda (LL)
            if (balance > 1 && book.getISBN() < node.Left.Book.getISBN())
            {
                return RotateRight(node);
            }

            // Caso Derecha-Derecha (RR)
            if (balance < -1 && book.getISBN() > node.Right.Book.getISBN())
            {
                return RotateLeft(node);
            }

            // Caso Izquierda-Derecha (LR)
            if (balance > 1 && book.getISBN() > node.Left.Book.getISBN())
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Caso Derecha-Izquierda (RL)
            if (balance < -1 && book.getISBN() < node.Right.Book.getISBN())
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        // ==========================================
        // 2. BÚSQUEDA DE LIBRO POR ISBN
        // ==========================================

        public Book Search(int isbn)
        {
            BookNode result = SearchRecursive(root, isbn);
            return result != null ? result.Book : null;
        }

        private BookNode SearchRecursive(BookNode node, int isbn)
        {
            if (node == null || node.Book.getISBN() == isbn)
            {
                return node;
            }

            if (isbn < node.Book.getISBN())
            {
                return SearchRecursive(node.Left, isbn);
            }

            return SearchRecursive(node.Right, isbn);
        }

        // ==========================================
        // 3. ELIMINACIÓN DE UN LIBRO POR ISBN
        // ==========================================

        public bool Delete(int isbn)
        {
            if (Search(isbn) == null)
            {
                Console.WriteLine($"[Error] No se encontró el libro con ISBN {isbn} para eliminar.");
                return false;
            }

            root = DeleteRecursive(root, isbn);
            return true;
        }

        private BookNode DeleteRecursive(BookNode node, int isbn)
        {
            if (node == null) return node;

            if (isbn < node.Book.getISBN())
            {
                node.Left = DeleteRecursive(node.Left, isbn);
            }
            else if (isbn > node.Book.getISBN())
            {
                node.Right = DeleteRecursive(node.Right, isbn);
            }
            else
            {
                // Nodo con un solo hijo o sin hijos
                if ((node.Left == null) || (node.Right == null))
                {
                    BookNode temp = node.Left != null ? node.Left : node.Right;

                    if (temp == null)
                    {
                        temp = node;
                        node = null;
                    }
                    else
                    {
                        node = temp;
                    }
                }
                else
                {
                    // Nodo con dos hijos: Obtener el sucesor en inorden (mínimo del subárbol derecho)
                    BookNode temp = GetMinNode(node.Right);
                    node.Book = temp.Book;
                    node.Right = DeleteRecursive(node.Right, temp.Book.getISBN());
                }
            }

            if (node == null) return node;

            // Actualizar altura
            node.Height = 1 + Max(GetHeight(node.Left), GetHeight(node.Right));

            // Balancear
            int balance = GetBalanceFactor(node);

            if (balance > 1 && GetBalanceFactor(node.Left) >= 0)
                return RotateRight(node);

            if (balance > 1 && GetBalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            if (balance < -1 && GetBalanceFactor(node.Right) <= 0)
                return RotateLeft(node);

            if (balance < -1 && GetBalanceFactor(node.Right) > 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        // ==========================================
        // 4. LIBRO CON MENOR Y MAYOR ISBN
        // ==========================================

        public Book GetMinBook()
        {
            if (root == null) return null;
            return GetMinNode(root).Book;
        }

        private BookNode GetMinNode(BookNode node)
        {
            BookNode current = node;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current;
        }

        public Book GetMaxBook()
        {
            if (root == null) return null;
            BookNode current = root;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current.Book;
        }

        // ==========================================
        // 5. IMPRIMIR EN ORDEN ASCENDENTE (INORDEN)
        // ==========================================

        public void PrintAscending()
        {
            Console.WriteLine("\n=== LIBROS EN ORDEN ASCENDENTE DE ISBN ===");
            InOrderRecursive(root);
        }

        private void InOrderRecursive(BookNode node)
        {
            if (node != null)
            {
                InOrderRecursive(node.Left);
                Console.WriteLine($"[ISBN: {node.Book.getISBN()}] - '{node.Book.getTitle()}' de {node.Book.getAuthor()} (Cat: {node.Book.getCategory()})");
                InOrderRecursive(node.Right);
            }
        }

        // ==========================================
        // 6. LISTAR LIBROS POR CATEGORÍA
        // ==========================================

        public void ListByCategory(string category)
        {
            Console.WriteLine($"\n=== LIBROS DE LA CATEGORÍA: '{category}' ===");
            bool found = FilterByCategoryRecursive(root, category);
            if (!found)
            {
                Console.WriteLine("No se encontraron libros asociados a esta categoría.");
            }
        }

        private bool FilterByCategoryRecursive(BookNode node, string category)
        {
            if (node == null) return false;

            bool leftFound = FilterByCategoryRecursive(node.Left, category);

            bool currentFound = false;
            if (node.Book.getCategory().Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[ISBN: {node.Book.getISBN()}] - '{node.Book.getTitle()}' de {node.Book.getAuthor()}");
                currentFound = true;
            }

            bool rightFound = FilterByCategoryRecursive(node.Right, category);

            return leftFound || currentFound || rightFound;
        }
    }
}