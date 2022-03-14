using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHPMP : MonoBehaviour {

    [SerializeField]
    Player _player;
    [SerializeField]
    Slider _hp;
    [SerializeField]
    Slider _mp;
    [SerializeField]
    Text _hpText;
    [SerializeField]
    Text _mpText;
	
	void Update () {
        if (_player.transform == null) return;

        _hp.value = _player.Hp_Percent();
        _hpText.text = _player.HP + "/" + _player.HPMax;
        _mp.value = _player.Mp_Percent();
        _mpText.text = _player.MP + "/" + _player.MPMax;
	}
}
