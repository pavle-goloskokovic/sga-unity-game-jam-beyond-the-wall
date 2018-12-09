using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkeletorTesStates : MonoBehaviour {

	public MeshFilter skeletorRenderer;
	public List<Mesh> skeletorStates;
	int frameInterval = 8;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(SkeletorCycle());
	}


    int state = 0;
	private IEnumerator SkeletorCycle()
	{
		while (true)
        {
			for (int i = 0; i < frameInterval; i++) {
				yield return new WaitForEndOfFrame();
		}
            skeletorRenderer.mesh = skeletorStates[state];
            state++;
            if (state == 5) state = 0;
            //do code
            Debug.Log("skeletor update");
		}

	}
	
}
