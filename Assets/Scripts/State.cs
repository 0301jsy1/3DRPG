using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 월드에 존재하는 모든 상태들을 상속받아 사용하는 추상 클래스
 * 일반화 프로그래밍(Generic Programming)을 활용해 Entity 클래스를 상속받는 오브젝트들은 모두 사용 가능하게 함
 */
public abstract class State<T> where T : Entity
{
    public abstract void Enter(T entity);
    public abstract void Execute(T entity);
    public abstract void Exit(T entity);
}
