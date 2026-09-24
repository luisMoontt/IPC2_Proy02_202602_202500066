using System;

namespace Biblioteca.Models
{
    public class Category
    {
        private string name;
        private Category parent;
        private Category[] sons; 
        private int count;      

        public Category(string name)
        {
            this.name = name;
            this.parent = null;
            this.sons = new Category[25]; 
            this.count = 0;
        }

        public string getName() { return name; }
        public void setName(string name) { this.name = name; }

        public Category getParent() { return parent; }
        public void setParent(Category parent) { this.parent = parent; }

        public Category[] getSons() { return sons; }


        public bool addSon(Category son)
        {
            if (count >= 25)
            {
                Console.WriteLine("Capacidad máxima de 25 subcategorias alcanzada para: " + this.name);
                return false;
            }

            son.setParent(this);
            sons[count] = son;
            count++;
            return true;
        }

        public void printSubtree(int depth)
        {
            string indent = "";
            for (int i = 0; i < depth; i++)
            {
                indent += "    ";
            }

            Console.WriteLine(indent + "- " + this.name);

            for (int i = 0; i < sons.Length; i++)
            {
                if (sons[i] != null)
                {
                    sons[i].printSubtree(depth + 1);
                }
            }
        }

        public Category findCategory(string searchName)
        {
            if (this.name.Equals(searchName, StringComparison.OrdinalIgnoreCase))
            {
                return this;
            }

            for (int i = 0; i < sons.Length; i++)
            {
                if (sons[i] != null)
                {
                    Category found = sons[i].findCategory(searchName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        public void printChildrenAndGrandchildren()
        {
            Console.WriteLine("\n--- Subcategorias de " + this.name + " ---");

            for (int i = 0; i < sons.Length; i++)
            {
                if (sons[i] != null)
                {
                    Console.WriteLine("- " + sons[i].getName());

                    Category[] grandchildren = sons[i].getSons();
                    for (int j = 0; j < grandchildren.Length; j++)
                    {
                        if (grandchildren[j] != null)
                        {
                            Console.WriteLine("    - " + grandchildren[j].getName());
                        }
                    }
                }
            }
        }
    }
}