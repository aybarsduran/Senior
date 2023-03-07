using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State //: MonoBehaviour you can stick on a game object but state is normal class
//abstract means that you cant create a state, but oyu can inheritor and crreate a jumping state or sth like that.

{
    public abstract void Enter();
    public abstract void Tick(float deltaTime);
    public abstract void Exit();

}
