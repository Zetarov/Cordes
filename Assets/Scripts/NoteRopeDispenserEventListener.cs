using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRopeDispenserEventListener : RopeDispenserEventListener
{
    private float _appearTime;
    private VisualNote _visualNote = null;
    private RopeDispenser _ropeDispenser;

    private void Awake()
    {
        _visualNote = GetComponent<VisualNote>();
    }

    private void Start()
    {
        _appearTime = Time.timeSinceLevelLoad;
    }

    public override void OnActivatedChanged(bool on)
    {
        base.OnActivatedChanged(on);
        if(!on)
        {
            _visualNote.Fade();
        }
    }

    public override void OnClosedContour(float timeSinceLevelLoad)
    {
        base.OnClosedContour(timeSinceLevelLoad);
        if(timeSinceLevelLoad > _appearTime)
        {
            _visualNote.Fade();
        }
    }

    public void SetRopeDispenser(RopeDispenser ropeDispenser)
    {
        _ropeDispenser = ropeDispenser;
        if(_ropeDispenser != null)
        {
            _ropeDispenser.AddListener(this);
            _visualNote.Duration = ropeDispenser.Duration;
        }
    }

    private void OnDestroy()
    {
        _ropeDispenser?.RemoveListener(this);
    }
}
