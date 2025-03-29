using System;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero HeroSingleton;
    [Header("Set in Inspector")]
    // these fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [SerializeField]
    private float _shieldLevel = 1;
    private GameObject lastTrigger = null; // ig its nullable by default...
    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Create a WeaponFireDelegate field named fireDelegate.
    public WeaponFireDelegate fireDelegate;


    public float shieldLevel {
        get {
            return _shieldLevel;
        }
        set {
            // ensuring its bounded on top and bottom
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0) {
                Destroy(this.gameObject);
                Main.MainSingleton.DelayedRestart(gameRestartDelay);
            }
        }
    }
    void Awake() {
        if (HeroSingleton == null) {
            HeroSingleton = this;
        } else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        // fireDelegate += TempFire;
    }

    void Start() {
        
    }

    void Update() {
        // pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        ChangeShipPosition(xAxis, yAxis);
        RotateShip(xAxis, yAxis);

        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     TempFire();
        // }
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }

    }

    void ChangeShipPosition(float xAxis, float yAxis) {
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
    }

    void RotateShip(float xAxis, float yAxis) {
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
    }

    // void TempFire() {
    //     GameObject projectile = Instantiate<GameObject>(projectilePrefab);
    //     projectile.transform.position = transform.position;
    //     Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
    //     // projectileRigidbody.velocity = Vector3.up * projectileSpeed; // it changed from this to below line, interesting
    //     // projectileRigidbody.linearVelocity = Vector3.up * projectileSpeed;
    //     Projectile proj = projectile.GetComponent<Projectile>();
    //     proj.type = WeaponType.blaster;
    //     float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
    //     projectileRigidbody.linearVelocity = Vector3.up * tSpeed;
    // }

    void OnTriggerEnter(Collider other) {
        Transform rootTransform = other.gameObject.transform.root;
        GameObject rootTrigger = rootTransform.gameObject;
        if (rootTrigger == lastTrigger) {
            return;
        }
        lastTrigger = rootTrigger;
        if (rootTrigger.tag == "Enemy") {
            // if shield was destroyed by an enemy
            shieldLevel--; // Decrease the level of the shield by 1
            print(string.Format("Shield level went down {0}", shieldLevel));
            Destroy(rootTrigger); // if its the root game object it deletes all children too, intuitively
        }
        else {
            print("Triggered by non-Enemy: " + rootTrigger.name);
        }
    }
}
