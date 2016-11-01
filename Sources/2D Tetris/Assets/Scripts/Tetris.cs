using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tetris : MonoBehaviour 
{
	float offset = .5f;

	int gameHeight = 0;
	int gameWidth = 0;

	int score = 0;

	List<Vector3> partsPositions = new List<Vector3>();

	public GameObject[] shapesTypesList;
	public GameObject boundL;
	public GameObject boundR;

	public Text ScoreText;

	void Awake ()
	{
		gameHeight = (int) Camera.main.orthographicSize * 2;
		gameWidth = (int) Camera.main.orthographicSize * Screen.width / Screen.height * 2;

		boundL.transform.localScale = new Vector3 (0.1f, gameHeight, 0);
		boundR.transform.localScale = new Vector3 (0.1f, gameHeight, 0);
		boundL.transform.position = new Vector3 (-gameWidth/2 - boundL.transform.localScale.x/2, 0, 0);
		boundR.transform.position = new Vector3 ( gameWidth/2 + boundL.transform.localScale.x/2, 0, 0);

		ScoreText.transform.position = new Vector3 (0, Camera.main.orthographicSize - 1.5f, 0);
	}

	void Start () 
	{
		UpdateScore ();
		Add ();
	}

	void Add ()
	{
		int i = Random.Range(0, shapesTypesList.Length);

		GameObject shape = Instantiate ( shapesTypesList[i], 
											   new Vector2 (1, gameHeight/2+2), 
											   Quaternion.identity ) as GameObject;

		Shape shapeScript = shape.GetComponent<Shape>();

		shapeScript.Init (gameHeight/2, gameWidth/2, partsPositions);
	}

	public void Next (bool isGameOver)
	{
		if (!isGameOver)
		{
			UpdatePartsPositions ();
			CheckForFull ();
			StartCoroutine (AddNextAfterEndOfFrame());			
		}
	}

	IEnumerator AddNextAfterEndOfFrame ()
	{
		yield return new WaitForEndOfFrame();
		UpdatePartsPositions ();
		Add ();
	}

	void UpdatePartsPositions ()
	{
		GameObject[] parts = GameObject.FindGameObjectsWithTag("Part");
		partsPositions = new List<Vector3>(); 

		foreach (GameObject part in parts) 
		{
			partsPositions.Insert (partsPositions.Count, part.transform.position);
		}
	}

	void CheckForFull ()
	{
		for (int l = GetMaxLineID(); l > -gameHeight/2; l--) 
		{
			if (isFullLine(l)) 
				DeleteLine (l);
		}
	}

	void DeleteLine (int lineID)
	{
		GameObject[] parts = GameObject.FindGameObjectsWithTag("Part");

		foreach (GameObject part in parts)
		{
			int posY = (int) Mathf.Round (part.transform.position.y + offset);

			if (posY == lineID)
		    	Destroy(part);
		    else if (posY > lineID)
		    	part.transform.position += Vector3.down;
		}

		score++;
		UpdateScore ();
	}

	int GetMaxLineID ()
	{
		int maxLineID = -gameHeight/2 - 1;

		for (int i = 0; i < partsPositions.Count; i++) 
		{
			int posY = (int) Mathf.Round (partsPositions[i].y - offset);

			if (posY > maxLineID)
			{
				maxLineID = posY;
			}			
		}

		return maxLineID;
	}

	bool isFullLine (int lineID)
	{
		int count = 0;

		for (int i = 0; i < partsPositions.Count; i++) 
		{
			int posY = (int) Mathf.Round (partsPositions[i].y + offset);
			
			if (posY == lineID)
				count++;
		}
		
		return (count == gameWidth);
	}

	void UpdateScore () 
	{
		ScoreText.text = score.ToString();
	}
}