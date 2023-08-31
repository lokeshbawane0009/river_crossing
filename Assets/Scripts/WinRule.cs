using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct ActorSide
{
    public Actor actor;
    public bool onLeft;
}

public class WinRule : MonoBehaviour
{
    public List<ActorSide> actorSides = new List<ActorSide>();

    public bool CheckForWin()
    {
        foreach (ActorSide side in actorSides)
        {
            if (side.actor.OnLeft != side.onLeft)
            {
                return false;
            }
        }

        return true;
    }
}
