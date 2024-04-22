using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.WallMode
{
    public class _InitialScriptWall :MonoBehaviour 
    {
        public Sprite[] lstSprites;
        public Transform gridParent;
        public static Dictionary<int, int> newFrequency = new Dictionary<int, int>(BaseWall.FREQUENCY);
        void Start()
        {
            Initial();
        }
        public void Initial()
        {
            BaseWall.FREQUENCY = new Dictionary<int, int>(newFrequency);
            BaseWall BASEWall = new BaseWall();
            BaseWall.lstSprites = new Sprite[lstSprites.Length];
            for (int i = 0; i < lstSprites.Length; i++)
            {
                BaseWall.lstSprites[i] = lstSprites[i];
            }

            // Debug.Log(" BaseGravity.lstSprites: " + BaseGravity.lstSprites.ToString());
            BaseWall.gridParent = gridParent;
            BASEWall.GenerateMatrix(6, 12);

            var wallMode = new Wall(new List<int>() { 1, 3 }, new List<int>() { 4 });
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