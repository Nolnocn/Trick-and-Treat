using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
	private bool canCollide;
	private bool startDespawn;
	private int sortingOrderOffset;
	private int direction;
	private float yLevel;
	private SpriteRenderer spriteRenderer;

	private Rigidbody2D myRigidbody;
	
	void Start ()
	{
		canCollide = true;
		startDespawn = false;
		sortingOrderOffset = 4;
		GetComponent<Rigidbody2D>().velocity = new Vector2(10 * direction, 0);
		spriteRenderer = GetComponent<SpriteRenderer>();
		SetSortingOrder();

		myRigidbody = GetComponent<Rigidbody2D>();
	}

	void Update ()
	{
		CheckPosition();
		ConstrainVertical();
		SetSortingOrder();
		transform.Rotate(new Vector3(0, 0, 500 * direction * Time.deltaTime));
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(canCollide)
		{
			if(col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				myRigidbody.velocity = new Vector2(-myRigidbody.velocity.x * 0.25f, direction * myRigidbody.velocity.x * 0.5f);
				myRigidbody.gravityScale = 1.0f;
				canCollide = false;
				if(col.tag == "Ghost")
				{
					col.GetComponent<GhostScript>().Hit();
				}
				else if(col.tag == "Parent")
				{
					col.GetComponent<ParentScript>().Hit();
				}
				else if(col.tag == "Watchman")
				{
					col.GetComponent<NeighborWatchScript>().Hit();
				}

				//StartCoroutine(DespawnDelay());
			}
		}
	}

	private void SetSortingOrder()
	{
		spriteRenderer.sortingOrder = Mathf.FloorToInt(100 - transform.position.y * 10) - sortingOrderOffset;
	}

	private void CheckPosition()
	{
		if(transform.position.x > ManagerScript.player.position.x + ManagerScript.halfScreenWidth 
		   || transform.position.x < ManagerScript.player.position.x - ManagerScript.halfScreenWidth)
		{
			Destroy(gameObject);
		}
	}

	public void Initialize(int dir, float y)
	{
		direction = dir;
		yLevel = y;
	}

	private void ConstrainVertical()
	{
		if(transform.position.y < yLevel && myRigidbody.velocity.y < 0)
		{
			if(!startDespawn)
			{
				StartCoroutine(DespawnDelay());
			}
			//transform.position = new Vector3(transform.position.x, yLevel, transform.position.z);
			myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -0.5f * myRigidbody.velocity.y);
		}
	}

	private IEnumerator DespawnDelay()
	{
		startDespawn = true;
		//yield return new WaitForSeconds(2.5f);
		for(int i = 0; i < 15; i ++)
		{
			yield return new WaitForSeconds(0.1f);
			Color color = spriteRenderer.color;
			color.a = color.a == 1 ? 0.1f : 1;
			spriteRenderer.color = color;
		}
		Destroy(gameObject);
	}
}
