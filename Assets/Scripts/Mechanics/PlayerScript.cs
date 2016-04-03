using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	public ManagerScript mngr;
	public GameObject ballPrefab;
	public Transform ballPos;
	public Transform suckPos;

	private Animator animator;
	private AudioSource vacuumSound;
	private AudioSource collectSound;
	private SpriteRenderer spriteRenderer;
	private bool facingRight;
	private bool moving, throwing, sucking, shocked, invul, taunt, dead;
	private float horSpeed, vertSpeed, yLevel, invulTime;
	private int lives, score;
	private Transform[] candyToSuck;
	private Transform childToSuck;
	private UIManager joeShit;

	private Rigidbody2D myRigidbody;
	
	void Start ()
	{
		animator = GetComponent<Animator>();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		vacuumSound = audioSources[0];
		collectSound = audioSources[1];
		spriteRenderer = GetComponent<SpriteRenderer>();
		facingRight = true;
		moving = false;
		throwing = false;
		shocked = false;
		invul = false;
		dead = false;
		invulTime = 2.0f;
		horSpeed = 5.0f;
		vertSpeed = 2.5f;
		lives = 3;
		score = 0;
		joeShit = Camera.main.GetComponent<UIManager>();

		myRigidbody = GetComponent<Rigidbody2D>();
	}

	void Update ()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
		if(!joeShit.paused)
		{
			HandleInput();
		}

		if(Input.GetButtonDown("Pause"))
		{
			joeShit.TogglePause();
			if(joeShit.paused)
			{
				StopSucking();
			}
		}
	}

	private void HandleInput()
	{
		if(!throwing && !sucking && !shocked && !taunt && !dead)
		{
			DoMovement();

			if(Input.GetButton("Throw"))
			{
				StartThrow();
			}
			else if(Input.GetButton("Suck"))
			{
				StartSucking();
			}
			else if(Input.GetButtonDown("Taunt"))
			{
				StartTaunt();
			}

			ConstrainVertical();
			SetSortingOrder();
		}
		else if(sucking)
		{
			Suck();
			if(Input.GetButtonUp("Suck"))
			{
				StopSucking();
			}
		}
		else if(shocked)
		{
			Bounce();
		}
		else if(taunt)
		{
			if(Input.GetButtonUp("Taunt"))
			{
				StopTaunt();
			}
		}
	}

	private void DoMovement()
	{
		if(facingRight && Input.GetAxisRaw("Horizontal") < 0)
		{
			facingRight = false;
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else if(!facingRight && Input.GetAxisRaw("Horizontal") > 0)
		{
			facingRight = true;
			transform.localScale = new Vector3(1, 1, 1);
		}
		
		myRigidbody.velocity = 
			new Vector2(horSpeed * Input.GetAxisRaw("Horizontal"), vertSpeed * Input.GetAxisRaw("Vertical"));
		mngr.ParalaxifyTrees(-horSpeed * 0.5f * Input.GetAxisRaw("Horizontal"));

		if(Input.GetAxisRaw("Horizontal") == 0 && myRigidbody.velocity.y == 0)
		{
			StopMoving();
		}
		else if(!moving)
		{
			moving = true;
			animator.SetBool("moving", true);
		}
	}

	private void ConstrainVertical()
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

	private void SetSortingOrder()
	{
		spriteRenderer.sortingOrder = Mathf.FloorToInt(100 - transform.position.y * 10);
	}

	private void StopMoving()
	{
		if(moving)
		{
			moving = false;
			myRigidbody.velocity = Vector2.zero;
			animator.SetBool("moving", false);
		}
	}

	private void StartThrow()
	{
		throwing = true;
		animator.SetTrigger("throw");
		StopMoving();
	}

	private void SpawnBall()
	{
		GameObject ball = Instantiate(ballPrefab, ballPos.position, Quaternion.identity) as GameObject;
		ball.GetComponent<ProjectileScript>().Initialize(facingRight ? 1 : -1, transform.position.y - transform.localScale.y * 0.5f);
	}

	private void EndThrow()
	{
		throwing = false;
	}

	private void StartSucking()
	{
		sucking = true;
		vacuumSound.Play();
		animator.SetBool("sucking", true);
		StopMoving();
		ArrayList candyToSuckTemp = new ArrayList();

		Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 2.5f);

		for(int i = 0; i < hits.Length; i++)
		{
			if(hits[i].gameObject.tag == "Candy")
			{
				Transform candy = hits[i].transform;
				int dir = facingRight ? 1 : -1;
				Vector3 relativePos = dir * (candy.position - transform.position);
				float angleOfAim = Mathf.Rad2Deg * Mathf.Atan2(relativePos.y,relativePos.x);

				if(Mathf.Abs(angleOfAim) <= 30)
				{
					CandyScript cs = candy.GetComponent<CandyScript>();
					if(!cs.falling)
					{
						candyToSuckTemp.Add(candy);
					}
				}
			}
			else if(hits[i].gameObject.tag == "Ghost")
			{
				Transform ghost = hits[i].transform;
				if(ghost.position.y <= transform.position.y + .25f && ghost.position.y >= transform.position.y - .25f)
				{
					if(facingRight)
					{
						if(ghost.position.x <= transform.position.x + 1.5f
						   && ghost.position.x >= transform.position.x + 0.5f)
						{
							childToSuck = ghost;
						}
					}
					else if(ghost.position.x >= transform.position.x - 1.5f
					        && ghost.position.x <= transform.position.x - 0.5f)
					{
						childToSuck = ghost;
					}
				}
			}
		}

		candyToSuck = (Transform[]) candyToSuckTemp.ToArray(typeof(Transform));
	}

	private void Suck()
	{
		if(childToSuck != null)
		{
			GhostScript gs = childToSuck.GetComponent<GhostScript>();
			if(!gs.GetDead())
			{
				gs.Capturing();
			}
			else
			{
				mngr.KidCaptured();
				childToSuck = null;
			}
		}

		for(int i = 0; i < candyToSuck.Length; i++)
		{
			if(candyToSuck[i] != null)
			{
				candyToSuck[i].GetComponent<Rigidbody2D>().AddForce(350 * (suckPos.position - candyToSuck[i].position).normalized);
				if(Vector3.Distance(candyToSuck[i].position, suckPos.position) < 0.05f)
				{
					collectSound.Play();
					Destroy(candyToSuck[i].gameObject);
					AddToScore(100);
				}
			}
		}
	}

	private void StopSucking()
	{
		if(sucking)
		{
			if(childToSuck != null)
			{
				childToSuck.GetComponent<GhostScript>().StopCapturing();
			}

			sucking = false;
			animator.SetBool("sucking", false);
			vacuumSound.Stop();
		}
	}

	public void Shock(float attackerXPos)
	{
		if(!invul && !dead)
		{
			shocked = true;
			invul = true;
			animator.SetBool("shocked", true);
			joeShit.ShockedUI();
			if(throwing)
			{
				throwing = false;
			}
			StopMoving();
			StopSucking();
			StopTaunt();
			
			yLevel = transform.position.y;
			myRigidbody.gravityScale = 1.0f;
			myRigidbody.velocity = 5 * new Vector2(attackerXPos <= transform.position.x ? 1 : -1, 1).normalized;
			StartCoroutine(StopShock());
		}
	}

	private IEnumerator StopShock()
	{
		yield return new WaitForSeconds(1.0f);
		shocked = false;
		animator.SetBool("shocked", false);
		myRigidbody.gravityScale = 0.0f;
		joeShit.ShockedUI();
		SubtractLife();
	}

	private void StartTaunt()
	{
		animator.SetTrigger("startTaunt");
		taunt = true;
		animator.SetBool("taunt", true);
		StopMoving();
	}

	private void StopTaunt()
	{
		if(taunt)
		{
			taunt = false;
			animator.SetBool("taunt", false);
		}
	}

	private void StartInvul()
	{
		invul = true;
		StartCoroutine(InvulTimer());
	}

	private IEnumerator InvulTimer()
	{
		for(int i = 0; i < invulTime * 10; i++)
		{
			yield return new WaitForSeconds(0.1f);
			Color color = spriteRenderer.color;
			color.a = color.a == 1 ? 0.5f : 1;
			spriteRenderer.color = color;
		}
		invul = false;
		Color colorFinal = spriteRenderer.color;
		colorFinal.a = 1;
		spriteRenderer.color = colorFinal;
	}

	private void Bounce()
	{
		if(transform.position.y < yLevel && myRigidbody.velocity.y < 0)
		{
			myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -0.5f * myRigidbody.velocity.y);
		}
	}

	private void SubtractLife()
	{
		lives--;
		joeShit.LoseLife();
		if(lives <= 0)
		{
			myRigidbody.velocity = Vector2.zero;
			dead = true;
			animator.SetBool("DEAD", true);
		}
		else
		{
			StartInvul();
		}
	}

	private void AddToScore(int amt)
	{
		score += amt;
		joeShit.SetScore(score);
	}

	public bool IsDead()
	{
		return dead;
	}

	private void GameOver()
	{
		joeShit.GameOver();
	}
}