using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabParentManager : MonoBehaviour
{
    [SerializeField] Transform Rig;
    [SerializeField] Transform Environment;
    public void SetParentOnGrab()
    {
        transform.SetParent(Rig);
    }
    public void SetParentOnRelease()
    {
        transform.SetParent(Environment);
    }
}