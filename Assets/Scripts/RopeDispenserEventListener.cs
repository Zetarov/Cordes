using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeDispenserEventListener : MonoBehaviour
{
    public virtual void OnActivatedChanged(bool on)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeSinceLevelLoad">The time of the most recent points deleted.</param>
    public virtual void OnClosedContour(float timeSinceLevelLoad)
    {

    }

    public virtual void OnRopeDurationChanged(float time)
    {

    }
}
