using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InstructionStatement
{
    public string instruction;
    public Sprite assocaitedImage;
    public bool wrongTick;
}

public class Instruction : MonoBehaviour
{
    public List<InstructionStatement> instructions;

    private void Start()
    {
        UIManager.Instance.SetInstructions(this);
    }
}
