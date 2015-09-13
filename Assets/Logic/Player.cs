using UnityEngine;
using System.Collections;

public class Player
{
	public Unit[] Units;

	public Player()
	{
		// kari
		Units = new Unit[]{
			new Unit (false, 500, 100, 100, 5, 10, 0),
			new Unit (false, 500, 100, 100, 5, 10, 10),
			new Unit (false, 500, 100, 100, 5, 10, 20),
			new Unit (false, 500, 100, 100, 5, 10, 30),
			new Unit (false, 500, 100, 100, 5, 10, 40),
			new Unit (true, 800, 300, 100, 5, 20, 45),
			new Unit (true, 1000, 300, 100, 5, 20, 50),
			new Unit (true, 800, 300, 100, 5, 20, 55),
			new Unit (false, 500, 100, 100, 5, 10, 60),
			new Unit (false, 500, 100, 100, 5, 10, 70),
			new Unit (false, 500, 100, 100, 5, 10, 80),
			new Unit (false, 500, 100, 100, 5, 10, 90),
			new Unit (false, 500, 100, 100, 5, 10, 99),
		};
	}
}