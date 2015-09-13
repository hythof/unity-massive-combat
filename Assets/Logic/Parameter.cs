using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Parameter
{
	public readonly Stage[] Stages;
	public readonly Enemy[] Enemies;
	public readonly Formation[] Formations;
	public readonly Position[] Positions;
	public readonly Job[] Jobs;

	public Parameter(string dir)
	{
		Stages = load<Stage> (dir, "stage.csv", x => new Stage(x));
		Enemies = load<Enemy> (dir, "enemy.csv", x => new Enemy(x));
		Formations = load<Formation> (dir, "formation.csv", x => new Formation(x));
		Positions = load<Position> (dir, "position.csv", x => new Position(x));
		Jobs = load<Job> (dir, "job.csv", x => new Job(x));

		// set relation
		var enemies = toListDictionary<Enemy> (Enemies, x => x.StageId);
		var formations = toDictionary<Formation> (Formations, x => x.Id);
		foreach (var stage in Stages)
		{
			stage.Formation = formations[stage.FormationId];
			stage.Enemies = enemies[stage.Id].ToArray();
		}

		var jobs = toDictionary<Job> (Jobs, x => x.Id);
		foreach (var enemy in Enemies)
		{
			enemy.Job = jobs[enemy.StageId];
		}

		var positions = toListDictionary<Position> (Positions, x => x.FormationId);
		foreach (var formation in Formations)
		{
			formation.Positions = positions[formation.Id].ToArray();
		}
	}

	IDictionary<int, T> toDictionary<T>(T[] xs, System.Func<T, int> f)
	{
		var dict = new Dictionary<int, T> ();
		foreach(var x in xs)
		{
			dict[f(x)] = x;
		}
		return dict;
	}
	
	IDictionary<int, List<T>> toListDictionary<T>(T[] xs, System.Func<T, int> f)
	{
		var dict = new Dictionary<int, List<T>> ();
		foreach(var x in xs)
		{
			var id = f(x);
			if(!dict.ContainsKey(id))
			{
				dict[f(x)] = new List<T>();
			}
			dict[f(x)].Add(x);
		}
		return dict;
	}
	
	T[] load<T>(string dir, string name, System.Func<string[], T> init)
	{
		var path = dir + "/" + name;
		var lines = File.ReadAllText (path, Encoding.GetEncoding("Shift_JIS")).Split('\n');
		var xs = new List<T>();

		// skip label line, and eof line
		foreach(var line in lines)
		{
			var row = line.Replace("\"", "").Split(',');
			int v;
			if(row.Length > 0 && int.TryParse(row[0], out v) && v > 0)
			{
				xs.Add(init(row));
			}
		}
		return xs.ToArray();
	}

	public class Stage
	{
		public readonly int Id;
		public readonly string Name;
		public readonly int FormationId;
		public Formation Formation;
		public Enemy[] Enemies;

		public Stage(string[] rows)
		{
			Id = int.Parse(rows[0]);
			Name = rows[1];
			FormationId = int.Parse(rows[2]);
		}
	}

	public class Enemy
	{
		public readonly int Id;
		public readonly int StageId;
		public readonly int JobId;
		public readonly int Count;
		public Job Job;

		public Enemy(string[] rows)
		{
			Id = int.Parse(rows[0]);
			StageId = int.Parse(rows[1]);
			JobId = int.Parse(rows[2]);
			Count = int.Parse(rows[3]);
		}
	}

	public class Formation
	{
		public readonly int Id;
		public readonly string Name;
		public readonly string Description;
		public Position[] Positions;

		public Formation(string[] rows)
		{
			Id = int.Parse(rows[0]);
			Name = rows[1];
			Description = rows[2];
		}
	}

	public class Position
	{
		public readonly int Id;
		public readonly int FormationId;
		public readonly bool Leader;
		public readonly bool Forward;
		public readonly bool Back;
		public readonly int X;
		public readonly int Y;
			
		public Position(string[] rows)
		{
			Id = int.Parse(rows[0]);
			FormationId = int.Parse(rows[1]);
			Leader = rows[2] == "o";
			Forward = rows[3] == "o";
			Back = rows[4] == "o";
			X = int.Parse(rows[5]);
			Y = int.Parse(rows[6]);
		}
	}

	public class Job
	{
		public readonly int Id;
		public readonly string Name;
		public readonly string Prefab;
		public readonly int Hp;
		public readonly int Attack;
		public readonly int Defence;
		public readonly int Move;
		
		public Job(string[] rows)
		{
			Id = int.Parse(rows[0]);
			Name = rows[1];
			Prefab = rows[2];
			Hp = int.Parse(rows[3]);
			Attack = int.Parse(rows[4]);
			Defence = int.Parse(rows[5]);
			Move = int.Parse(rows[6]);
		}
	}
}