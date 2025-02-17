﻿using MauiLib1.Models;
using System.Text;

namespace dwWithEFAndDll.ViewModels
{
    public class WordAndTranslations
    {
        public Word? word { get; set; }
        public ICollection<Translation> translations { get; set; }
        public StringBuilder? translationsString { get; set; }
        public WordAndTranslations()
        {
            translations = new List<Translation>();
            translationsString = new StringBuilder();
        }
    }
}
