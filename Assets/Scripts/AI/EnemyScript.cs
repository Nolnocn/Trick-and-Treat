using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (BoxCollider2D))]

public class EnemyScript : MonoBehaviour
{
	protected bool stunned;
	protected bool hit;
	protected bool facingRight;
	protected int sortingOrderOffset;
	protected Animator animator;
	protected SpriteRenderer spriteRenderer;
	protected AudioSource ballSound;

	// Make sure to initialize these variables in the other NPC's scripts
	void Start ()
	{
		stunned = false;
		facingRight = true;
		sortingOrderOffset = 1;
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected void Wrap()
	{
		float worldLength = ManagerScript.cellWidth * ManagerScript.numCells;
		if(transform.position.x > ManagerScript.player.position.x + worldLength * 0.5f)
		{
			transform.position -= new Vector3(worldLength, 0, 0);
		}
		else if(transform.position.x < ManagerScript.player.position.x - worldLength * 0.5f)
		{
			transform.position += new Vector3(worldLength, 0, 0);
		}
	}

	protected void SetSortingOrder()
	{
		spriteRenderer.sortingOrder = Mathf.FloorToInt(100 - transform.position.y * 10) - sortingOrderOffset;
	}

	protected void ConstrainVertical()
	{
		if(transform.position.y < ManagerScript.lowerVertBounds + transform.localScale.y * 0.5f)
		{
			transform.position = new Vector3(transform.position.x, ManagerScript.lowerVertBounds + transform.localScale.y * 0.5f, transform.position.z);
		}
		else if(transform.position.y > ManagerScript.upperVertBounds)
		{
			transform.position = new Vector3(transform.position.x, ManagerScript.upperVertBounds, transform.position.z);
		}
	}

	public void Stun()
	{
		stunned = true;
	}

	public void Hit()
	{
		hit = true;
		ballSound.Play();
	}
}
