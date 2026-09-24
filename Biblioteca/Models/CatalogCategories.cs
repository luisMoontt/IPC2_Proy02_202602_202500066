using System;

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
    }
}