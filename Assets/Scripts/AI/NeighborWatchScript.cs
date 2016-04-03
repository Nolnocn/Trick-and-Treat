using UnityEngine;
using System.Collections;

public class NeighborWatchScript : EnemyScript {

	public bool aware, chasing, patrolling, attacking; 
	private bool movingUp, movingDown, patrolRight;
	private float horSpeed, vertSpeed, attackRange, stunTime;
	private Transform tomPos;
	private AudioSource shockSound;

	private Rigidbody2D myRigidbody;

	// Use this for initialization
	void Start () {

		aware = false;
		chasing = false;
		patrolling = true;
		hit = false;
		stunned = false;
		movingUp = false;
		movingDown = true;
		attacking = false;
		horSpeed = Random.Range(1.5f, 2.0f);
		vertSpeed = horSpeed * 0.5f;
		attackRange = 1.0f;
		tomPos = ManagerScript.player;
		//set patrol direction
		if(Random.Range(1,3) == 1)
		{
			patrolRight = true;
		}
		else
		{
			patrolRight = false;
		}
		stunned = false;
		facingRight = true;
		sortingOrderOffset = 1;
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		StartPatrolling ();

		AudioSource[] audioSources = GetComponents<AudioSource>();
		ballSound = audioSources[0];
		shockSound = audioSources[1];

		myRigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!chasing){
			CheckVerticalBounds ();
		}
		else{
			ConstrainVertical ();
		}
		Move ();
		CheckStun ();
		SendAlert ();
		Wrap();
		SetSortingOrder();
	}

	void Move() {

		//Direction NW should be facing
		if(facingRight)
		{
			transform.localScale = new Vector3(1,1,1);
		}
		else
		{
			transform.localScale = new Vector3(-1,1,1);
		}

		if(chasing && !stunned && !patrolling && !attacking)
		{
			Chase ();
		}
		else if(!aware && !chasing && !stunned && patrolling && !attacking)
		{
			Patrol ();
		}
	}

	void Patrol() {

		int horDir = 1;
		int vertDir = 1;

		if(movingDown)
		{
			vertDir = -1;
		}
		else if(movingUp)
		{
			vertDir = 1;
		}

		if(patrolRight)
		{
			horDir = 1;
			facingRight = true;
		}
		else
		{
			horDir = -1;
			facingRight = false;
		}


		/*if(movingDown)
		{
			vertSpeed *= -1.0f;
		}

		if(!patrolRight)
		{
			facingRight = false;
			horSpeed *= -1.0f;
		}
		else
		{
			facingRight = true;
		}*/

		//transform.position += new Vector3(horSpeed, vertSpeed, 0);
		myRigidbody.velocity = new Vector2(horSpeed * horDir, vertSpeed * vertDir);
	}

	void Chase() {
		
		Vector3 moveVector = tomPos.position - transform.position;
		Vector3 moveDirection = moveVector.normalized;

		if(tomPos.position.x > transform.position.x)
		{
			facingRight = true;
		}
		else
		{
			facingRight = false;
		}

		//transform.position += new Vector3(horSpeed*moveDirection.x, vertSpeed*moveDirection.y, 0);
		myRigidbody.velocity = new Vector2(horSpeed * moveDirection.x * 2, vertSpeed * vertSpeed*moveDirection.y * 2);
		CheckChaseRange ();
		CheckCanAttack ();

		//Check if in position to attack Tom
		if(attacking)
		{
			StartAttack ();
		}
	}

	void CheckVerticalBounds() {
		if(movingUp && transform.position.y > ManagerScript.upperVertBounds)
		{
			movingUp = false;
			movingDown = true;
		}
		else if(movingDown && transform.position.y < ManagerScript.lowerVertBounds + transform.localScale.y * 0.5f)
		{
			movingDown = false;
			movingUp = true;
		}
	}

	void CheckChaseRange() {
		if(chasing && aware && !stunned)
		{
			if(Mathf.Abs(transform.position.x - tomPos.position.x) > ManagerScript.halfScreenWidth * 1.25f)//Out of range
			{
				aware = false;
				chasing = false;
				patrolling = true;
				patrolRight = facingRight;
				StopChasing ();
				StartPatrolling ();
			}
			else//In range
			{
				aware = true;
				chasing = true;
				patrolling = false;
				StopPatrolling ();
				StartChasing ();
			}
		}
	}

	void CheckCanAttack()
	{
		//if(Mathf.Abs(transform.position.x-tomPos.position.x) <= attackRange && Mathf.Abs(transform.position.y-tomPos.position.y) < attackRange)
		if(Vector3.Distance(transform.position, tomPos.position) <= attackRange)
		{
			attacking = true;
			myRigidbody.velocity = Vector2.zero;
			StopChasing ();
			StartAttack ();
		}
	}

	void AttackTom()
	{
		if(Vector3.Distance(transform.position, tomPos.position) <= attackRange)
		{
			PlayerScript ps = tomPos.GetComponent<PlayerScript>();
			if(!ps.IsDead()) shockSound.Play();
			ps.Shock(transform.position.x);
		}
		//StopAttack ();
	}

	void CheckStun()
	{
		if(hit)
		{
			aware = true;
			stunned = true;
			patrolling = false;
			chasing = false;
			stunTime = 2.0f;
			hit = false;
			StartStun ();
		}
		if(stunned)
		{
			stunTime -= Time.deltaTime;
			if(stunTime <= 0f)
			{
				stunned = false;
				aware = true;
				chasing = true;
				patrolling = false;
				StopStun ();
			}
		}
	}

	void StartChasing()
	{
		chasing = true;
		animator.SetBool("chasing", true);
		StopPatrolling ();
	}

	void StopChasing()
	{
		chasing = false;
		animator.SetBool("chasing", false);
	}

	void StartPatrolling()
	{
		patrolling = true;
		animator.SetBool("patrolling", true);
	}

	void StopPatrolling()
	{
		patrolling = false;
		animator.SetBool("patrolling", false);
	}

	void StartStun()
	{
		myRigidbody.velocity = Vector2.zero;
		stunned = true;
		animator.SetBool("stunned", true);
		StopPatrolling ();
		StopChasing ();
		StopAttack ();
	}

	void StopStun()
	{
		stunned = false;
		animator.SetBool("stunned", false);
	}

	void StartAttack()
	{
		animator.SetTrigger("attacking");
	}

	void StopAttack()
	{
		attacking = false;
		StartChasing ();
	}

	void SendAlert()
	{
		if (hit || chasing || stunned) {
			RaycastHit2D[] hits;
			hits = Physics2D.CircleCastAll (transform.position, ManagerScript.halfScreenWidth * 1.25f, Vector2.up);
			
			for (int i = 0; i < hits.Length; i++) {
				GameObject g = hits [i].collider.gameObject;

				if (g.tag == "Ghost" && !chasing) {
					g.GetComponent<GhostScript> ().Flee();
				}
				if (g.tag == "Watchman") {
					g.GetComponent<NeighborWatchScript>().aware = true;
					g.GetComponent<NeighborWatchScript>().chasing = true;
					g.GetComponent<NeighborWatchScript>().patrolling = false;
				}
				
			}
		}
	}
}
