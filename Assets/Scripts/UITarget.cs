using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITarget : MonoBehaviour {

    [SerializeField]
    Player _player;
    [SerializeField]
    GameObject _panel;
    [SerializeField]
    Slider _hp;
    [SerializeField]
    Text _name;
    [SerializeField]
    Text _hpText;

	void Update () {
        if (_player == null) return;

        if(_player._target != null && _player._target != _player)
        {
            _panel.SetActive(true);
            _hp.value = _player._target.Hp_Percent();
            _name.text = _player._target.ID + "(LV. " + _player._target.Level + ")";
            _hpText.text = _player._target.HP + "/" + _player._target.HPMax;
        }
        else
        {
            _panel.SetActive(false);
        }
	}
}
