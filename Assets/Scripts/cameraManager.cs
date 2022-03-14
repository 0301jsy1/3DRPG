using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour {

    [SerializeField]
    private Transform target;
    private float distance;
    private float xSpeed, ySpeed;
    private float yMinLimit, yMaxLimit;
    private float x, y;
    private Vector3 position;
    private Quaternion rotation;

	void Awake() {
        distance = 10.0f;
        xSpeed = 250.0f;
        ySpeed = 120.0f;
        yMinLimit = 5.0f;
        yMaxLimit = 80.0f;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        rotation = Quaternion.Euler(y, x, .0f);
	}
	
	void Update () {

        if (!target) return; //모든 함수 단위를 종료시킴

        position = rotation * new Vector3(.0f, .0f, -distance) + target.position;

        transform.rotation = rotation;
        transform.position = position;

        //마우스 휠을 이용해 캐릭터와의 거리 값을 변경
        if(Input.GetAxis("Mouse ScrollWheel") > .0f)
        {
            if (distance > 2.0f) distance -= .2f;
            else distance = 2.0f;
        }

        if(Input.GetAxis("Mouse ScrollWheel")<.0f)
        {
            if (distance < 10.0f) distance += .2f;
            else distance = 10.0f;
        }

        if(Input.GetMouseButton(1))
        {
            //마우스 오른쪽 클릭이 캐릭터 이동에도 사용되기 때문에 왼쪽 Ctrl키를 눌렀을 때만 반응
            if (!Input.GetKey(KeyCode.LeftControl)) return;

            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit); 

            rotation = Quaternion.Euler(y, x, .0f); //이동은 x, y, 회전은 y, x
        }
	}

    float ClampAngle(float angle, float min, float max)
    {
        if(angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
