using System;
using UnityEngine;
public class SimpleCam : MonoBehaviour
{
    const int waitWidth = 16;

    public int width = 640;
    public int height = 360;

 
    public float redMax = 255;
    public float redMin = 255;

    public float bluMax = 0;
    public float bluMin = 0;

    public float grnMax = 0;
    public float grnMin = 0;

    public Transform myObject;

    Color32[] pixels;

    WebCamTexture cam;
    Texture2D tex;

    Action update;
    void Start()
    {
        update = WaitingForCam;

        cam = new WebCamTexture(WebCamTexture.devices[0].name, width, height);
        cam.Play();
    }

    public void Update()
    {
       update();
    }
    void WaitingForCam()
    {
        if (cam.width > waitWidth)
        {
            width = cam.width;
            height = cam.height;
            pixels = new Color32[cam.width * cam.height];
            tex = new Texture2D(cam.width, cam.height, TextureFormat.RGBA32, false);
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = tex;
            update = CamIsOn;
        }
    }
    void CamIsOn()
    {
        int rowBegin = 0;
        int rowEnd = height;
        int colBegin = 0;
        int colEnd = width;
        
        if (cam.didUpdateThisFrame)
        {
            cam.GetPixels32(pixels);
            tex.SetPixels32(pixels);
            tex.Apply();

            
            float counter = 0;
            float xCount = 0;
            float yCount = 0;
            for (int row = rowBegin; row < rowEnd; row++)
            {
                int rowLocation = width * row;
                for (int col = colBegin; col < colEnd; col++)
                {
                    int colLocation = rowLocation + col;

                    Color32 myPixel = pixels[colLocation];
                   
                    if (myPixel.r < redMax &&
                       myPixel.r > redMin &&
                       myPixel.g < grnMax &&
                       myPixel.g > grnMin &&
                       myPixel.b < bluMax &&
                       myPixel.b > bluMin
                       )
                    {
                        xCount += row;
                        yCount += col;
                        counter++;
                    }
                }
            }

            if (counter > 0)
            {
                float avgCol = xCount / counter;
                float avgRow = yCount / counter;
                float x = avgCol * 16 / (width - 1) - 8;
                float y =(int) (avgRow * 9 / (height - 1) - 4.5);

                myObject.position = new Vector3(x, y, -1);
                
            }

        }
    }


}

