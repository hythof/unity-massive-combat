using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum BattleResult
{
	Win,
	Lose,
	Draw
}

public class BattleState
{
	public Team Left;
	public Team Right;
	public bool Done;
	public BattleResult Result;
	int turn;

	public BattleState(Team left, Team right)
	{
		Left = left;
		Right = right;
		turn = 0;
	}

	public void NextTurn()
	{
		++turn;
		Left.Move ();
		Right.Move ();
		moveHitCheck ();
		Done = Left.Finish || Right.Finish;
		var rv = Right.AliveCount;
		var lv = Left.AliveCount;
		Result = BattleResult.Draw;
		if(rv > lv)
		{
			Result = BattleResult.Win;
		}
		else if (rv < lv)
		{
			Result = BattleResult.Lose;
		}
	}

	void moveHitCheck()
	{
		List<Unit> left;
		List<Unit> right;
		for(int i=0; i<100; ++i)
		{
			if(!Left.HitGroup.TryGetValue(i, out left) ||
			   !Right.HitGroup.TryGetValue(i, out right))
			{
				continue;
			}

			if(left.Count == 0 ||
			   right.Count == 0)
			{
				continue;
			}
			var leftX = left.Max(x => x.X);
			var rightX = right.Max(x => x.X);
			if(leftX + rightX >= 100)
			{
				var mid = (leftX + rightX) / 2;
				var lmid = mid;
				var rmid = Const.BattleWidth - mid;
				var l = left.Where(x => x.X >= Const.BattleWidth - rightX).ToList();
				var r = right.Where(x => x.X >= Const.BattleWidth - leftX).ToList();
				if(l.Count == 0)
				{
					Debug.LogError("contact but empty left");
				}
				if(r.Count == 0)
				{
					Debug.LogError("contact but empty right");
				}
				int power = l.Count - r.Count;
				var lv = l.Count - r.Count;
				var rv = r.Count - l.Count;
				l.ForEach(x => x.X = lmid - Const.RepelBase + lv);
				r.ForEach(x => x.X = rmid - Const.RepelBase + rv);
				attack(l, r, Left, Right);
				attack(r, l, Right, Left);
				clear(l, Left);
				clear(r, Right);
			}
		}
	}

	void clear(List<Unit> units, Team team)
	{
		int hitGroup = -1;
		var removes = units.Where (x => x.X == Const.BattleWidth || x.Dead).ToList();
		foreach (var unit in removes) {
			units.Remove(unit);
			hitGroup = unit.HitGroup;
		}
		if(units.Count == 0 && hitGroup >= 0)
		{
			team.HitGroup.Remove(hitGroup);
		}
	}

	void attack(List<Unit> attack, List<Unit> defence, Team attackTeam, Team defenceTeam)
	{
		var damage = attack.Sum(x => x.Attack) * attackTeam.Rate;
		var unitDamage = damage / defence.Count;
		foreach(var unit in defence)
		{
			unit.Damage(unitDamage, defenceTeam.Rate);
		}
	}
}
