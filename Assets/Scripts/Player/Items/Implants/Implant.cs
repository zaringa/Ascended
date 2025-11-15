namespace Player.Items.Implants
{
    public abstract class Implant : Item, IImplantable
    {
        public enum Rareness { Common, Rare }

        public abstract void Action();
    }
}
