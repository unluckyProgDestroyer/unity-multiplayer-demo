using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool
{
    public GameObject GetObject();
    public void ReturnObject(GameObject gameObject);
    public void ExpandPool(int addition);
    public void FillPool();
}