using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Oryan_ProjectileParent
{
    public void childProjectileDestroyed();

    public void childProjectileDamagedPlayer();

    public void onGrandchildProjectileCreated();

    public bool hasChildProjectilesDamagedPlayerThisAttack();
}
