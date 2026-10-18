using System;

namespace CubeSpinner
{
    class Flashcards
    {
        public string? name { get; set; }
        public string? algorithm { get; set; }
        public SuperMemo scheduler = new();

        public void Store() { }

        public void Read() { }

        public void Write() { }

        public void Delete() { }
    }
}
