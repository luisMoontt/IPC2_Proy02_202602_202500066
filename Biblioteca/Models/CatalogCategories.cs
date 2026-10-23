using System;
using System.Text;

namespace Biblioteca.Models
{
    public class CatalogCategories
    {
        private Category root;

        public CatalogCategories()
        {
            this.root = new Category("Libro");
            this.root.setParent(null);
        }

        public Category getRoot() { return root; }

        public bool addFirstLevelCategory(Category category)
        {
            return root.addSon(category);
        }

        public void listAllCategories()
        {
            Console.WriteLine("=== CATALOGO DE CATEGORÍAS ===");
            root.printSubtree(0);
        }

        public void searchAndShowHierarchy(string categoryName)
        {
            Category found = root.findCategory(categoryName);
            if (found != null)
            {
                found.printChildrenAndGrandchildren();
            }
            else
            {
                Console.WriteLine("\nCategoria '" + categoryName + "' no encontrada.");
            }
        }
        public bool Exists(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) 
                return false;

            return root != null && root.findCategory(categoryName) != null;
        }

        public string GetFullTreeText(CatalogBooks books, bool includeBooks)
        {
            StringBuilder sb = new StringBuilder();
            Category actual = null;
            while ((actual = root.GetNextAlphabetical(actual?.getName())) != null)
            {
                actual.AppendTree(sb, 0, books, includeBooks);
            }
            return sb.ToString();
        }

        public string GetSubtreeText(string categoryName, CatalogBooks books, bool includeBooks)
        {
            Category encontrada = root.findCategory(categoryName);
            if (encontrada == null) return null;

            StringBuilder sb = new StringBuilder();
            encontrada.AppendTree(sb, 0, books, includeBooks);
            return sb.ToString();
        }
    }
}