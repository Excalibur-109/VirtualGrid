using UnityEngine;

namespace Excalibur
{
    public static class ExcalbiurFactory
    {
        public static T MonoBehaviourProducer<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            return Object.Instantiate(prefab, parent);
        }
    }   
}