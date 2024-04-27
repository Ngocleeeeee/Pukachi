using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script.Gravity
{
    public class _InitialScriptGravity : MonoBehaviour
    {
        public Sprite[] lstSprites;
        public Transform gridParent;
        public static Dictionary<int, int> newFrequency = new Dictionary<int, int>(BaseGravity.FREQUENCY);
        void Start()
        {
            Initial();
        }
        public void Initial()
        {
            BaseGravity.FREQUENCY = new Dictionary<int, int>(newFrequency);
            BaseGravity BASEGravity = new BaseGravity();
            BaseGravity.lstSprites = new Sprite[lstSprites.Length];
            for (int i = 0; i < lstSprites.Length; i++)
            {
                BaseGravity.lstSprites[i] = lstSprites[i];
            }

            // Debug.Log(" BaseGravity.lstSprites: " + BaseGravity.lstSprites.ToString());
            BaseGravity.gridParent = gridParent;
            BASEGravity.GenerateMatrix(6, 12);

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