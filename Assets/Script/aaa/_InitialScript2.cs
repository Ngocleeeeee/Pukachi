using Assets.Script.WallMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _InitialScript2 : MonoBehaviour
{
    public Sprite[] lstSprites; // cố định
    public Transform gridParent;
    void Start()
    {
        Base1 BASE = new Base1();
        Base1.lstSprites = new Sprite[lstSprites.Length];
        for (int i = 0; i < lstSprites.Length; i++)
        {
            Base1.lstSprites[i] = lstSprites[i];

        }

       // Debug.Log(" Base1.lstSprites: " + Base1.lstSprites.ToString());
        Base1.gridParent = gridParent;
        BASE.GenerateMatrix(6, 12);
        
        /*var wallMode = new Wall1(new List<int>() {1, 3}, new List<int>() { 4});
        wallMode.GenerateWall();*/
        
        
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
