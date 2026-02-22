using UnityEngine;
using System.Collections.Generic;
using Systems.Inventory.Affixes;

namespace Systems.Inventory.Managers
{
    // Компонент для управления аффиксами на оружии
    public class WeaponAffixManager : MonoBehaviour
    {
        private GunInfo gunInfo;
        private List<IConditionalAffix> activeConditionalAffixes = new List<IConditionalAffix>();
        
        void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            var baseWeapon = GetComponent<global::BaseWeapon>();
            if(baseWeapon != null && baseWeapon.gunInfo != null)
            {
                gunInfo = baseWeapon.gunInfo;
                SetupAffixes();
            }
        }
        
        private void SetupAffixes()
        {
            if(gunInfo == null) return;
            
            // Подписываем условные аффиксы на события
            foreach(var affixInstance in gunInfo.affixes)
            {
                if(affixInstance.isEquipped && affixInstance.affix != null)
                {
                    var conditionalAffix = affixInstance.affix as IConditionalAffix;
                    if(conditionalAffix != null)
                    {
                        conditionalAffix.SubscribeToEvents();
                        activeConditionalAffixes.Add(conditionalAffix);
                    }
                }
            }
        }
        
        public void UpdateAffixes()
        {
            if(gunInfo == null) return;
            
            // Обновляем список активных аффиксов
            CleanupInactiveAffixes();
            SetupAffixes();
        }
        
        private void CleanupInactiveAffixes()
        {
            // Отписываемся от событий у неприсоединенных аффиксов
            for(int i = activeConditionalAffixes.Count - 1; i >= 0; i--)
            {
                bool stillActive = false;
                
                foreach(var affixInstance in gunInfo.affixes)
                {
                    if(affixInstance.isEquipped && affixInstance.affix is IConditionalAffix conditionalAffix && 
                       (IConditionalAffix)affixInstance.affix == activeConditionalAffixes[i])
                    {
                        stillActive = true;
                        break;
                    }
                }
                
                if(!stillActive)
                {
                    activeConditionalAffixes[i].UnsubscribeFromEvents();
                    activeConditionalAffixes.RemoveAt(i);
                }
            }
        }
        
        void OnDestroy()
        {
            // Отписываемся от событий при уничтожении
            foreach(var affix in activeConditionalAffixes)
            {
                affix.UnsubscribeFromEvents();
            }
            activeConditionalAffixes.Clear();
        }
    }
}