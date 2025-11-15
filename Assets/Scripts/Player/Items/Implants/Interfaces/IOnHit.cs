using Enemy;

namespace Player.Items.Implants.Interfaces
{
    public interface IOnHit
    {
        public void OnHit(IEnemy target, ref float damage);
    }
}
