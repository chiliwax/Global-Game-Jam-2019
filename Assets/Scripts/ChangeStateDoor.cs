using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStateDoor : MonoBehaviour
{
    public BoxCollider2D captionDoor = null;
    public BoxCollider2D BlockDoor = null;
    public Sprite open = null;
    public Sprite closed = null;
    public float cooldown = 1;
    private float TimeLeft = 0;
    private SpriteRenderer Render = null;
    private bool toogle = false;

    // Start is called before the first frame update
    void Start()
    {
        Render = GetComponent<SpriteRenderer>();
        Render.sprite = closed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            toogle = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            toogle = false;
    }

    void Update()
    {
        float isAction = Input.GetAxis("Action");

        if (isAction > 0 && TimeLeft == 0 && toogle)
        {
            if (Render.sprite == open)
            {
                Render.sprite = closed;
                BlockDoor.enabled = true;
            }
            else
            {
                Render.sprite = open;
                BlockDoor.enabled = false;
            }
            TimeLeft = cooldown;
        }
        TimeLeft = Mathf.Clamp(TimeLeft - Time.fixedDeltaTime, 0, cooldown);
    }
}
