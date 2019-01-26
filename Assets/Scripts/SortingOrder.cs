using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrder : MonoBehaviour
{
    // Start is called before the first frame update
    public int layerOrder = 5;
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().sortingOrder = layerOrder;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
