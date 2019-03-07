using System;
using System.Collections.Generic;
using System.Text;

namespace SQL
{
	class gameStats
	{
		int personID;
		int pawnsEaten;
		int pawnsLost;
		int finalScore;
		int turns;
		int time;
		String mapName;
		int unlocked;
		int passed;

		public gameStats(int pawnsEaten,int pawnsLost,int finalScore,int turns,int time, String mapName, int unlocked, int passed){
			setPersonID(personID);
			setPawnsEaten(pawnsEaten);
			setPawnsLost(pawnsLost);
			setFinalScore(finalScore);
			setTurns(turns);
			setTime(time);
			setMapName(mapName);
			setUnlocked(unlocked);
			setPassed(passed);
		}

		

		public void setPersonID(int x){
			
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
		public void setTurns(int x)
		{
			turns = x;
		}
		public void setTime(int x)
		{
			time = x;
		}
		public void setMapName(String x)
		{
			mapName = x;
		}
		public void setUnlocked(int x)
		{
			unlocked = x;
		}
		public void setPassed(int passed)
		{
			this.passed = passed;
		}
		public int getID(){
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

		public int getTime()

		{
			return time;
		}

		public int getTurns()

		{
			return turns;
		}
		public String getMap()
		{
			return mapName;
		}
		public int getUnlocked()
		{
			return unlocked;
		}
		public int getPassed(){
			return passed;
		}

	}
}
