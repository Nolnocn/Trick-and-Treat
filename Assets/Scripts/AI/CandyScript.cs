using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (BoxCollider2D))]


public class CandyScript : MonoBehaviour {
	
	float lifespan;
	SpriteRenderer spriteRenderer;
	public float yLevel;
	public bool falling;
	public Sprite[] sprites;

	// Use this for initialization
	void Start ()
	{
		lifespan = 5.0f;
		spriteRenderer = GetComponent<SpriteRenderer>();
		GetComponent<Rigidbody2D>().AddForce(100 * new Vector2(Random.Range(-0.75f, 0.75f), Random.Range(0.5f, 1.0f)));
		GetComponent<Rigidbody2D>().AddTorque(Random.Range(-50.0f, 50.0f));
		falling = true;

		spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!falling)
		{
			lifespan -= Time.deltaTime;
		}
		if(lifespan <= 0.0f)
		{
			Destroy(this.gameObject);
		}
		if(transform.position.y < yLevel && GetComponent<Rigidbody2D>().velocity.y < 0 && falling)
		{
			falling = false;
			GetComponent<Rigidbody2D>().drag = 100;
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			GetComponent<Rigidbody2D>().gravityScale = 0.0f;
			yLevel = 0.0f;
			GetComponent<Rigidbody2D>().angularDrag = 100;
			InvokeRepeating("Flashing", 3.0f, 0.25f);
		}
	}

	private void Flashing()
	{
		//spriteRenderer.enabled = !spriteRenderer.enabled;
		Color color = spriteRenderer.color;
		color.a = color.a == 1 ? 0.5f : 1;
		spriteRenderer.color = color;
	}

	public void Initialize(float yLevel)
	{
		this.yLevel = Mathf.Max(ManagerScript.lowerVertBounds, Mathf.Min(ManagerScript.upperVertBounds - 0.5f, yLevel + Random.Range(-0.5f, 0.5f)));
	}
}
