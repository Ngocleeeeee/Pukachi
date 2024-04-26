using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.WallMode
{
    public class _InitialScriptWall :MonoBehaviour 
    {
        public Sprite[] lstSprites; // cố định
        public Transform gridParent;
        public static Dictionary<int, int> newFrequency = new Dictionary<int, int>(BaseWall.FREQUENCY);
        void Start()
        {
            BaseWall.FREQUENCY = new Dictionary<int, int>(newFrequency);
            BaseWall BASE = new BaseWall();
            BaseWall.lstSprites = new Sprite[lstSprites.Length];
            for (int i = 0; i < lstSprites.Length; i++)
            {
                BaseWall.lstSprites[i] = lstSprites[i];

            }
            // Debug.Log(" Base1.lstSprites: " + Base1.lstSprites.ToString());
            BaseWall.gridParent = gridParent;
            BaseWall.SetHelp(5, 2);
            BASE.GenerateMatrix(6, 12);

            var wallMode = new Wall(new int[] { 1, 2, 3, 4, 5, 6 }, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
            wallMode.GenerateWall();

        }
        public int getSize()
        {
            return 6 * 12;
        }
        // Update is called once per frame
        void Update()
        {

        }


    }
}