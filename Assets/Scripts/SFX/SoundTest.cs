using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        MusicManager.GetMusic().volume = 0.3f;
        MusicManager.GetMusic().PlaySound("PaisibleMusic");
    }

    // Update is called once per frame
    void Update() {
        
    }
}
