using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionUtilities;

public class BallTypeManager : MonoBehaviour
{
    public static BallTypeManager instance {get; private set; }

    [SerializeField] private List<Color> colors;

    private void Awake() {
        if(instance != null && instance != this)
        {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    public Color GetColor(int type)
    {
        return colors[type];
    }

    public int GetRandomType()
    {
        Debug.Log(colors.Count);
        return Random.Range(0,colors.Count);
    }

}
