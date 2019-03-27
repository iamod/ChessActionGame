using System;
using System.Collections;
using System.Text;
namespace SQL
{
	class mainClass
	{


		static void Main(String[] args)
		{
			SQLConnector conn = new SQLConnector("server=127.0.0.1;user=root;database=chesstesting;port=3306;password=password");
			int userID = conn.checkLogin("Eckersley", "testikoira123");
			GameStats testStats = new GameStats(userID, 1,2,3,4,5,"catMap",0,1);
			conn.afterGameInsert(testStats);
			/*ArrayList x = conn.getUnlockedMaps(userID);
			foreach (String y in x){
				Console.WriteLine(y);
			}*/
			Console.WriteLine(userID);
			ConsoleKeyInfo name = Console.ReadKey();
		}

	}

}