using Player.Items.Implants.Interfaces;

namespace Player.Items.Implants.Base
{
    public abstract class Implant : Item, IImplantable
    {
        public enum Rareness { Common, Rare }

        public abstract void Action();
    }
}
