using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck boundsCheck;
    private Renderer rend;
    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;
    // This public property masks the field _type and takes action when it is set
    public WeaponType type {
        get {
            return (_type);
        }
        set {
            SetType(value);
        }
    }
    void Awake() {
        boundsCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>(); // d
        rigid = GetComponent<Rigidbody>();
    }
    void Update() {
        bool projectileIsOutOfBoundsOnTop = boundsCheck.offUp;
        if (projectileIsOutOfBoundsOnTop) {
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// Sets the _type private field and colors this projectile to match the
    /// WeaponDefinition.
    /// </summary>
    /// <param name="eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType) {
        // Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}