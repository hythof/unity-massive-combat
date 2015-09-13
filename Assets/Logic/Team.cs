using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Team {
	public readonly Unit[] Units;
	public readonly Dictionary<int, List<Unit>> HitGroup = new Dictionary<int, List<Unit>> ();
	public int Moral = Const.MaxMoral;
	public float Rate { get { return (float)Moral / (float)Const.MaxMoral; } }
	public int AliveCount { get { return Units.Count (x => x.Alive); } }
	public bool Finish { get { return HitGroup.Count == 0; } }

	public Team(Unit[] units)
	{
		Units = units;
		foreach(var unit in units)
		{
			if(!HitGroup.ContainsKey(unit.HitGroup))
			{
				HitGroup[unit.HitGroup] = new List<Unit>();;
			}
			HitGroup[unit.HitGroup].Add(unit);
		}
	}

	public void Move()
	{
		foreach(var unit in Units)
		{
			unit.X += unit.Move;
		}
	}
	
	public void Attack(int count)
	{
		Moral -= (int)Mathf.Floor(count * Const.AttackMoralRate);
	}
	
	public void Defence(int count)
	{
		Moral -= (int)Mathf.Floor(count * Const.DefenceMoralRate);
	}
	
	public void Clear(int count)
	{
		Moral += (int)Mathf.Floor(count * Const.ClearMoralRate);
	}
}