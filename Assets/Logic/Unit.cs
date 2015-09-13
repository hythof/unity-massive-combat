using UnityEngine;
using System.Collections.Generic;

public class Unit
{
	public bool Leader = leader;
	public bool Alive { get { return Hp > 0; } }
	public bool Dead { get { return Hp <= 0; } }
	public int Hp;
	public int Attack;
	public int Defence;
	public int Move;
	public int X;
	public int Y;
	public int HitGroup { get { return Y / 10; } }

	public Unit(bool leader, int hp, int attack, int defence, int move, int x=0, int y=0)
	{
		Leader = leader;
		Hp = hp;
		Attack = attack;
		Defence = defence;
		Move = move;
		X = x;
		Y = y;
	}

	public void Damage(float damage, float rate)
	{
		var hit = (int)Mathf.Floor (damage - (Defence * rate));
		Hp -= Mathf.Max (Const.MinDamage, hit);
		if(damage <= 0)
		{
			Debug.LogError("no damage " + damage);
		}
	}
}