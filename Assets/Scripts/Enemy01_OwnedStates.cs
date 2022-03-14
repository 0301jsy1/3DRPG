//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy01.States
{
    //끊임없이 임의의 목표지점을 정해 배회하는 행동 정의
    public class Wander : State<Enemy>
    {
        static readonly Wander instance = new Wander();
        public static Wander Instance
        {
            get { return instance; }
        }

        public override void Enter(Enemy entity)
        {
            entity._target = null;
            //GlobalState Class의 Execute() 메소드에 보면 현재 Target이 없을 때만 주변에 다가오는 적을 감지
            //적을 감지해야 하는 상황은 배회하기 상태일 때이기 때문에 배회하기 상태로 들어갈 때 Target을 null로 설정
            UpdateWander(entity);
        }

        public override void Execute(Enemy entity)
        {
            Vector3 move_pos = Vector3.Normalize(entity.Wander_Target_Pos - entity.Get_Pos());
            entity.Add_Pos(move_pos * entity.move_speed * Time.deltaTime);
            Debug.DrawLine(entity.Get_Pos(), entity.Wander_Target_Pos);

            if(Vector3.Distance(entity.Wander_Target_Pos, entity.Get_Pos()) < 0.02f)
            {
                UpdateWander(entity);
            }
        }

        public override void Exit(Enemy entity)
        {

        }

        void UpdateWander(Enemy entity)
        {
            float wander_radius = 3.0f; //배회하기 반지름
            int wander_jitter = 0;
            int wander_jitter_min = 0;
            int wander_jitter_max = 360;

            Vector3 parent_pos = entity.transform.parent.transform.position;
            Vector3 parent_scale = entity.transform.parent.transform.localScale;

            wander_jitter = Random.Range(wander_jitter_min, wander_jitter_max);
            Vector3 target_pos = entity.Get_Pos() + Set_Angle(wander_radius, wander_jitter);

            target_pos.x = Mathf.Clamp(target_pos.x, parent_pos.x - parent_scale.x * 0.5f + 4.0f, parent_pos.x + parent_scale.x * 0.5f - 4.0f);
            target_pos.y = .0f;
            target_pos.z = Mathf.Clamp(target_pos.z, parent_pos.z - parent_scale.x * 0.5f + 4.0f, parent_pos.z + parent_scale.x * 0.5f - 4.0f);

            entity.Wander_Target_Pos = target_pos;
            entity.Anim.Play("Walk");
            entity.transform.localRotation = Quaternion.LookRotation(entity.Wander_Target_Pos - entity.Get_Pos());
        }

        Vector3 Set_Angle(float radius, int angle)
        {
            Vector3 pos = Vector3.zero;
            pos.x = Mathf.Cos(angle) * radius;
            pos.z = Mathf.Sin(angle) * radius;

            return pos;
        }
     
    }

    //적에게 맞앗을 때의 행동 정의
    public class Hit : State<Enemy>
    {
        static readonly Hit instance = new Hit();
        public static Hit Instance
        {
            get { return instance; }
        }

        public override void Enter(Enemy entity)
        {
            entity.Anim.Play(null);
            entity.Anim.Play("Hit_Front");

            entity._target = GameObject.Find("PC_01").GetComponent<Player>();

            int dmg = 0;
            if (entity.Hit_Type == 1)
                dmg = entity._target.Physics_Damage - entity.Physics_Defense;
            else if (entity.Hit_Type == 2)
                dmg = entity._target.Magic_Damage + entity._target.cur_skill.damage - entity.Magic_Defense;
             
            dmg = Mathf.Clamp(dmg, 1, 99999);
            //때린 상대의 공격력보다 내 방어력이 높을 경우엔 Damage가 음수여서 체력이 반대로 증가하게 되기 때문에
            //Damage는 1~99999 사이로 설정

            entity.HP -= dmg;
            entity.Damage_Popup("" + dmg);
            //맞았을 때 Damage_Popup()메소드에 깎이는 체력을 인자로 하여 호출

            if(entity.HP <= 0)
            {
                entity.ChangeState(Die.Instance);
            }
        }

        public override void Execute(Enemy entity)
        {
            if (entity.Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Base"))
                entity.ChangeState(Wander.Instance);
            //기존처럼 이저 상태로 되돌리는 메소드를 호출하면 
            //Hit 상태를 연속적으로 했을 경우에는 계속해서 Hit 상태만 호출하기 때문에 ChangeState를 사용
        }

        public override void Exit(Enemy entity)
        {
            entity.Hit_Type = 0;
        }
    }

    //상태의 소유주(entity)가 죽었을 때 행동 정의
    public class Die : State<Enemy>
    {
        Renderer[] renders;
        Color[] colors;

        static readonly Die instance = new Die();
        public static Die Instance
        {
            get { return instance; }
        }

        public override void Enter(Enemy entity)
        {
            entity.Anim.Play("Die");

            GameObject.Find("PC_01").GetComponent<Player>().Exp += entity.Level * 30;
            //entity._target.GetComponent<Player>().Exp += entity.Level * 30;
            entity.Set_TargetMarkOff();

            renders = entity.GetComponentsInChildren<Renderer>();
            colors = new Color[renders.Length];
            for(int i = 0; i < renders.Length; ++i)
            {
                colors[i] = renders[i].material.color;
                renders[i].material.shader = Shader.Find("Legacy Shaders/Transparent/VertexLit");
            }
        }

        public override void Execute(Enemy entity)
        {
            for(int i = 0; i < renders.Length; ++i)
            {
                if(colors[i].a > .0f)
                {
                    colors[i].a -= Time.deltaTime;
                    renders[i].material.color = colors[i];
                }
                else
                {
                    entity.transform.parent.GetComponent<FieldManager>().Delete_Enemy(entity);
                    return;
                }
            }
        }

        public override void Exit(Enemy entity)
        {
           
        }
    }

    //전역상태로 현재 진행중인 상태와 별개로 지속젹으로 검사를 수행하는 상태
    public class GlobalState : State<Enemy>
    {
        static readonly GlobalState instance = new GlobalState();
        public static GlobalState Instance
        {
            get { return instance; }    
        }

        public override void Enter(Enemy entity)
        {
            
        }

        public override void Execute(Enemy entity)
        {
            //현재는 Target(플레이어)이 한명이지만 여러명일땐 불러와서
            /*Entity targets = GameObject.Find("PC_01").GetComponent<Entity>();
            if ( targets._target == entity )
            {
                entity._target = targets;
                entity.ChangeState(Pursuit.Instance); //Pursuit : 추적하는 클래스
            }
            else
            {
                if (entity.GetCurrentState() != Wander.Instance )
                {
                    entity._target = null;
                    entity.ChangeState(Wander.Instance);
                }
            }*/

            if(entity._target == null)
            {
                //현재는 Target(플레이어)이 한명이지만 여러명일땐 불러와서
                Entity targets = GameObject.Find("PC_01").GetComponent<Entity>();

                //반복문을 이용해서 검사
                float dis = Vector3.Distance(targets.Get_Pos(), entity.Get_Pos());

                if(dis <= 4.0f)
                {
                    entity._target = targets;
                    entity.ChangeState(Pursuit.Instance); //Pursuit : 추적하는 클래스
                }
                //Collider를 이용해 작성해도 되고 지금과 같이 상태에 넣어 거리 기준으로 작성해도 됨
                //현재 Tatget이 없을 때 Target으로 삼을 수 있는 대상 모두와의 거리를 검사
                //거리 4(적 인식 범위)안에 들어와 있는 대상을 Targeet으로 설정하고 추적
            }
        }

        public override void Exit(Enemy entity)
        {
      
        }
    }

    //추적 클래스
    public class Pursuit : State<Enemy>
    {
        static readonly Pursuit instance = new Pursuit();
        public static Pursuit Instance
        {
            get { return instance; }
        }

        public override void Enter(Enemy entity)
        {
            entity.move_speed = 2.0f; //추적을 달려서 하기 때문에 entity의 속도를 2로 증가
            entity.Anim.Play("Run");
        }

        public override void Execute(Enemy entity)
        {
            float dis = Vector3.Distance(entity._target.Get_Pos(), entity.Get_Pos());

            //적 추적 범위를 벗어나면 추적 해제
            if(dis > 6.0f)
            {
                entity._target = null;
                entity.ChangeState(Wander.Instance);
            }
            else if(dis>0.5f) //추적
            {
                entity.transform.localRotation = Quaternion.LookRotation(entity._target.Get_Pos() - entity.Get_Pos());
                Vector3 move_dir = Vector3.Normalize(entity._target.Get_Pos() - entity.Get_Pos());
                entity.Add_Pos(move_dir * entity.move_speed * Time.deltaTime);
                Debug.DrawLine(entity.Get_Pos(), entity._target.Get_Pos());
                //추적할 때는 대상을 바라보도록 하고 해당 방향으로 동일한 속도(2)로 이동
            }
            else //추적 종료
            {
                if (entity._target != null)
                {
                    //Debug.Log("공격 상태로 변경"); 
                    //현재 공격하기 상태가 없기 때문에 Log만 출력하고 배회하기 상태로 변경
                    entity.ChangeState(Attack.Instance);
                    //entity.ChangeState(Wander.Instance);
                }
                else
                    entity.ChangeState(Wander.Instance);
            }
        }

        public override void Exit(Enemy entity)
        {
            entity.move_speed = 0.5f; //추적을 종료하게 되면 entity의 속도를 다시 0.5로 감소
        }
    }

    //상태의 소유주(entity)가 어떤 대상(Target)을 공격할 때 행동 정의
    public class Attack : State<Enemy>
    {
        static readonly Attack instance = new Attack();
        public static Attack Instance
        {
            get { return instance; }
        }

        public override void Enter(Enemy entity)
        {
            entity.Anim.Play(null);
            entity.Anim.Play("Enemy_Attack");
            entity.attack_state = 0;
        }

        public override void Execute(Enemy entity)
        {
            if(entity.attack_state == 1)
            {
                entity.attack_state = 0;
                Update_Physics_Attack(entity);
            }
            else if(entity.attack_state == 2)
            {
                End_Physics_Attack(entity);
            }
        }

        public override void Exit(Enemy entity)
        {
            
        }

        void Update_Physics_Attack(Enemy entity) //공격 타점이 플레이어에게 닿을 때 호출
        {
            if (entity._target == null) entity.ChangeState(Wander.Instance);
            Player player = entity._target.GetComponent<Player>() as Player;
            if (player == null) entity.ChangeState(Wander.Instance);
            player.Hit(entity);
        }

        void End_Physics_Attack(Enemy entity) //공격 애니메이션이 종료될 때 호출
        {
            if (entity._target != null)
            {
                float dis = Vector3.Distance(entity._target.Get_Pos(), entity.Get_Pos());

                if (dis <= 0.7f) entity.ChangeState(Attack.instance);
                else entity.ChangeState(Pursuit.Instance);
            }
            //타겟이 살아있으면 타겟과의 거리를 비교하여 공격 범위에 있으면 다시 공격하고 멀리 있으면 추격한다
            //타겟이 죽거나 사라졌다면 배회한다.
            else entity.ChangeState(Wander.Instance);
        }
    }
}