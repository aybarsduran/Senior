using System;
using UnityEngine;

namespace IdenticalStudios
{
    using Random = UnityEngine.Random;
    
    public static class PhysicsUtils
    {
        private static readonly RaycastHit[] s_RaycastHits = new RaycastHit[32];
        private static readonly Collider[] s_OverlappedColliders = new Collider[32];
        
        
        public static Ray GenerateRay(Transform transform, float randomSpread, Vector3 localOffset = default)
        {
            Vector3 raySpreadVector = transform.TransformVector(new Vector3(Random.Range(-randomSpread, randomSpread), Random.Range(-randomSpread, randomSpread), 0f));
            Vector3 rayDirection = Quaternion.Euler(raySpreadVector) * transform.forward;
            
            return new Ray(transform.position + transform.TransformVector(localOffset), rayDirection);
        }
        
        public static bool RaycastNonAlloc(Ray ray, float distance, out RaycastHit hitInfo,
            int layers = Physics.DefaultRaycastLayers, Collider[] ignoredColliders = null,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var size = Physics.RaycastNonAlloc(ray, s_RaycastHits, distance, layers, triggerInteraction);
            
            int closestHit = -1;
            float closestDistance = Mathf.Infinity;
        
            for (int i = 0; i < size; i++)
            {
                if (ignoredColliders != null && ContainsCollider(ignoredColliders, s_RaycastHits[i].collider))
                    continue;

                if (!(s_RaycastHits[i].distance < closestDistance))
                    continue;
				
                closestDistance = s_RaycastHits[i].distance;
                closestHit = i;
            }

            if (closestHit != -1)
            {
                hitInfo = s_RaycastHits[closestHit];
                return true;
            }
			
            hitInfo = default;
            return false;
        }

        public static bool SphereCastNonAlloc(Ray ray, float radius, float distance, out RaycastHit hitInfo,
            int layers = Physics.DefaultRaycastLayers, Collider[] ignoredColliders = null,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var size = Physics.SphereCastNonAlloc(ray, radius, s_RaycastHits, distance, layers, triggerInteraction);
            
            int closestHit = -1;
            float closestDistance = Mathf.Infinity;
        
            for (int i = 0; i < size; i++)
            {
                if (ignoredColliders != null && ContainsCollider(ignoredColliders, s_RaycastHits[i].collider))
                    continue;

                if (!(s_RaycastHits[i].distance < closestDistance))
                    continue;
				
                closestDistance = s_RaycastHits[i].distance;
                closestHit = i;
            }

            if (closestHit != -1)
            {
                hitInfo = s_RaycastHits[closestHit];
                return true;
            }
			
            hitInfo = default;
            return false;
        }

        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 extents, Quaternion orientation, out Collider[] colliders, LayerMask layers, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int size = Physics.OverlapBoxNonAlloc(center, extents, s_OverlappedColliders, orientation, layers, triggerInteraction);

            if (size == 0)
            {
                colliders = Array.Empty<Collider>();
                return size;
            }

            colliders = s_OverlappedColliders;
            
            return size;
        }
        
        public static int OverlapSphereNonAlloc(Vector3 position, float radius, out Collider[] colliders, LayerMask layers, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            int size = Physics.OverlapSphereNonAlloc(position, radius, s_OverlappedColliders, layers, triggerInteraction);

            if (size == 0)
            {
                colliders = Array.Empty<Collider>();
                return size;
            }

            colliders = s_OverlappedColliders;
            
            return size;
        }

        private static bool ContainsCollider(Collider[] ignoredColliders, Collider collider)
        {
            foreach (var ignoredCollider in ignoredColliders)
            {
                if (ignoredCollider == collider)
                    return true;
            }

            return false;
        }
    }
}
