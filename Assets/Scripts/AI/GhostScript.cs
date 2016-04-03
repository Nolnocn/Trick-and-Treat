using UnityEngine;
using System.Collections;

public class GhostScript : EnemyScript
{
		// The location the ghost kid is trying to reach,
		// namely a house or parent
		public Vector2 targetLocation;
		// if the ghost kid is fleeing, being captured, or 'dead'
		private bool flee;
		private bool capturing;
		private bool dead;
		private bool protect;
		// speed of kid when fleeing and walking
		private float fleeSpeed, walkSpeed;
		private float stunTime, captureTime;
		private float idleTime;
		private int numCandy;
		private int stoopNum;
		// the player
		public Transform player;
		// the protecting parent
		public GameObject parent;
		public GameObject candy;
		// Use this for initialization

		void Start ()
		{
				stunned = false;
				hit = false;
				facingRight = true;
				flee = false;
				capturing = false;
				dead = false;

				sortingOrderOffset = 1;
				spriteRenderer = GetComponent<SpriteRenderer> ();
				animator = GetComponent<Animator> ();
				player = ManagerScript.player;
				parent = null;

				walkSpeed = Random.Range (1.5f, 2.0f);
				fleeSpeed = walkSpeed * 2;
				stunTime = 0.0f;
				captureTime = 1.0f;
				numCandy = 25;
				idleTime = Random.Range (0.0f, 1.0f);
				stoopNum = Random.Range (0, 30);
				targetLocation = ManagerScript.stoops [stoopNum].position;
				ballSound = GetComponent<AudioSource> ();
		}

		// Update is called once per frame
		private void Update ()
		{		
				CheckDead ();
				if (!dead) {
						Move ();
				} else if (Mathf.Abs (player.position.x - transform.position.x) > ManagerScript.halfScreenWidth + Mathf.Abs (transform.localScale.x)) {
						Destroy (this.gameObject);
				}
				SendAlert ();
				Wrap ();
				SetSortingOrder ();
				ConstrainVertical ();
		}


		// Move the ghost kid and set a new destination
		private void Move ()
		{
				CheckStun ();
				CheckDanger ();
				idleTime -= Time.deltaTime;

				if(parent == null)
				{
					targetLocation = ManagerScript.stoops [stoopNum].position;
				}
				// if the ghost kid is not being captured and not stunned
				if (!capturing && !stunned && idleTime <= 0.0f) {
						// if the ghost has reached his destination, set a new one
						if (Vector2.Distance (targetLocation, (Vector2)transform.position) < 0.25f) {
								if (parent == null) {
										stoopNum = Random.Range (0, 30);
										targetLocation = ManagerScript.stoops [stoopNum].position;
								} else {
										protect = true;
										flee = false;
										animator.SetBool ("flee", false);
										animator.SetBool ("walk", false);
										GetComponent<Rigidbody2D>().velocity = Vector2.zero;
								}
						}
						
						if (flee) {
								// run at fleeing speed away from player if not protected
								if (parent == null) {
										GetComponent<Rigidbody2D>().velocity = -((Vector2)player.position - (Vector2)transform.position).normalized * fleeSpeed;
								
								} else {
										GetComponent<Rigidbody2D>().velocity = (targetLocation - (Vector2)transform.position).normalized * fleeSpeed;

								}
								animator.SetBool ("flee", true);
								animator.SetBool ("walk", false);
						}
						// otherwise just walk around
						else if (!protect) {
								GetComponent<Rigidbody2D>().velocity = (targetLocation - (Vector2)transform.position).normalized * walkSpeed;
								animator.SetBool ("flee", false);
								animator.SetBool ("walk", true);
						}
						

						if (GetComponent<Rigidbody2D>().velocity.x > 0) {
								facingRight = true;
								transform.localScale = new Vector3 (1, 1, 1);
						} else {
								facingRight = false;
								transform.localScale = new Vector3 (-1, 1, 1);
						}
				} 
				// if being captured or stunned, don't move
				else {
						GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				}
		}

		private void CheckStun ()
		{
				if (hit) {
						stunned = true;
						animator.SetTrigger ("hit");
						animator.SetBool ("stunned", true);
						animator.SetBool ("flee", false);
						animator.SetBool ("walk", false);

						stunTime = 3.0f;
						if (numCandy > 20.0f) {
								SpawnCandy ();
						}
						SendAlert ();
						hit = false;
				}

				if (stunned) {
						stunTime -= Time.deltaTime;
						if (stunTime <= 0.0f) {
								stunned = false;
								flee = true;
								animator.SetBool ("stunned", false);
						}
				}
		}

		private void CheckDanger ()
		{
				if (protect && parent.GetComponent<ParentScript> ().IsStunned ()) {
						if (parent != null) {
								protect = false;
								parent.GetComponent<ParentScript> ().StopProtect ();
								parent = null;
								flee = true;
						}
				}
				if ((flee || protect) && Mathf.Abs (player.position.x - transform.position.x) > 10.0f) {
						flee = false;
						protect = false;
						if (parent != null) {
								parent.GetComponent<ParentScript> ().StopProtect ();
								parent = null;
						}
			
			stoopNum = Random.Range (0, 30);
			targetLocation = ManagerScript.stoops [stoopNum].position;
				}
		}

		public void Flee ()
		{
				flee = true;
		}

		public void Capturing ()
		{
				if (stunned) {
						capturing = true;
						animator.SetBool ("capturing", true);
						animator.SetBool ("stunned", false);
						stunned = false;
						if (player.position.x > transform.position.x) {
								transform.localScale = new Vector3 (-1, 1, 1);
						} else {
								transform.localScale = new Vector3 (1, 1, 1);
						}
						SendAlert ();
				}

		}

		private void CheckDead ()
		{
				if (capturing) {
						captureTime -= Time.deltaTime;
			
						if (captureTime <= 0.0f) {
								while (numCandy > 0) {
										SpawnCandy ();
								}
								gameObject.GetComponent<BoxCollider2D> ().enabled = false;
								dead = true;
								flee = false;	
								capturing = false;
								if (parent != null) {
										parent.GetComponent<ParentScript> ().StopProtect ();
										parent = null;
								}
								animator.SetTrigger ("dead");
						}
				}
		}
	
		public void StopCapturing ()
		{
				if (capturing) {
						capturing = false;
						flee = true;
						animator.SetBool ("flee", true);
						animator.SetBool ("capturing", false);
						captureTime = 1.0f;
				}
		}

		private void SpawnCandy ()
		{
				GameObject c = (GameObject)Instantiate (candy, transform.position, Quaternion.identity);
				c.GetComponent<CandyScript> ().Initialize (transform.position.y - transform.localScale.y / 2);
				numCandy--;
		}

		private void SendAlert ()
		{
				if (hit || capturing || flee) {
						RaycastHit2D[] hits;
						hits = Physics2D.CircleCastAll (transform.position, ManagerScript.halfScreenWidth * 1.25f, Vector2.up);

						for (int i = 0; i < hits.Length; i++) {
								GameObject g = hits [i].collider.gameObject;

								if (g.tag == "Ghost" && (hit || capturing)) {
										g.GetComponent<GhostScript> ().flee = true;
								}
								if (g.tag == "Parent" && flee && !g.GetComponent<ParentScript> ().IsStunned () && parent == null) {
										if (!g.GetComponent<ParentScript> ().IsProtecting ()) {
												targetLocation = (Vector2)g.transform.position;
												parent = g;
												g.GetComponent<ParentScript> ().Protect (this.transform);
										}

								}
								if (g.tag == "Watchman" && (hit || capturing || flee)) {
										g.GetComponent<NeighborWatchScript> ().aware = true;
										g.GetComponent<NeighborWatchScript> ().chasing = true;
										g.GetComponent<NeighborWatchScript> ().patrolling = false;
								}

						}
				}
		}

		public bool GetDead ()
		{
				return dead;
		}

	public bool HasParent()
	{
		if(parent == null)
		{
			return false;
		}
		return true;
	}
}