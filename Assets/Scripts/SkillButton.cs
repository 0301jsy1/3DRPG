using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {

    [SerializeField]
    GameObject[] btns;
    [SerializeField] Player player;
    [SerializeField]
    GridLayoutGroup grid;
	// Use this for initialization
	void Start () {
        //player.skills[1].level++; //FireBall 스킬에 대한 레벨업 코드
        for (int i = 0; i < btns.Length; ++i)
        {
            btns[i].SetActive(false);
        }
    }
    void Update()
    {
        OnPanel();
    }
    public void OnPanel()
    {
        if (player.Level <= 4)
            grid.constraintCount = player.Level;
        
        for (int i = 0; i < btns.Length; ++i)
        {
            if (i >= player.Level) return;

            btns[i].SetActive(true);
        }
    }
    public void OnSkill(int number)
    {
        if ( number < player.Level )
            player.BeforeCastSkill(player.skills[number]);
    }
}
