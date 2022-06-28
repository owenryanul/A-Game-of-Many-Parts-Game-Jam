using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EnemySplitProjectile : Oryan_EnemyProjectile
{
    [Header("Split Projectile")]
    public GameObject splitProjectile;
    public bool forbidChildSpliting = true;
    public bool splitRight;
    public int shotsToSplitRight;
    public float maxSplitRightAngle;
    public bool splitLeft;
    public int shotsToSplitLeft;
    public float maxSplitLeftAngle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        spawnSplitProjectiles();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void spawnSplitProjectiles()
    {
        if (splitLeft)
        {
            Debug.Log("Spawning Split");
            for (int i = 0; i <= shotsToSplitLeft; i++)
            {
                Debug.Log("Spawning");
                Vector3 pos = this.gameObject.transform.position;
                Vector3 targetDirection = Quaternion.Euler(0, 0, i * (maxSplitLeftAngle / shotsToSplitLeft)) * base.targetDirection;
                GameObject splitProjectile = Instantiate(this.gameObject, pos, this.gameObject.transform.rotation);
                this.parentEnemy.onGrandchildProjectileCreated();
                splitProjectile.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this.parentEnemy);
                splitProjectile.GetComponent<Oryan_EnemyProjectile>().fireInDirection(targetDirection);
                if (forbidChildSpliting)
                {
                    splitProjectile.GetComponent<Oryan_EnemySplitProjectile>().splitLeft = false;
                    splitProjectile.GetComponent<Oryan_EnemySplitProjectile>().splitRight = false;
                }
            }
        }

        if (splitRight)
        {
            Debug.Log("Spawning Split");
            for (int i = 0; i <= shotsToSplitRight; i++)
            {
                Debug.Log("Spawning");
                Vector3 pos = this.gameObject.transform.position;
                Vector3 targetDirection = Quaternion.Euler(0, 0, i * -(maxSplitRightAngle / shotsToSplitRight)) * base.targetDirection;
                GameObject splitProjectile = Instantiate(this.gameObject, pos, this.gameObject.transform.rotation);
                this.parentEnemy.onGrandchildProjectileCreated();
                splitProjectile.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this.parentEnemy);
                splitProjectile.GetComponent<Oryan_EnemyProjectile>().fireInDirection(targetDirection);
                if (forbidChildSpliting)
                {
                    splitProjectile.GetComponent<Oryan_EnemySplitProjectile>().splitLeft = false;
                    splitProjectile.GetComponent<Oryan_EnemySplitProjectile>().splitRight = false;
                }
            }
        }
    }
}
