using System;

namespace CubeSpinner
{
    public class SolveRepo
    {
        private const string connectstring = "data source = solves.db";

        public static void Initialise()
        {
            using var connection = new SqliteConnection(connectstring);
            connection.Open();
        }
    }
}

//TODO
//[ ]write scramble,rawtime, time, penalty, data, solved at as plaintext or csv/ maybe simple database
//[ ]solve persistance
//[ ]store session name, number and the whatever state the scramble generator was in before program close
//[ ]clean up install file
//[ ]seperate folder for files
//[ ]file structure
//[ ]dependencies -> misc files and dependencies
//[ ]Sessions -> session.txt, one file per session

