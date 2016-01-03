using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleScript : MonoBehaviour
{
	[SerializeField] GameObject unitPrefab;
	[SerializeField] Transform leftRoot;
	[SerializeField] Transform rightRoot;
	[SerializeField] BattleState battleState;
	[SerializeField] Text uiResult;
	UnitScript[] LeftUnits;
	UnitScript[] RightUnits;
	Player player = new Player();
	Parameter parameter;

	void Awake()
	{
		uiResult.text = "";
		Application.targetFrameRate = 30; // fixed fps
		parameter = new Parameter(Application.dataPath + "/parameters/");
	}

	// Use this for initialization
	void Start ()
	{
		var stage = parameter.Stages [0];
		var left = new Team (toUnits(stage));
		var right = new Team (player.Units);
		battleState = new BattleState (left, right);
		LeftUnits = deploy (battleState.Left.Units, leftRoot, Side.Left);
		RightUnits = deploy (battleState.Right.Units, rightRoot, Side.Right);

		StartCoroutine (turnUpdate());
	}

	Unit[] toUnits(Parameter.Stage stage)
	{
		var units = new List<Unit>();
		foreach (var enemy in stage.Enemies)
		{
			var job = enemy.Job;
			for(var i=0; i<enemy.Count; ++i)
			{
				var unit = new Unit(false, job.Hp, job.Attack, job.Defence, job.Move);
				units.Add(unit);
			}
		}

		var ps = stage.Formation.Positions;
		int len = stage.Formation.Positions.Length;
		for(int i=0; i<units.Count; ++i)
		{
			var p = ps[i % len];
			units[i].Leader = i < len ? ps[i].Leader : false;
			units[i].X = p.X;
			units[i].Y = p.Y;
		}

		return units.ToArray ();
	}

	UnitScript[] deploy(Unit[] units, Transform root, Side side)
	{
		int index = 0;
		var scripts = new UnitScript[units.Length];
		foreach (var unit in units)
		{
			var unitGameObject = GameObject.Instantiate(unitPrefab);
			unitGameObject.transform.parent = root;
			var script = unitGameObject.GetComponent<UnitScript>();
			script.Deploy(
				unit,
				side);
			scripts[index] = script;
			++index;
		}
		return scripts;
	}

	IEnumerator turnUpdate ()
	{
		yield return new WaitForSeconds (Const.FormationSecond);

		while(true)
		{
			battleState.NextTurn ();
			foreach(var x in LeftUnits)
			{
				x.ApplyAnimation();
			}
			foreach(var x in RightUnits)
			{
				x.ApplyAnimation();
			}
			yield return new WaitForSeconds (Const.TurnSecond);
			if(battleState.Done)
			{
				switch(battleState.Result)
				{
				case BattleResult.Win:
					uiResult.text = "You Win";
					break;
				case BattleResult.Lose:
					uiResult.text = "You Lose";
					break;
				case BattleResult.Draw:
					uiResult.text = "Draw";
					break;
				default:
					Debug.LogError("BUG");
					break;
				}
				yield break;
			}
		}
	}
}
