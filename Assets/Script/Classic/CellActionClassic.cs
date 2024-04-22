namespace Assets.Script.Classic
{
    public class CellActionClassic : CellAction
    {
        // Start is called before the first frame update
        void Start()
        {
            StartProcess();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateProcess();
            CheckFinalScore(score);
        }

    }
}