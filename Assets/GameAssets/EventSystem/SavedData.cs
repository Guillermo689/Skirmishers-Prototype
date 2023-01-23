using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SavedData", menuName = "Save Data")]
public class SavedData : ScriptableObject
{
    public float masterSlider;
    public float musicSlider;
    public float effectsSlider;

    public bool spanish;
}
