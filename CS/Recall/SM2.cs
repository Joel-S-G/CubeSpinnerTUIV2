//file for the sm2 algorithm implementation
using System;

namespace CubeSpinner
{
    public static class SuperMemo
    {
        public enum Rating
        {
            Easy = 4,
            Good = 3,
            Hard = 2, //succeeded but hard
            Again = 1, //failed
        }

        public static void EaseCalc(Flashcards card, Rating rating)
        {
            switch (rating)
            {
                case Rating.Again:
                    card.EaseFactor = Math.Max(1.3, card.EaseFactor - 0.2);
                    break;

                case Rating.Hard:
                    card.EaseFactor = Math.Max(1.3, card.EaseFactor - 0.15);
                    break;

                case Rating.Good:
                    card.EaseFactor = card.EaseFactor;
                    break;

                case Rating.Easy:
                    card.EaseFactor += 0.15;

                    break;

                default:
                    break;
            }
        }

        public static void Review(Flashcards card, Rating rating)
        {
            EaseCalc(card, rating);

            if (rating == Rating.Again)
            {
                card.Repetitions = 0;
                card.Interval = TimeSpan.FromMinutes(5);
            }
            else
            {
                card.Repetitions++;

                card.Interval = card.Repetitions switch
                {
                    1 => TimeSpan.FromMinutes(20),
                    2 => TimeSpan.FromMinutes(45),
                    _ => TimeSpan.FromMinutes(card.Interval.TotalMinutes * card.EaseFactor),
                };
            }

            card.NextReview = DateTime.UtcNow.Add(card.Interval);
        }

        public static void RestoreState(
            Flashcards card,
            int repetitions,
            double easeFactor,
            TimeSpan interval,
            DateTime nextReview
        )
        {
            card.Repetitions = repetitions;
            card.EaseFactor = easeFactor;
            card.Interval = interval;
            card.NextReview = nextReview;
        }
    }
}


//TODO
//[X] enums for Rating
//[] method to ratecard and behaviour if a card is rated
//[]some way to pass that to flashcard.cs to store
//[X] interval control using DateTime -> create timestamp at the time that the rating was given and compare it to the current time
