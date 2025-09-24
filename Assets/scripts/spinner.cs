using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get the transform component and rotate it 90 degrees on the y axis every second
        transform.Rotate(0,0, 90 * Time.deltaTime);
        
    }
}
