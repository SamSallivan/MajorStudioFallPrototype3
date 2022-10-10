using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTest : MonoBehaviour
{
    WebCamTexture camTex;
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        camTex = new WebCamTexture(devices[0].name);
        camTex.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.mainTexture = camTex;
        
    }
}
