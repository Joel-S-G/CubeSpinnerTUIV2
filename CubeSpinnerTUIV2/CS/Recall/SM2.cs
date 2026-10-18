//file for the sm2 algorithm implementation
using System;

namespace CubeSpinner
{
    public class SuperMemo
    {
        public int Repetitions { get; private set; } = 0;
        public double EaseFactor { get; set; } = 2.5;
        public TimeSpan Interval { get; private set; } = TimeSpan.Zero;
        public DateTime NextReview { get; private set; } = DateTime.UtcNow;

        public enum Rating
        {
            Easy = 4,
            Good = 3,
            Hard = 2, //succeeded but hard
            Again = 1, //failed
        }

        public void EaseCalc(Rating rating)
        {
            switch (rating)
            {
                case Rating.Again:
                    EaseFactor = Math.Max(1.3, EaseFactor - 0.2);
                    break;

                case Rating.Hard:
                    EaseFactor = Math.Max(1.3, EaseFactor - 0.15);
                    break;

                case Rating.Good:
                    EaseFactor = EaseFactor;
                    break;

                case Rating.Easy:
                    EaseFactor += 0.15;

                    break;

                default:
                    break;
            }
        }

        public void Review(Rating rating)
        {
            EaseCalc(rating);

            if (rating == Rating.Again)
            {
                Repetitions = 0;
                Interval = TimeSpan.FromMinutes(5);
                EaseCalc(rating);
            }
            else
            {
                Repetitions++;

                Interval = Repetitions switch
                {
                    1 => TimeSpan.FromMinutes(20),
                    2 => TimeSpan.FromMinutes(45),
                    _ => TimeSpan.FromMinutes(Interval.TotalHours * EaseFactor),
                };
            }

            NextReview = DateTime.UtcNow.Add(Interval);
        }
    }
}


//TODO
//[X] enums for Rating
//[] method to ratecard and behaviour if a card is rated
//[]some way to pass that to flashcard.cs to store
//[X] interval control using DateTime -> create timestamp at the time that the rating was given and compare it to the current time
