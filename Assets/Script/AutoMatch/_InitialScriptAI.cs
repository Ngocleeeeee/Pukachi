using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.AutoMatch
{
    public class _InitialScriptAI : MonoBehaviour
    {
        public Sprite[] lstSprites;
        public Transform gridParent;
        public static Dictionary<int, int> newFrequency = new Dictionary<int, int>(BaseAI.FREQUENCY);
        void Start()
        {
            Initial();
        }
        public void Initial()
        {
            BaseAI.FREQUENCY = new Dictionary<int, int>(newFrequency);
            BaseAI BASEAI = new BaseAI();
            BaseAI.lstSprites = new Sprite[lstSprites.Length];
            for (int i = 0; i < lstSprites.Length; i++)
            {
                BaseAI.lstSprites[i] = lstSprites[i];
            }

            // Debug.Log(" BaseGravity.lstSprites: " + BaseGravity.lstSprites.ToString());
            BaseAI.gridParent = gridParent;
            BASEAI.GenerateMatrix(6, 12);

        }

        // Update is called once per frame
        void Update()
        {

        }

        public int getSize()
        {
            return 6 * 12;
        }
    }
}