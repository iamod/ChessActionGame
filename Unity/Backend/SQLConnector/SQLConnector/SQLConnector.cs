using System;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Net;

namespace SQL{
	class SQLConnector
	{
		MySqlConnection conn;

		public SQLConnector(String connectionString)
		{
			initializeConnection(connectionString);
		}


		public void initializeConnection(String connectionString)
		{
			try
			{
				conn = new MySqlConnection(connectionString);
				Console.WriteLine("Connecting to mySQL DB");
				conn.Open();
				Console.WriteLine("Managed to connect to DB");
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.ToString());
			}

		}


		public int checkLogin(String username, String password)
		{
			
			MySqlCommand queryCommand = new MySqlCommand();
			string dbPW = "";
			int userID = -1;
			queryCommand.Connection = conn;
			queryCommand.CommandText = "SELECT password,ID from users WHERE username = @username";
			queryCommand.Prepare();
			queryCommand.Parameters.AddWithValue("@username", username);
			try
			{
				MySqlDataReader reader = queryCommand.ExecuteReader();
				while (reader.Read())
				{
					dbPW = (string)reader["password"];
					userID = (int)reader["ID"];
				}
				Sha1Hasher hashObj = new Sha1Hasher();
				if (dbPW == hashObj.GetHash(password))
				{
					Console.WriteLine("logged in");
					//Return playerID for the logged in player
					return userID;
					

				}
				else
				{
					Console.WriteLine("Incorret password or username");
				}
			}
			catch(Exception exc){
				Console.WriteLine(exc.Message);
				return -1;
			}
			//Failed to fetch ID
			return -1;

		}
		public String afterGameInsert(gameStats x){
			

			if(x.getID() != -1 && x.getPassed() == 1){
				checkUnlockMap(x);
				careerStats oldStats = getOldCareerStats(x.getID());
				careerStats combinedCareerStats = getNewCareerStats(x, oldStats);
				insertCareerStats(combinedCareerStats);
			}
			return "Stats saved post-game!";
			
		}
		public void insertCareerStats(careerStats combinedStats){

			MySqlCommand queryCommand = new MySqlCommand();

			queryCommand.Connection = conn;
			queryCommand.CommandText = "UPDATE playerStats SET  pawnsEaten = @pawnsEaten, pawnsLost = @pawnsLost, finalScore = @finalScore" +
			"WHERE personID= @personID";
			queryCommand.Prepare();
			queryCommand.Parameters.AddWithValue("@personID", combinedStats.getID());
			queryCommand.Parameters.AddWithValue("@pawnsEaten", combinedStats.getpawnsEaten());
			queryCommand.Parameters.AddWithValue("@pawnsLost", combinedStats.getPawnsLost());
			queryCommand.Parameters.AddWithValue("@finalScore", combinedStats.getFinalScore());
			try
			{
				MySqlDataReader reader = queryCommand.ExecuteReader();
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.Message);

			}


		}
		public careerStats getNewCareerStats(gameStats y, careerStats x){

			careerStats combinedStats = new careerStats(y.getID(), (y.getpawnsEaten() + x.getpawnsEaten()),
			(y.getPawnsLost() + x.getPawnsLost()), (y.getFinalScore() + x.getFinalScore()));
			return combinedStats;
		}

		public careerStats getOldCareerStats(int ID){
			careerStats oldStats = null;
			if (ID != -1){
				MySqlCommand queryCommand = new MySqlCommand();
				
				queryCommand.Connection = conn;
				queryCommand.CommandText = "SELECT pawnsEaten, pawnsLost, finalScore from careerStats WHERE ID = @ID";
				queryCommand.Prepare();
				queryCommand.Parameters.AddWithValue("@ID", ID);
				try
				{
					MySqlDataReader reader = queryCommand.ExecuteReader();
					while (reader.Read())
					{
						oldStats = new careerStats(ID, (int)reader["pawnsEaten"], (int)reader["pawnsLost"], (int)reader["finalScore"]);
						
					}
					return oldStats;

				}
				catch (Exception exc)
				{
					Console.WriteLine(exc.Message);
				}
			}
			return oldStats;
		}
		public String checkUnlockMap(gameStats x){

			if (x.getID() != -1 && x.getPassed() == 1) 
			{
				MySqlCommand queryCommand = new MySqlCommand();
				queryCommand.Connection = conn;
				queryCommand.CommandText = "IF NOT EXISTS (SELECT unlocked from userMaps WHERE ID = @ID AND mapName = @mapName" +
				"AND unlocked = 0) BEGIN INSERT INTO mapStats(personID, mapName, unlocked) VALUES (@ID,@mapName,@unlocked) END;";
				queryCommand.Prepare();
				queryCommand.Parameters.AddWithValue("@ID", x.getID());
				queryCommand.Parameters.AddWithValue("@mapName", x.getMap());
				queryCommand.Parameters.AddWithValue("@unlocked", x.getUnlocked());
				try
				{
					MySqlDataReader reader = queryCommand.ExecuteReader();
					return ("Unlocked new map!");

				}
				catch (Exception exc)
				{
					Console.WriteLine(exc.Message);
					return ("Failed to save map progress into database");
				}
			}
			return ("Map already unlocked");
		}
	}
}