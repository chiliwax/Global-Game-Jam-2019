using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaidAnimation : MonoBehaviour
{
    public Animator animator = null;
    private float x;
    private float y;

    void start(){
        x = this.transform.position.x;
        y = this.transform.position.y;
    }
    void Update()
    {
        ///ANIMATOR     
        if (Mathf.Abs(this.transform.position.x - x) > Mathf.Abs(this.transform.position.y - y)) {
        if (this.transform.position.x > x )
            animator.SetInteger("Way", 1);
        else if (this.transform.position.x < x )
            animator.SetInteger("Way", 3);
        }
        else if (Mathf.Abs(this.transform.position.x - x) < Mathf.Abs(this.transform.position.y - y)) {
        if (this.transform.position.y > y)
            animator.SetInteger("Way", 4);
        else if (this.transform.position.y < y)
            animator.SetInteger("Way", 2);
        }
        else
            animator.SetInteger("Way", 0);

        x = this.transform.position.x;
        y = this.transform.position.y;
        ///  
    }
}
