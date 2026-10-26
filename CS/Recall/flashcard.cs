using System;

namespace CubeSpinner
{
    public class Flashcards
    {
        public string? Name { get; set; }
        public string? Algorithm { get; set; }
        public string? AlgorithmType { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();

        //SM2 proprs
        public int Repetitions { get; set; } = 0;
        public double EaseFactor { get; set; } = 1.3;
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;
        public DateTime NextReview { get; set; } = DateTime.UtcNow;

        public SuperMemo scheduler { get; private set; } = new();

        public Flashcards(string? name, string? algorithm)
        {
            this.Name = name;
            this.Algorithm = algorithm;
        }

        public void RateCard(SuperMemo.Rating rating)
        {
            scheduler.Review(rating);
        }

        public bool isDue()
        {
            return DateTime.UtcNow >= scheduler.NextReview;
        }

        public void RestoreState(
            string name,
            string algorithm,
            string algorithmtype,
            string ID,
            int repetitions,
            double easeFactor,
            TimeSpan interval,
            DateTime nextReview
        )
        {
            Name = name;
            Algorithm = algorithm;
            AlgorithmType = algorithmtype;
            Id = Guid.Parse(ID);

            scheduler.RestoreState(repetitions, easeFactor, interval, nextReview);
        }
    }
}
