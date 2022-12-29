using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bird : MonoBehaviour
{
    Rigidbody2D rb;
    Game game;
    float jumpTimer;

    public AI ai;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        game = FindObjectOfType<Game>();
        ai = GetComponent<AI>();
    }

    public Vector2 getVelocity()
    {
        return rb.velocity;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        game.Lost(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (ai.network == null)
        {
            return;
        }
        double[] outputs = ai.network.GetOutputs(game.GetInputs(this));
        if (outputs[0] > 0.5f)
        {
            Jump();
        }
        if (transform.position.y < -5.6f)
        {
            game.Lost(this);
        }
    }

    public void Jump()
    {
        if (Time.time > jumpTimer)
        {
            jumpTimer = Time.time + 0.1f; //can only jump once per 0.1s
            rb.velocity = new Vector2(0f, 4f);
        }
    }
}
