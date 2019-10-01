using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkThroughObstacle : MonoBehaviour
{
    private int normalLayer = 11;
    private int walkThroughLayer = 12; //TEMP LAYERS
    private void OnEnable()
    {
        CharacterControllerScript.CanWalkThroughWalls += SwitchCollisionLayer;
    }
    private void OnDisable()
    {
        CharacterControllerScript.CanWalkThroughWalls -= SwitchCollisionLayer;
    }

    private void SwitchCollisionLayer(bool canGoThrough)
    {
        if (canGoThrough)
            gameObject.layer = walkThroughLayer;
        else
            gameObject.layer = normalLayer;
    }
}
