namespace Editor
{
    [System.Serializable]
    public class MaterialUnit
    {
        public string MaterialName; 
        public int MaterialID;

        public MaterialUnit (string materialName, int materialID)
        {
            MaterialName = materialName;
            MaterialID = materialID;
        }
    }
}
