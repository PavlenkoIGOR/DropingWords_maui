using MauiLib1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dwWithEFAndDll.ViewModels
{
    public class WordsWithTranslationsViewModel
    {
        
        public List<Word> wordsInVM { get; set; } = null!;
        public List<Translation> translationInVM { get; set; } = null!;
        public bool isActive { get; set; } = true;

        public WordsWithTranslationsViewModel()
        {
            wordsInVM = new List<Word>();
            translationInVM = new List<Translation>();
        }
    }
}
