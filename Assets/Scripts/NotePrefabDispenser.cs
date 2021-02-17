using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class NotePrefabDispenser : PrefabDispenser
{
    Dictionary<int, SampleRecord<VisualNote>> _spawnedNotes = new Dictionary<int, SampleRecord<VisualNote>>();

    protected override GameObject SpawnIfNecessary()
    {
        GameObject go = base.SpawnIfNecessary();
        if(go != null)
        {
            int instanceId = go.GetInstanceID();
            VisualNote visualNote = go.GetComponent<VisualNote>();
            _spawnedNotes.Add(instanceId, new SampleRecord<VisualNote>(visualNote));
            visualNote.AboutToBeDestroyed.AddListener(() => _spawnedNotes.Remove(instanceId));
        }

        return go;
    }

    public void FadeAllNotes()
    {
        _spawnedNotes.Values.Select(record => record.Value).ForEach(visualNote => visualNote.Fade());
    }

    public void FadeAllNotesOlderThan(float time)
    {
        _spawnedNotes
            .Values
            .Where(record => record.Time <= time)
            .Select(record => record.Value)
            .ForEach(visualNote => visualNote.Fade());
    }
}
