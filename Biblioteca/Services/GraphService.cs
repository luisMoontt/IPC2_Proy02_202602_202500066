using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Biblioteca.Models;

namespace Biblioteca.Services
{

    public class GraphService
    {
        private readonly string _dataFolder;

        public GraphService(IWebHostEnvironment env)
        {
            _dataFolder = Path.Combine(env.ContentRootPath, "Data");
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }
        }

        public void SaveBookTreePdf(CatalogBooks catalogo)
        {
            string dot = BuildBookDot(catalogo);
            string dotPath = Path.Combine(_dataFolder, "BookTree.dot");
            string pdfPath = Path.Combine(_dataFolder, "BookTree.pdf");

            File.WriteAllText(dotPath, dot);
            RunGraphvizToFile(dotPath, pdfPath, "pdf");
        }

        public string GetBookTreeSvg(CatalogBooks catalogo)
        {
            string dot = BuildBookDot(catalogo);
            return RunGraphvizToString(dot, "svg");
        }

        private string BuildBookDot(CatalogBooks catalogo)
        {
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph AVL {");
            dot.AppendLine("    bgcolor=transparent;");
            dot.AppendLine("    node [shape=circle, style=filled, fillcolor=white, color=black, fontsize=10, fixedsize=true, width=0.9];");
            dot.AppendLine("    edge [color=gray40];");
            catalogo.AppendDot(dot);
            dot.AppendLine("}");
            return dot.ToString();
        }

        public void SaveCategoryTreePdf(CatalogCategories catalogo)
        {
            string dot = BuildCategoryDot(catalogo);
            string dotPath = Path.Combine(_dataFolder, "CategoryTree.dot");
            string pdfPath = Path.Combine(_dataFolder, "CategoryTree.pdf");

            File.WriteAllText(dotPath, dot);
            RunGraphvizToFile(dotPath, pdfPath, "pdf");
        }

        private string BuildCategoryDot(CatalogCategories catalogo)
        {
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph Categorias {");
            dot.AppendLine("    bgcolor=transparent;");
            dot.AppendLine("    node [shape=circle, style=filled, fillcolor=white, color=black, fontsize=10];");
            dot.AppendLine("    edge [color=gray40];");
            catalogo.getRoot().AppendDot(dot);
            dot.AppendLine("}");
            return dot.ToString();
        }


        private void RunGraphvizToFile(string dotPath, string outputPath, string formato)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dot",
                    Arguments = $"-T{formato} \"{dotPath}\" -o \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo ejecutar Graphviz: " + ex.Message);
            }
        }

        private string RunGraphvizToString(string dotContenido, string formato)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dot",
                    Arguments = $"-T{formato}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.StandardInput.Write(dotContenido);
                    process.StandardInput.Close();
                    string salida = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return salida;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo ejecutar Graphviz: " + ex.Message);
                return null;
            }
        }
    }
}