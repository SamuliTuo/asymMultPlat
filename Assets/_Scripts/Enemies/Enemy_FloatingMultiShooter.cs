using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloatingShooterShootDirection {
    NULL,
    LEFT,
    RIGHT,
    UP,
    DOWN,
    HORIZONTAL,
    VERTICAL,
    CARDINAL,
    HALF_CARDINAL,
    CARDINALS_ALTERNATING
}

public class Enemy_FloatingMultiShooter : MonoBehaviour {

    [SerializeField] private FloatingShooterShootDirection type = FloatingShooterShootDirection.NULL;
    [SerializeField] private float shootCooldown = 1;
    [SerializeField] private float shootForce = 1;

    List<Vector3> halfCardinals = new List<Vector3>();
    bool alternating = false;
    private GameObject projectile;


    void Start() {
        Vector3 v1 = Vector3.left;
        Vector3 v2 = Vector3.right;
        Vector3 v3 = Vector3.up;
        Vector3 v4 = Vector3.down;
        v1 = Quaternion.Euler(0, 0, -45) * v1;
        v2 = Quaternion.Euler(0, 0, -45) * v2;
        v3 = Quaternion.Euler(0, 0, -45) * v3;
        v4 = Quaternion.Euler(0, 0, -45) * v4;
        halfCardinals.Add(v1);
        halfCardinals.Add(v2);
        halfCardinals.Add(v3);
        halfCardinals.Add(v4);
        projectile = Resources.Load("enemies/projectiles/EnemyProjectile_basic") as GameObject;

        StartCoroutine(Shoot());
    }

    public void GotHit() {
        //Gotta have this. Use for something or not.
    }

    IEnumerator Shoot() {
        yield return new WaitForSeconds(shootCooldown);
        foreach (var dir in ChooseDirections()) {
            GameObject clone = Instantiate(projectile, transform.position, Quaternion.identity);
            clone.GetComponent<Rigidbody>().AddForce(dir * shootForce, ForceMode.Impulse);
        }
        StartCoroutine(Shoot());
    }

    List<Vector3> ChooseDirections() {
        List<Vector3> r = new List<Vector3>();
        switch (type) {
            case FloatingShooterShootDirection.NULL:
                break;
            case FloatingShooterShootDirection.LEFT:
                r.Add(Vector3.left);
                break;
            case FloatingShooterShootDirection.RIGHT:
                r.Add(Vector3.right);
                break;
            case FloatingShooterShootDirection.UP:
                r.Add(Vector3.up);
                break;
            case FloatingShooterShootDirection.DOWN:
                r.Add(Vector3.down);
                break;
            case FloatingShooterShootDirection.HORIZONTAL:
                r.Add(Vector3.left);
                r.Add(Vector3.right);
                break;
            case FloatingShooterShootDirection.VERTICAL:
                r.Add(Vector3.up);
                r.Add(Vector3.down);
                break;
            case FloatingShooterShootDirection.CARDINAL:
                r.Add(Vector3.left);
                r.Add(Vector3.right);
                r.Add(Vector3.up);
                r.Add(Vector3.down);
                break;
            case FloatingShooterShootDirection.HALF_CARDINAL:
                r.Add(halfCardinals[0]);
                r.Add(halfCardinals[1]);
                r.Add(halfCardinals[2]);
                r.Add(halfCardinals[3]);
                break;
            case FloatingShooterShootDirection.CARDINALS_ALTERNATING:
                alternating = !alternating;
                if (alternating) {
                    r.Add(halfCardinals[0]);
                    r.Add(halfCardinals[1]);
                    r.Add(halfCardinals[2]);
                    r.Add(halfCardinals[3]);
                }
                else {
                    r.Add(Vector3.left);
                    r.Add(Vector3.right);
                    r.Add(Vector3.up);
                    r.Add(Vector3.down);
                }
                break;
            default:
                break;
        }
        return r;
    }
}
