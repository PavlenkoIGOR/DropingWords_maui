using MauiLib1.Models;
using System.Text;

namespace dwWithEFAndDll.ViewModels
{
    public class WordAndTranslationsLP
    {
        public string? word { get; set; }
        public List<string> translations { get; set; }
        public string? translation { get; set; }
        public StringBuilder? translationsString { get; set; }
        public WordAndTranslationsLP()
        {
            translations = new List<string>();
            translationsString = new StringBuilder();
        }
    }
}
