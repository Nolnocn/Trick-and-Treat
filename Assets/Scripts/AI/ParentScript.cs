
using UnityEngine;
using System.Collections;

public class ParentScript : EnemyScript
{

		// is the parent protecting a child
		private bool protecting;
		private bool attacking;
		private float moveSpeed;

		// the transform of the ghost kid which is being protected
		public Transform ghost;
		private Transform player;
		private float stunTime;
		private Vector2 targetPosition;
		private AudioSource zapSound;


		// Use this for initialization
		void Start ()
		{
				stunned = false;
				hit = false;
				facingRight = true;
				protecting = false;
				attacking = false;
		
				sortingOrderOffset = 1;
				spriteRenderer = GetComponent<SpriteRenderer> ();
				animator = GetComponent<Animator> ();
				player = ManagerScript.player;
				ghost = null;

				moveSpeed = 4.0f;
				stunTime = 0.0f;

				AudioSource[] audioSources = GetComponents<AudioSource> ();
				ballSound = audioSources [0];
				zapSound = audioSources [1];

		}
	
		// Update is called once per frame
		void Update ()
		{
				
				Move ();
				

				if(!GetComponent<Renderer>().isVisible)
				{
					GetComponent<Rigidbody2D>().velocity = Vector2.zero;
					animator.SetBool("moving", false);
				}
				Wrap ();
				SetSortingOrder ();
				ConstrainVertical ();
	
		}

		private void CalcTargetPosition ()
		{
				targetPosition = (player.position - ghost.position).normalized;
				targetPosition += (Vector2)ghost.position;
		}

		private void Move ()
		{
				CheckStun ();
				if (!stunned && !attacking && ghost != null) {
						if (!ghost.GetComponent<GhostScript> ().HasParent ()) {
								ghost = null;
						} else {
								CalcTargetPosition ();
								if (Vector2.Distance ((Vector2)ghost.position, (Vector2)transform.position) < 1.0f) {
										animator.SetBool ("protecting", true);
										protecting = true;
										CalcTargetPosition ();
										GetComponent<Rigidbody2D>().velocity = (targetPosition - (Vector2)transform.position) * moveSpeed;

										/*if (rigidbody2D.velocity.x > 0) {
										facingRight = true;
										transform.localScale = new Vector3 (1, 1, 1);
								} else {
										facingRight = false;
										transform.localScale = new Vector3 (-1, 1, 1);
								}*/
								} else {
										GetComponent<Rigidbody2D>().velocity = Vector2.zero;
										animator.SetBool("moving", false);
								}
								Attack ();
						}

						if (GetComponent<Rigidbody2D>().velocity.magnitude > 0.25f) {
								animator.SetBool ("moving", true);
						} else if ((!attacking && !stunned) || ghost == null) {
								GetComponent<Rigidbody2D>().velocity = Vector2.zero;
								animator.SetBool ("moving", false);
						}
						if (!stunned && !attacking) {
								if (player.position.x > transform.position.x) {
										transform.localScale = new Vector3 (1, 1, 1);
								} else {
										transform.localScale = new Vector3 (-1, 1, 1);
								}
						}
		

				}
		}

		private void CheckStun ()
		{
				if (hit) {
						stunned = true;
						StopProtect ();
						animator.SetTrigger ("hit");
						animator.SetBool ("stunned", true);
						animator.SetBool ("moving", false);
			
						stunTime = 3.0f;
						SendAlert ();
						hit = false;
				}
		
				if (stunned) {
						stunTime -= Time.deltaTime;
						if (stunTime <= 0.0f) {
								stunned = false;
								animator.SetBool ("stunned", false);
						}
				}
		}

		private void Attack ()
		{
				if (protecting && !stunned && Vector2.Distance (player.position, transform.position) < 1.25f && !player.GetComponent<PlayerScript> ().IsDead ()) {
						attacking = true;
						GetComponent<Rigidbody2D>().velocity = Vector2.zero;
						animator.SetBool ("moving", false);
						animator.SetTrigger ("attack");
				}
		}

		private void ShockPlayer ()
		{
				if (Vector2.Distance (player.position, transform.position) < 1.25f) {
						player.GetComponent<PlayerScript> ().Shock (transform.position.x);
						zapSound.Play ();
				}
		}

		private void StopAttack ()
		{
				attacking = false;
		}

		public void Protect (Transform g)
		{
				if (ghost == null) {
						ghost = g;
				}

		}

		public void StopProtect ()
		{
				animator.SetBool ("protecting", false);
				protecting = false;
				ghost = null;
				targetPosition = transform.position;
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}

		public bool IsProtecting ()
		{
				if (ghost == null) {
						return false;
				}
				return true;
		}

		private void SendAlert ()
		{

				RaycastHit2D[] hits;
				hits = Physics2D.CircleCastAll (transform.position, 100.0f, Vector2.up);
			
				for (int i = 0; i < hits.Length; i++) {
						GameObject g = hits [i].collider.gameObject;
				
						if (g.tag == "Ghost" && (hit)) {
								g.GetComponent<GhostScript> ().Flee ();
						}
						if (g.tag == "Watchman" && (hit)) {
								g.GetComponent<NeighborWatchScript> ().aware = true;
								g.GetComponent<NeighborWatchScript> ().chasing = true;
								g.GetComponent<NeighborWatchScript> ().patrolling = false;
						}
				
				}

		}

		public bool IsStunned ()
		{
				return stunned;
		}
}
