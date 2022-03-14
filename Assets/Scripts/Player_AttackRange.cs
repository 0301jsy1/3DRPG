using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackRange : MonoBehaviour {

    [SerializeField]
    Player owner;

    void OnTriggerEnter(Collider col)
    {
        if (!col.name.Contains("Enemy")) return;
        if (owner._target == null) return;
        if (col.GetComponent<Entity>() != owner._target) return;

        owner.Base_Attack();
    }
}
