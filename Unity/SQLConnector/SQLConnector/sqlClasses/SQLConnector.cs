using System;
using System.Configuration;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Net;

namespace SQL{
	class SQLConnector
	{
		MySqlConnection conn;
		MySqlCommand queryCommand;
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
				queryCommand = new MySqlCommand();
				queryCommand.Connection = conn;
				Console.WriteLine("Managed to connect to DB");
			}
			catch (Exception exc)
			{
				Console.WriteLine("Initializing connection to database failed" + exc.Message);
			}

		}


		public int checkLogin(String username, String password)
		{

			string dbPW = "";
			int userID = -1;
			queryCommand.Parameters.Clear();
			queryCommand.CommandText = "SELECT password,ID from users WHERE username = @username";
			queryCommand.Parameters.AddWithValue("@username", username);
			queryCommand.Prepare();
			try
			{
				MySqlDataReader reader = queryCommand.ExecuteReader();
				while (reader.Read())
				{
					dbPW = (string)reader["password"];
					userID = (int)reader["ID"];
				}
				reader.Close();
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
			catch (Exception exc)
			{
				Console.WriteLine("Logging in failed" + exc.Message);
				return userID;
			}
			//Failed to fetch ID
			return userID;

		}
		public void afterGameInsert(GameStats afterGameStats)
		{


			if (afterGameStats.getID() != -1 && afterGameStats.getPassed() == 1)
			{
				checkUnlockMap(afterGameStats);
				insertCareerStats(afterGameStats);
			}

		}
		public void insertCareerStats(GameStats afterStats)
		{
			queryCommand.Parameters.Clear();
			queryCommand.CommandText = "UPDATE careerstats SET pawnsEaten = pawnsEaten + @pawnsEaten, pawnsLost = pawnsLost + @pawnsLost, totalScore = totalScore +  @totalScore" +
			"WHERE personID= @personID";
			queryCommand.Parameters.AddWithValue("@personID", afterStats.getID());
			queryCommand.Parameters.AddWithValue("@pawnsEaten", afterStats.getpawnsEaten());
			queryCommand.Parameters.AddWithValue("@pawnsLost", afterStats.getPawnsLost());
			queryCommand.Parameters.AddWithValue("@totalScore", afterStats.getFinalScore());
			queryCommand.Prepare();
			try
			{
				queryCommand.ExecuteNonQuery();
			}
			catch (Exception exc)
			{
				Console.WriteLine("Inserting career stats failed" + exc.Message);

			}


		}


		public CareerStats getCareerStats(int ID)
		{

			CareerStats playerStats = null;
			if (ID != -1)
			{
				queryCommand.Parameters.Clear();
				queryCommand.CommandText = "SELECT pawnsEaten, pawnsLost, totalScore from careerstats WHERE ID = @id";

				queryCommand.Parameters.AddWithValue("@id", ID);
				queryCommand.Prepare();
				try
				{
					MySqlDataReader reader = queryCommand.ExecuteReader();
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							playerStats = new CareerStats(ID, (int)reader["pawnsEaten"], (int)reader["pawnsLost"], (int)reader["totalScore"]);

						}
						reader.Close();
						if (playerStats != null)
						{
							return playerStats;
						}
						else
						{
							Console.WriteLine("Getting old career stats failed");
						}
					}
					else
					{
						Console.WriteLine("Getting old career stats failed");
					}

				}
				catch (Exception exc)
				{
					Console.WriteLine("Getting old career stats failed " + exc.Message);
				}
			}
			return playerStats;
		}
		public void checkUnlockMap(GameStats x)
		{

			if (x.getID() != -1 && x.getPassed() == 1)
			{

				queryCommand.Parameters.Clear();
				queryCommand.CommandText = "UPDATE userMaps SET	 unlocked = @unlocked WHERE mapName = @mapName AND unlocked = 0 AND ID = @ID;";
				queryCommand.Parameters.AddWithValue("@ID", x.getID());
				queryCommand.Parameters.AddWithValue("@mapName", x.getMap());
				queryCommand.Parameters.AddWithValue("@unlocked", x.getUnlocked());
				queryCommand.Prepare();
				try
				{
					queryCommand.ExecuteNonQuery();
					Console.WriteLine("Map " + x.getMap() + "unlocked");

				}
				catch (Exception exc)
				{
					Console.WriteLine("Checking map unlock failed" + exc.Message);
				}
			}

		}
		public ArrayList getUnlockedMaps(int userID)
		{
			ArrayList unlocks = new ArrayList();
			if (userID != -1)
			{

				queryCommand.Parameters.Clear();
				queryCommand.CommandText = "SELECT mapName from userMaps where ID = @ID AND unlocked = 1";
				queryCommand.Parameters.AddWithValue("@ID", userID);
				queryCommand.Prepare();

				try
				{

					MySqlDataReader reader = queryCommand.ExecuteReader();
					while (reader.Read())
					{
						unlocks.Add((string)reader["mapName"]);
					}
					reader.Close();

				}
				catch (Exception exc)
				{
					Console.WriteLine("Fetching unlocked maps failed" + exc.Message);
				}
			}
			return unlocks;
		}


		public ArrayList getMapHiscores(String mapName, int topX)
		{
			ArrayList playerHighScores = new ArrayList();
			queryCommand.Parameters.Clear();
			queryCommand.CommandText = "SELECT username FROM yourTable ORDER BY Likes DESC LIMIT @amount;";
			queryCommand.Parameters.AddWithValue("@mapname", mapName);
			queryCommand.Parameters.AddWithValue("@amount", topX);
			queryCommand.Prepare();
			try
			{

				MySqlDataReader reader = queryCommand.ExecuteReader();
				while (reader.Read())
				{
					unlocks.Add((string)reader["mapName"]);
				}
				reader.Close();

			}
			catch (Exception exc)
			{
				Console.WriteLine("Fetching unlocked maps failed" + exc.Message);
			}

			return playerHighScores;
		}
	}
	
}