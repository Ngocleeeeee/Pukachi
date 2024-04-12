using Assets.Script.WallMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _InitialScript : MonoBehaviour
{
    public Sprite[] lstSprites; // cố định
    public Transform gridParent;
    void Start()
    {
        Base BASE = new Base();
        Base.lstSprites = new Sprite[lstSprites.Length];
        for (int i = 0; i < lstSprites.Length; i++)
        {
            Base.lstSprites[i] = lstSprites[i];
        }

       // Debug.Log(" Base.lstSprites: " + Base.lstSprites.ToString());
        Base.gridParent = gridParent;
        BASE.GenerateMatrix(4, 8);
        
        var wallMode = new Wall(new List<int>() {1, 3}, new List<int>() { 4});
        wallMode.GenerateWall();
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
