namespace BaseMod
{
    public class CustomMapManager : CreatureMapManager
    {
        public virtual bool IsMapChangable()
        {
            return true;
        }
        public virtual void CustomInit()
        {
        }
    }
}
