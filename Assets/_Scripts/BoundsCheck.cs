using UnityEngine;

// To type the next 4 lines, start by typing /// and then Tab.
/// <summary>
/// Checks whether a GameObject is on screen and can force it to stay on screen.
/// Note that this ONLY works for an orthographic Main Camera.
/// </summary>
public class BoundsCheck : MonoBehaviour {

    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;
    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;
    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    void Awake() {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    // ensuring that this screen-bounding logic occurs immediately after update() is called
    // still executes every frame
    void LateUpdate() {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        if (pos.x > camWidth - radius) {
            pos.x = camWidth - radius;
            offRight = true;
            isOnScreen = false;
        }
        if (pos.x < -camWidth + radius) {
            pos.x = -camWidth + radius;
            offLeft = true;
            isOnScreen = false;
        }
        if (pos.y > camHeight - radius) {
            pos.y = camHeight - radius;
            offUp = true;
            isOnScreen = false;
        }
        if (pos.y < -camHeight + radius) {
            pos.y = -camHeight + radius;
            offDown = true;
            isOnScreen = false;
        }
        isOnScreen = !(offRight || offLeft || offUp || offDown);
        if (keepOnScreen && !isOnScreen) {
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }

    // Draw the bounds in the Scene pane using OnDrawGizmos()
    void OnDrawGizmos() {
        if (!UnityEngine.Application.isPlaying) {
            return;
        }
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
