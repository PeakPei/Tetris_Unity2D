using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shape : MonoBehaviour 
{
	float lastFallStepTime = 0f;	
	float speed = 1f;

	int vBound = 0;
	int hBound = 0;

	List<Vector3> nonValidPositions = new List<Vector3>();
 
	void Update() 
	{
		if (Input.GetKeyDown(KeyCode.RightArrow)) 
		{
			transform.position += Vector3.right;

			if (!isValidPosition())
				transform.position += Vector3.left;	
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			transform.position += Vector3.left;

			if (!isValidPosition())
				transform.position += Vector3.right;
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			transform.Rotate(0, 0, 90);

			if (!isValidPosition())
				transform.Rotate(0, 0, -90);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) ||
		         Time.time - lastFallStepTime >= speed) 
		{
			transform.position += Vector3.down;

			if (!isValidPosition())
			{
				transform.position += Vector3.up;
				
				bool isGameOver = (transform.position.y >= vBound ? true : false);
				Camera.main.GetComponent <Tetris>().Next(isGameOver);
				enabled = false;
			}

			lastFallStepTime = Time.time;
		}
	}

	public void Init (int boundV, int boundH, List<Vector3> nonValidPos)
	{
		vBound = boundV;
		hBound = boundH;

		nonValidPositions = nonValidPos;
	}

	bool isValidPosition() 
	{        
		foreach (Transform child in transform) 
		{
			Vector3 v = RoundPos(child.position);

			if (!isInsideGrid(v))
				return false;
			else 
			{
				for (int i = 0; i < nonValidPositions.Count; i++) 
				{
					if (child.position == nonValidPositions[i])
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	Vector3 RoundPos(Vector3 v) 
	{
		return new Vector3(v.x + .5f, v.y - .5f, 0);
	}

	bool isInsideGrid(Vector3 pos) 
	{
		return (   pos.x >= -hBound + 1
				&& pos.x <= hBound
				&& pos.y >= -vBound);
	}
}
