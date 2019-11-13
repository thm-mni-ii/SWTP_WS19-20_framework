using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
	private GlobalManager globalCanvas;
	
    // Start is called before the first frame update
	void Start () {		
	globalCanvas = gameObject.GetComponent<GlobalManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void RegisterMethod(){

		globalCanvas.ToggleCanvas("login");


	}
	
}
