using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour
{
#if DEBUG
	[SerializeField] int hp; // for debug
#endif
	[SerializeField] Animator animator;
	Side side;
	Unit unit;
	const float minY = 0;
	const float maxY = 10;
	const float minX = -7.5f;
	const float maxX = 7.5f;

	public void Deploy(Unit unit, Side side)
	{
		this.unit = unit;
		this.side = side;
		var scaleX = side == Side.Left ? 1 : -1;

		// animation
		animator.SetBool ("isGround", true);
		animator.SetFloat ("Horizontal", 1.0f);
		transform.localScale = new Vector3 (scaleX, 1, 1);

		transform.localPosition = Vector3.zero;
		StartCoroutine(moveAnimation(unit.X, unit.Y, Const.FormationSecond));
	}

	IEnumerator moveAnimation(int inX, int inY, float second, int rowX=0, int rowY=0)
	{
		bool isClear = inX >= Const.BattleWidth;
		int frame = (int)System.Math.Floor(Application.targetFrameRate * second);
		var x = Mathf.Clamp (inX, 0, Const.BattleWidth);
		var y = Mathf.Clamp (inY, 0, Const.BattleHeight);
		x = side == Side.Left ? x : Const.BattleWidth - x;

		// convert logical position(0..100) to unity position(minX .. maxX, minY .. maxY)
		float localX = Mathf.Lerp (minX, maxX, (float)x / Const.BattleWidth);
		float localY = Mathf.Lerp (maxY, minY, (float)y / Const.BattleHeight);

		var pos = transform.localPosition;
		var deltaX = (localX - pos.x + rowX) / frame;
		var deltaY = (localY - pos.y + rowY) / frame;
		for (int i=0; i<frame; ++i)
		{
			yield return null;
			pos.x += deltaX;
			pos.y += deltaY;
			transform.localPosition = pos;
		}

		if(isClear)
		{
			StartCoroutine(clear());
		}
	}

	public void ApplyAnimation()
	{
		if(unit.Alive)
		{
			StartCoroutine(moveAnimation (unit.X, unit.Y, Const.TurnAnimationSecond));
		}
		else
		{
			StartCoroutine(clear()); // todo die motion
		}
#if DEBUG
		hp = unit.Hp;
#endif
	}

	IEnumerator clear()
	{
		animator.SetBool ("Jump", true);
		yield return StartCoroutine(moveAnimation (Const.BattleWidth, unit.Y + 20, Const.ClearSecond));
		animator.enabled = false;
		GetComponent<SpriteRenderer> ().enabled = false;
	}
}

public enum Side
{
	Left,
	Right
}
