using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    [Range(1, 4)]
    public int playerNumber = 1;
    public enum SpawnFacing { right = 1, left = -1 }
    public SpawnFacing facingDir = SpawnFacing.right;

}
