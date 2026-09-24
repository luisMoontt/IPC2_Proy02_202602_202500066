using System;

namespace Biblioteca.Models
{
    public class Book
    {
        private int ISBN;
        private string title;
        private string author;
        private string category;

        public Book(int ISBN, string title, string author, string category)
        {
            this.ISBN = ISBN;
            this.title = title;
            this.author = author;
            this.category = category;
        }

        public int getISBN(){return ISBN; }
        public void setISBN(int IBN){ this.ISBN = ISBN; }
        
        public string getTitle() { return title; }
        public void setTitle(string title) { this.title = title; }

        public string getAuthor() { return author; }
        public void setAuthor(string author) { this.author = author; }

        public string getCategory() { return category; }
        public void setCategory(string category) { this.category = category; }
    }
}


