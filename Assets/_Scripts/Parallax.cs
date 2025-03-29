using UnityEngine;

public class Parallax : MonoBehaviour {
    [Header("Set in Inspector")]
    public GameObject playerShip;
    public GameObject[] scrollingForegrounds;
    public float scrollSpeed = -30f;
    // motionMult controls how much panels react to player movement
    public float motionMult = 0.25f;
    private float heightOfEachPanel;
    private float depthOfEachPanel;
    void Start() {
        heightOfEachPanel = scrollingForegrounds[0].transform.localScale.y;
        depthOfEachPanel = scrollingForegrounds[0].transform.position.z;
        // Set initial positions of panels
        scrollingForegrounds[0].transform.position = new Vector3(0, 0, depthOfEachPanel);
        scrollingForegrounds[1].transform.position = new Vector3(0, heightOfEachPanel, depthOfEachPanel);
    }
    void Update() {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % heightOfEachPanel + (heightOfEachPanel * 0.5f);
        if (playerShip != null)
        {
            tX = -playerShip.transform.position.x * motionMult;
        }
        // Position scrollingForegrounds[0]
        scrollingForegrounds[0].transform.position = new Vector3(tX, tY, depthOfEachPanel);
        // Then position scrollingForegorunds[1] where needed to make a continuous starfield
        if (tY >= 0) {
            scrollingForegrounds[1].transform.position = new Vector3(tX, tY - heightOfEachPanel, depthOfEachPanel);
        }
        else {
            scrollingForegrounds[1].transform.position = new Vector3(tX, tY + heightOfEachPanel, depthOfEachPanel);
        }
    }
}
