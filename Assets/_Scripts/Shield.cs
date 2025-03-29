using UnityEngine;

public class Shield : MonoBehaviour {

    [Header("Set in Inspector")]
    public float rotationsPerSecond = 0.1f;
    [Header("Set Dynamically")]
    public int levelShown = 0;
    // private variable so wont appear in the inspector. Ig they're proving a point
    // with this but it not being explicitly private is surely horrible design right?
    Material mat;
    void Start() {
        mat = GetComponent<Renderer>().material;
    }
    void Update() {
        // Read the current shield level from the Hero Singleton
        int currLevel = Mathf.FloorToInt(Hero.HeroSingleton.shieldLevel);
        // If this is different from levelShown...
        if (levelShown != currLevel) {
            levelShown = currLevel;
            // Adjust the texture offset to show different shield level
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }
        // Rotate the shield a bit every frame in a time-based way
        float rZ = -(rotationsPerSecond * Time.time * 3600) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
