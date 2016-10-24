using UnityEngine;
using System.Collections;

public class SnapAppController : MonoBehaviour {
    Texture snapDisplay;
    Transform topLeftAnchor;
    Transform bottomRightAnchor;

    MainCameraController mainCam;
    InputController ioController;

	// Use this for initialization
	void Start () {
        snapDisplay = transform.Find("SnapDisplay").GetComponent<MeshRenderer>().material.mainTexture;
        topLeftAnchor = transform.Find("SnapTopLeft");
        bottomRightAnchor = transform.Find("SnapBottomRight");
        mainCam = GameObject.Find("MainController").GetComponent<MainCameraController>();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "SnapButton":
                        StartCoroutine(TakeSnap());
                        break;
                    case "SnapBackButton":
                        ioController.RevertToNormal();
                        GameObject.Destroy(this.gameObject);
                        break;
                }
            }
        }
    }

    IEnumerator TakeSnap()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        mainCam.SetTargetTexture(renderTexture);
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        mainCam.SecondaryRender();
        RenderTexture.active = renderTexture;

        var topLeftScreen = mainCam.WorldToScreen(topLeftAnchor.position);
        var bottomRightScreen = mainCam.WorldToScreen(bottomRightAnchor.position);
        var width = bottomRightScreen.x - topLeftScreen.x;
        var height = topLeftScreen.y - bottomRightScreen.y;

        screenShot.ReadPixels(
            new Rect(topLeftScreen.x, Screen.height - topLeftScreen.y, width, height),
            0, 0);
        screenShot.Apply();

        // For saving the screenshot
        // Destroy(renderTexture);
        // byte[] bytes = screenShot.EncodeToPNG();
        // string filename = "newScreenshot.png";
        // System.IO.File.WriteAllBytes(filename, bytes);
        // Debug.Log(string.Format("Took screenshot to: {0}", filename));

        snapDisplay = screenShot;
        // snapDisplay.SetTexture("NewDisplay", snapDisplayTexture);
    }
}
