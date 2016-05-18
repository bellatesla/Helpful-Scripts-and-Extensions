using UnityEngine;
using System.Collections;
using System.IO;

public class HiResScreenShots : MonoBehaviour
{

    public int resWidth = 1920; public int resHeight = 1080;

    [Range(1, 6)]
    public int enlarge = 1;

    public bool transparent = false;
    public string customDir= "/../Screenshots2/";//default "/../Screenshots/" but manually add any dir here
    public KeyCode screenshotKey = KeyCode.F8;
    private bool takeHiResShot;
    private TextureFormat transp = TextureFormat.ARGB32;
    private TextureFormat nonTransp = TextureFormat.RGB24;
    private int count;



    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }


    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown(screenshotKey);
        if (takeHiResShot)
        {

            TextureFormat textForm = nonTransp;
            if (transparent)textForm = transp;

            RenderTexture rt = new RenderTexture(resWidth * enlarge, resHeight * enlarge, 24);
            
            //Camera.main.targetTexture = rt;
            GetComponent<Camera>().targetTexture = rt;

            Texture2D screenShot = new Texture2D(resWidth * enlarge, resHeight * enlarge, textForm, false);
            
            //Camera.main.Render();
            GetComponent<Camera>().Render();

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth * enlarge, resHeight * enlarge), 0, 0);
            //Camera.main.targetTexture = null;
            GetComponent<Camera>().targetTexture = null;

            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth * enlarge, resHeight * enlarge);
            if (Directory.Exists(Application.dataPath + customDir) == false)
            {
                Directory.CreateDirectory(Application.dataPath + customDir);
            }
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Saved screenshot to: {0}", filename));

            takeHiResShot = false;
        }
    }
    public string ScreenShotName(int width, int height)
    {
        return string.Format("{0}{4}screen_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")+count++, customDir);
    }
}