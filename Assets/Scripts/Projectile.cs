using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Entity caster;
    Entity target;
    int damage;
    float aoeRadius;

    float speed;

    public void Init(Entity _caster, Entity _target, int _damage, float _aoeRadius)
    {
        caster = _caster;
        target = _target;
        damage = _damage;
        aoeRadius = _aoeRadius;

        speed = 0.1f;
    }

    void LateUpdate()
    {
        if (target != null && caster != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.Get_Pos()+new Vector3(0, .5f, 0), speed);
            transform.LookAt(target.Get_Pos() + new Vector3(0, .5f, 0));

            if (Vector3.Distance(transform.position, target.Get_Pos() + new Vector3(0, .5f, 0)) < .3f)
            {
                //맞았을 때 effect hit skill 표현
                GameObject clone = Instantiate(caster.cur_skill.effectHitSkill) as GameObject;
                clone.transform.position = target.Get_Pos() + new Vector3(0, .5f, 0);
                Destroy(clone, .5f);

                //multysplash 적용시 구간

                if (target.HP > 0)
                {  
                    caster.DealDamageAt(target, damage, aoeRadius);
                }  

                Destroy(gameObject);
            }
        }
        else Destroy(gameObject);
    }
}
