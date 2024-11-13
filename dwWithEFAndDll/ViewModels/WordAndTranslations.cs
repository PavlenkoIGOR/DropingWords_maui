using MauiLib1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dwWithEFAndDll.ViewModels
{
    public class WordAndTranslations
    {
        public Word? word { get; set; }
        public List<Translation> translations { get; set; }
        public WordAndTranslations()
        {
            translations = new List<Translation>();
        }
    }
}
