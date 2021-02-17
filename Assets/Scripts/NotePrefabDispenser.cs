using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePrefabDispenser : PrefabDispenser
{
    [SerializeField]
    RopeDispenser _ropeDispenser;

    protected override GameObject SpawnIfNecessary()
    {
        GameObject go = base.SpawnIfNecessary();
        if(go != null)
            go.GetComponent<NoteRopeDispenserEventListener>().SetRopeDispenser(_ropeDispenser);

        return go;
    }
}
