using System;
using System.Collections.Generic;
using System.Text;

namespace SQL
{
	class mainClass
	{


		static void Main(String[] args)
		{
			SQLConnector conn = new SQLConnector("server=127.0.0.1;user=root;database=chesstesting;port=3306;password=password");
			int userID = conn.checkLogin("Eckersley", "testikoira123");
			Console.WriteLine(userID);
			ConsoleKeyInfo name = Console.ReadKey();
		}

	}

}