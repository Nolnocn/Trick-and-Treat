using UnityEngine;
using System.Collections;

public class ManagerScript : MonoBehaviour
{
	public static int numCells;
	public static float cellWidth = 16;
	public static float upperVertBounds = 6.25f;
	public static float lowerVertBounds = 1.0f;
	public static float halfScreenWidth;
	public static Transform player;
	public static Transform[] stoops;

	public GameObject treePrefab;
	public Transform treeholder;
	private float treeWidth = 40;
	private Transform[] cells;
	private Transform[] trees;
	private bool spawnNW;

	public GameObject nwPrefab;
	public GameObject ghostPrefab;
	public GameObject parentPrefab;
	
	void Awake ()
	{
		spawnNW = false;
		Transform cellContainer = transform.FindChild("Cells");
		cells = new Transform[cellContainer.childCount];
		for(int i = 0; i < cells.Length; i++)
		{
			cells[i] = cellContainer.GetChild(i);
		}
		numCells = cells.Length;
		player = transform.FindChild("Tom");
		halfScreenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f)).x;

		CreateTrees();
		SpawnNPCs();

		stoops = new Transform[30];
		for(int i = 0; i < cells.Length; i++)
		{
			for(int j = 0; j < 6; j++)
			{
				stoops[i * 6 + j] = cells[i].transform.GetChild(j);
			}
		}
	}

	void Update ()
	{
		CheckCellPositions();
	}

	private void CreateTrees()
	{
		int treeCount = 100;
		trees = new Transform[treeCount];
		for(int i = 0; i < treeCount; i++)
		{
			GameObject tree = Instantiate(treePrefab,
			                              new Vector3(Random.Range(-treeWidth * 0.5f, treeWidth * 0.5f), 6.9f, 0),
			                              Quaternion.identity) as GameObject;
			tree.transform.parent = treeholder;
			float rand = Random.Range(0.5f, 1.0f);
			tree.transform.localScale = new Vector3(rand, rand, 1);
			tree.transform.position = new Vector3(tree.transform.position.x, tree.transform.position.y - (1 - rand), 0);
			trees[i] = tree.transform;
		}
	}

	private void SpawnNPCs()
	{
		for(int i = 0; i < 15; i++)
		{
			Vector3 ghostPos = new Vector3(0, Random.Range(2.0f, 6.0f), 0);
			ghostPos.x = Random.Range(-40, 40);
			Instantiate(ghostPrefab, ghostPos, Quaternion.identity);
		}

		for(int i = 0; i < numCells; i++)
		{
			Vector3 parentPos = new Vector3(0, Random.Range(2.0f, 6.0f), 0);
			parentPos.x = cells[i].position.x + Random.Range(-8, 8);
			Instantiate(parentPrefab, parentPos, Quaternion.identity);
		}
			
		Vector3 nwPos = new Vector3(0, 5, 0);
		nwPos.x = -36;
		Instantiate(nwPrefab, nwPos, Quaternion.identity);
	}

	public void ParalaxifyTrees(float speed)
	{
		for(int i = 0; i < trees.Length; i++)
		{
			trees[i].Translate(speed * Time.deltaTime, 0, 0);
			if(trees[i].position.x > player.position.x + treeWidth * 0.5f)
			{
				trees[i].transform.position -= new Vector3(treeWidth, 0, 0);
			}
			else if(trees[i].position.x < player.position.x - treeWidth * 0.5f)
			{
				trees[i].transform.position += new Vector3(treeWidth, 0, 0);
			}
		}
	}

	public void CheckCellPositions()
	{
		for(int i = 0; i < numCells; i++)
		{
			if(cells[i].position.x > player.position.x + cellWidth * numCells * 0.5f)
			{
				ShiftCellLeft(i);
			}
			else if(cells[i].position.x < player.position.x - cellWidth * numCells * 0.5f)
			{
				ShiftCellRight(i);
			}
		}
	}

	private void ShiftCellRight(int cellIndex)
	{
		Transform cell = cells[cellIndex];
		cell.position += new Vector3(cellWidth * numCells, 0, 0);
	}

	private void ShiftCellLeft(int cellIndex)
	{
		Transform cell = cells[cellIndex];
		cell.position -= new Vector3(cellWidth * numCells, 0, 0);
	}

	public void KidCaptured()
	{
		Vector3 ghostPos = new Vector3(0, 5, 0);
		ghostPos.x = player.position.x + Random.Range(halfScreenWidth * 1.5f, 32) * player.transform.localScale.x;
		Instantiate(ghostPrefab, ghostPos, Quaternion.identity);

		if(spawnNW)
		{
			Vector3 nwPos = new Vector3(0, 5, 0);
			nwPos.x = player.position.x + Random.Range(halfScreenWidth * 1.5f, 32) * player.transform.localScale.x;
			Instantiate(nwPrefab, nwPos, Quaternion.identity);
		}

		spawnNW = !spawnNW;
	}
}
