using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class FaceDetector : MonoBehaviour
{
    WebCamTexture camTex;

    CascadeClassifier faceEngine;
    CascadeClassifier smileEngine;
    OpenCvSharp.Rect faceRect;
    OpenCvSharp.Rect smileRect;

    public Vector2 faceCord;
    public Vector2 centerCord;
    public Vector2 lastCord;
    public bool reset;

    public GameObject cam;
    public GameObject gun;
    public Vector2 rotation;
    public Vector2 rotationSmooth;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        camTex = new WebCamTexture(devices[0].name);
        camTex.Play();

        // faceEngine = new CascadeClassifier(System.IO.Path.Combine(Application.dataPath + @"/haarcascade_frontalface_default.xml"));
        // smileEngine = new CascadeClassifier(System.IO.Path.Combine(Application.dataPath + @"/haarcascade_smile.xml"));
        faceEngine = new CascadeClassifier(System.IO.Directory.GetCurrentDirectory() + @"/haarcascade_frontalface_default.xml");
        smileEngine = new CascadeClassifier(System.IO.Directory.GetCurrentDirectory() + @"/haarcascade_smile.xml");
        
        rotation = cam.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.mainTexture = camTex;

        Mat tex = OpenCvSharp.Unity.TextureToMat(camTex);
        faceDetection(tex);
        moveObject();
    }

    void faceDetection(Mat frame){
        int[] faceRejectLevels;
        double[] faceLevelWeights;
        var faces = faceEngine.DetectMultiScale(frame, out faceRejectLevels, out faceLevelWeights, 1.1, 2, HaarDetectionType.ScaleImage, new Size(25, 25), new Size(1000, 1000),true);

        if (faces.Length >= 1)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                if (faceLevelWeights[i] >= 5){
                    
                    faceRect = faces[i];
                    frame.Rectangle(faceRect, new Scalar(255, 0, 0, 0), 3);
                    faceCord = new Vector2(faces[i].Y, faces[i].X);
                    if(!reset)
                    {
                        reset = true;
                        lastCord = faceCord;
                        centerCord = faceCord;
                    }
                    Mat faceArea = frame[faceRect];

                    int[] smileRejectLevels;
                    double[] smileLevelWeights;
                    var smiles = smileEngine.DetectMultiScale(faceArea, out smileRejectLevels, out smileLevelWeights, 1.16, 65, HaarDetectionType.ScaleImage, new Size(25, 25), new Size(1000, 1000),true);

                    if (smiles.Length >= 1)
                    {
                        for (int j = 0; j < smiles.Length; j++)
                        {
                            smileRect = smiles[j];
                            if (smileLevelWeights[j] >= 3.5f){
                                faceArea.Rectangle(smileRect, new Scalar(0, 0, 255, 0), 3);
                                gun.GetComponent<Shoot>().Shot();
                            }
                        }
                    }

                }
                else{
                    faceCord = centerCord;
                }

            }
        }


        Texture drawRect = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = drawRect;
    }

    // void resetCenter()
    // {
    //     centerX = faceX;
    //     centerY = faceY;
    // }

    void moveObject()
    {
        // Vector2 change = lastCord - faceCord;
        // change *= 0.1f;
        // lastCord = faceCord;
        // if(Mathf.Abs(change.x) > 0.01f){
        //     rotation.x -= change.x;
        // }
        // if(Mathf.Abs(change.y) > 0.01f){
        //     rotation.y += change.y;
        // }

        //Vector2 shift = centerCord - faceCord;
        //Vector2 shift = Vector2.ClampMagnitude((centerCord - faceCord)/10, 1.5f);
        Vector2 shift = (centerCord - faceCord)/2.5f;
        if(Mathf.Abs(shift.x) > 0.01f){
            //rotation.x -= shift.x*2;
            rotation.x = -shift.x*2;
        }
        if(Mathf.Abs(shift.y) > 0.01f){
            //rotation.y += shift.y;
            rotation.y = shift.y;
        }

        rotation.x = Mathf.Clamp(rotation.x, -85, 85);
        rotationSmooth = Vector2.Lerp(rotationSmooth, rotation, Time.deltaTime/2);
        cam.transform.localRotation = Quaternion.Euler(rotationSmooth);
        if(Input.GetKey(KeyCode.Space)){
            centerCord = faceCord;
            Debug.Log("!!!!!!!!!");
        }
    }
}
