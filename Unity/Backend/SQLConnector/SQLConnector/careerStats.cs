using System;
using System.Collections.Generic;
using System.Text;

namespace SQL
{
	class careerStats
	{
		int personID;
		int pawnsEaten;
		int pawnsLost;
		int finalScore;

		public careerStats(int personID, int pawnsEaten, int pawnsLost, int finalScore)
		{
			setPersonID(personID);
			setPawnsEaten(pawnsEaten);
			setPawnsLost(pawnsLost);
			setFinalScore(finalScore);
		}
		public void setPersonID(int x)
		{

			personID = x;
		}
		public void setPawnsEaten(int x)
		{
			pawnsEaten = x;
		}
		public void setPawnsLost(int x)
		{
			pawnsLost = x;
		}
		public void setFinalScore(int x)
		{
			finalScore = x;
		}


		public int getID()
		{
			return personID;
		}
		public int getpawnsEaten()
		{
			return pawnsEaten;
		}

		public int getPawnsLost()
		{
			return pawnsLost;
		}

		public int getFinalScore()
		{
			return finalScore;
		}


	}
}
