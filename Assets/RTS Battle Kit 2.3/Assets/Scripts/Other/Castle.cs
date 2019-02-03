using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Castle : MonoBehaviour {
	
	//variables visible in the inspector
	public float lives;
	public float size;
	public GameObject fracture;
	
	void Update(){
	//destroy castle when lives are 0
	    if(lives <= 0f){
		    lives = 0;
		    /***
		    if a fractured version of this part of the castle has been assigned, instantiate it in order to have a cool destruction effect (not recommended on mobile devices)
		    if(fracture && gameObject.name != "Castle gate" && gameObject.name != "1"){
			    GameObject obj = Instantiate(fracture, transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0));
                    obj.transform.parent = GameObject.Find("GameObject3").transform;
                    obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); 
            } else if(fracture){
                GameObject obj = Instantiate(fracture, transform.position, Quaternion.Euler(0, 0, 0));
                    obj.transform.parent = GameObject.Find("GameObject3").transform;
                    obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		    }
		    **/
		    Destroy(gameObject);
            CombatController.remainCastle -= 1;
	    }
	}
}
