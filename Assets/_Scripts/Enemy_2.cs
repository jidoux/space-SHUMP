using UnityEngine;

public class Enemy_2 : Enemy {
    [Header("Set in Inspector: Enemy_2")]
    // Determines how much the Sine wave will affect movement
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;
    [Header("Set Dynamically: Enemy_2")]
    // Enemy_2 uses a Sin wave to modify a 2-point linear interpolation
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;
    void Start () {
        // Pick any point on the left side of the screen
        p0 = Vector3.zero; // b
        p0.x = -boundsCheck.camWidth - boundsCheck.radius;
        p0.y = Random.Range( -boundsCheck.camHeight, boundsCheck.camHeight );
        // Pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = boundsCheck.camWidth + boundsCheck.radius;
        p1.y = Random.Range( -boundsCheck.camHeight, boundsCheck.camHeight );
        // Possibly swap sides
        if (Random.value > 0.5f) {
            // Setting the .x of each point to its negative will move it to the other side of the screen
            p0.x *= -1;
            p1.x *= -1;
        }
        // Set the birthTime to the current time
        birthTime = Time.time; // c
    }
    public override void Move() {
        // Bézier curves work based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifeTime;
        // If u>1, then it has been longer than lifeTime since birthTime
        bool enemyHasFinishedItsLife = u > 1;
        if (enemyHasFinishedItsLife) {
            Destroy(this.gameObject );
            return;
        }
        // Adjust u by adding a U Curve based on a Sine wave
        u = u + sinEccentricity*(Mathf.Sin(u*Mathf.PI*2));
        // Interpolate the two linear interpolation points
        pos = (1-u)*p0 + u*p1;
    }
}

