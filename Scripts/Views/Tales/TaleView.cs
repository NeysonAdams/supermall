using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TaleView : MonoBehaviour
{
    public TaleModel Model{get; set;}
    

    public virtual void Init(TaleModel model, TaleModel[,] models)
    {
        Model = model;
    }

}
