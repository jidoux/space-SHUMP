using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part is another serializable data storage class just like WeaponDefinition
/// </summary>
[System.Serializable]
public class Part {
    // These three fields need to be defined in the Inspector pane
    public string name;
    public float health;
    public string[] otherPartsThatProtectThis;
    // These two fields are set automatically in Start().
    // Caching like this makes it faster and easier to find these later
    [HideInInspector]
    public GameObject part;
    [HideInInspector]
    public Material matToShowDamage;
}


/// <summary>
/// Enemy_4 will start offscreen and then pick a random point on screen to
/// move to. Once it has arrived, it will pick another random point and
/// continue until the player has shot it down.
/// </summary>
public class Enemy_4 : Enemy {
    [Header("Set in Inspector: Enemy_4")]
    public Part[] shipParts;
    private Vector3 p0, p1; // The two points to interpolate
    private float timeStart; // Birth time for this Enemy_4
    private float duration = 4; // Duration of movement
    void Start() {
        // There is already an initial position chosen by Main.SpawnEnemy()
        // so add it to points as the initial p0 & p1
        p0 = p1 = pos;
        InitMovement();
        // Cache GameObject & Material of each Part in parts
        Transform t;
        foreach (Part shipPart in shipParts) {
            t = transform.Find(shipPart.name);
            if (t != null) {
                shipPart.part = t.gameObject;
                shipPart.matToShowDamage = shipPart.part.GetComponent<Renderer>().material;
            }
        }
    }
    void InitMovement() {
        p0 = p1; // Set p0 to the old p1
        // Assign a new on-screen location to p1
        float widMinRad = boundsCheck.camWidth - boundsCheck.radius;
        float hgtMinRad = boundsCheck.camHeight - boundsCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        // Reset the time
        timeStart = Time.time;
    }
    public override void Move() {
      // This completely overrides Enemy.Move() with a linear interpolation
        float u = (Time.time - timeStart) / duration;
        if (u >= 1) {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // Apply Ease Out easing to u
        pos = (1 - u) * p0 + u * p1; // Simple linear interpolation
    }

    // These two functions find a Part in parts based on name or GameObject
    Part FindPart(string n) {
        foreach (Part prt in shipParts) {
            if (prt.name == n) {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go) {
        foreach (Part prt in shipParts) {
            if (prt.part == go) {
                return (prt);
            }
        }
        return (null);
    }
    // These functions return true if the Part has been destroyed
    bool Destroyed(GameObject go) {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n) {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt) {
        if (prt == null) {
            // If no real ph was passed in
            return (true); // Return true (meaning yes, it was destroyed)
        }
        // Returns the result of the comparison: prt.health <= 0
        // If prt.health is 0 or less, returns true (yes, it was destroyed)
        return (prt.health <= 0);
    }
    // This changes the color of just one Part to red instead of the whole ship.
    void ShowLocalizedDamage(Material m) {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
    // This will override the OnCollisionEnter that is part of Enemy.cs.
    void OnCollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!boundsCheck.isOnScreen) {
                    Destroy(other);
                    break;
                }
                // Hurt this Enemy
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // f
                Part prtHit = FindPart(goHit);
                bool prtHitNotFound = prtHit == null;
                if (prtHitNotFound) {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // Check whether this part is still protected
                if (prtHit.otherPartsThatProtectThis != null) {
                    foreach (string s in prtHit.otherPartsThatProtectThis) {
                        // If one of the protecting parts hasn't been destroyed...
                        if (!Destroyed(s)) {
                            // ...then don't damage this part yet
                            Destroy(other); // Destroy the ProjectileHero
                            return; // return before damaging Enemy_4
                        }
                    }
                }
                // It's not protected, so make it take damage
                // Get the damage amount from the Projectile.type and Main.W_DEFS
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // Show damage on the part
                ShowLocalizedDamage(prtHit.matToShowDamage);
                if (prtHit.health <= 0) {
                  // Instead of destroying this enemy, disable the damaged part
                    prtHit.part.SetActive(false);
                }
                // Check to see if the whole ship is destroyed
                bool allDestroyed = true; // Assume it is destroyed
                foreach (Part prt in shipParts) {
                    bool aPartStillExists = !Destroyed(prt);
                    if (aPartStillExists) {
                        allDestroyed = false; // ...change allDestroyed to false
                        break; // & break out of the foreach loop
                    }
                }
                if (allDestroyed) {
                    // ...tell the Main singleton that this ship was destroyed
                    //Main.MainSingleton.ShipDestroyed(this);
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(other); // Destroy the ProjectileHero
                break;
        }
    }
}