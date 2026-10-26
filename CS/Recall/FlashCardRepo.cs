using System;
using Microsoft.Data.Sqlite;

namespace CubeSpinnerubespinner
{
    public class FlashcarDB
    {
        private const string connectstring = "data source = flashcard.db";

        public static void initilise()
        {
            using var connection = new SqliteConnection(connectstring);
            connection.Open();

            string sql =
                @"
                  CREATE TABLE IF NOT EXISTS flashcards ( 
                    id TEXT PRIMARY KEY,
                    name TEXT,
                    algorithm TEXT,
                    algorithmtype TEXT,
                    repetitions INTEGER,
                    EF REAL,
                    interval REAL,
                    nextreview REAL
                    );";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        public static void addcards(Flashcards flashcards)
        {
            using var connection = new SqliteConnection(connectstring);
            connection.Open();

            string sql =
                @"
                  insert into flashcard(
                   id,
                   name,
                   algorithm,
                   algorithmtype,
                   repetitions,
                   ef,
                   interval,
                   nextreview
                      )
                  values(
                    @id,
                    @name,
                    @algorithm,
                    @algorithmtype,
                    @repetitions,
                    @ef,
                    @interval,
                    @nextreview
                      )";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            command.Parameters.AddWithValue("@id", flashcards.id.ToString());
            command.Parameters.AddWithValue("@name", flashcards.name);
            command.Parameters.AddWithValue("@algorithm", flashcards.algorithm);
            command.Parameters.AddWithValue("@algorithmtype", flashcards.algorithmtype);
            command.Parameters.AddWithValue("@repetitions", flashcards.repetitions);
            command.Parameters.AddWithValue("@ef", flashcards.ef);
            command.Parameters.AddWithValue("@interval", flashcards.interval.TotalMinutes);
            command.Parameters.AddWithValue("@nextreview", flashcards.nextreview.Ticks);

            command.ExecuteNonQuery();
        }

        public static void removecard(Flashcards flashcards)
        {
            using var connection = new SqliteConnection(connectstring);
            connection.Open();
            string sql =
                @"
                     insert into flashcard(
                      id,
                      name,
                      algorithm,
                      algorithmtype,
                      repetitions,
                      ef,
                      interval,
                      nextreview
                         )
                     values(
                       @id,
                       @name,
                       @algorithm,
                       @algorithmtype,
                       @repetitions,
                       @ef,
                       @interval,
                       @nextreview
                         )";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            command.Parameters.AddWithValue("@id", flashcards.id.ToString());
            command.Parameters.AddWithValue("@name", flashcards.name);
            command.Parameters.AddWithValue("@algorithm", flashcards.algorithm);
            command.Parameters.AddWithValue("@algorithmtype", flashcards.algorithmtype);
            command.Parameters.AddWithValue("@repetitions", flashcards.repetitions);
            command.Parameters.AddWithValue("@ef", flashcards.ef);
            command.Parameters.AddWithValue("@interval", flashcards.interval.TotalMinutes);
            command.Parameters.AddWithValue("@nextreview", flashcards.nextreview.Ticks);

            command.ExecuteNonQuery();
        }

        public static void updatecard(Flashcards flashcards) { }
    }
}

//TODO
//find and add the following to flashcard: Repetitions, EaseFactor, Interval, NextReview
//complete sql queries for remove and update
//
