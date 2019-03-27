using System;
using System.Collections.Generic;
using System.Text;

namespace SQL
{
	class CareerStats
	{
		int personID;
		int pawnsEaten;
		int pawnsLost;
		int totalScore;

		public CareerStats(int personID, int pawnsEaten, int pawnsLost, int totalScore)
		{
			setPersonID(personID);
			setPawnsEaten(pawnsEaten);
			setPawnsLost(pawnsLost);
			setFinalScore(totalScore);
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
			totalScore = x;
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

		public int getTotalScore()
		{
			return totalScore;
		}


	}
}
