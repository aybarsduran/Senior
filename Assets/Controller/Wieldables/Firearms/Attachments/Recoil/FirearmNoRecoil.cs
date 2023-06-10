using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Recoil/No Recoil")]
    public class FirearmNoRecoil : FirearmRecoilBehaviour
    {
        public override void DoRecoil(bool isAiming, float heatValue, float triggerValue)
        {
        }
    }
}
