using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    [SerializeField]
    private Transform to_portal;

    public void Move_To(Player p)
    {
        p.Set_Pos(to_portal.position);
    }
}
