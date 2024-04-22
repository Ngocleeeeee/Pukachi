using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Classic
{
    public class _InitialScriptClassic : MonoBehaviour
    {
        public Sprite[] lstSprites;
        public Transform gridParent;

        public static Dictionary<int, int> newFrequency = new Dictionary<int, int>(BaseClassic.FREQUENCY);
        void Start()
        {
            Initial();
        }
        public void Initial()
        {
            BaseClassic.FREQUENCY = new Dictionary<int, int>(newFrequency);

            BaseClassic BASEClassic = new BaseClassic();
            BaseClassic.lstSprites = new Sprite[lstSprites.Length];
            for (int i = 0; i < lstSprites.Length; i++)
            {
                BaseClassic.lstSprites[i] = lstSprites[i];
            }

            // Debug.Log(" BaseGravity.lstSprites: " + BaseGravity.lstSprites.ToString());
            BaseClassic.gridParent = gridParent;
            BASEClassic.GenerateMatrix(6, 12);

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
