using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//필드는 구역별로 1~5레벨로 나뉘게 되며 생성되는 적의 레벨은 구역 레벨에 따라 10단위로 상승, 1구역 : 1~10, 2구역 : 11~20
public class FieldManager : MonoBehaviour {

    [SerializeField]
    GameObject owner_field;
    [SerializeField]
    GameObject[] enemys;
    [SerializeField, Range(1, 5)]
    int field_level;

    List<Enemy> enemy_list;
    int max_enemy;

    void Awake()
    {
        enemy_list = new List<Enemy>();
        max_enemy = 5;

        for(int i = 0; i < 2; ++i)
        {
            Create_Enemy();
        }
        InvokeRepeating("Update_FieldEnemy", 1.0f, 5.0f);
    }

    void Update_FieldEnemy()
    {
        if (enemy_list.Count < max_enemy)
            Create_Enemy();
    }

    void Create_Enemy()
    {
        int enemy_type = Random.Range(0, enemys.Length);
        GameObject clone = Instantiate(enemys[enemy_type]) as GameObject;
        clone.transform.SetParent(transform);

        Enemy enemy = clone.GetComponent<Enemy>() as Enemy;
        float fx = transform.position.x + Random.Range(-transform.localScale.x * 0.5f + 4.0f, transform.localScale.x * 0.5f - 4.0f);
        float fz = transform.position.z + Random.Range(-transform.localScale.z * 0.5f + 4.0f, transform.localScale.z * 0.5f - 4.0f);
        enemy.Set_Pos(new Vector3(fx, .0f, fz));
        enemy.Init(Random.Range((field_level - 1) * 10 + 1, field_level * 10 + 1));
        enemy_list.Add(enemy);
    }

    public void Delete_Enemy(Enemy _enemy)
    {
        enemy_list.Remove(_enemy);
        Destroy(_enemy.gameObject);
    }
}
