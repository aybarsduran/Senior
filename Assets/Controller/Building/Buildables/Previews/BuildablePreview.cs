using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.BuildingSystem
{
    public abstract class BuildablePreview : MonoBehaviour, ISaveableComponent
    {
        public static List<BuildablePreview> AllPreviewsInScene = new();
        public Vector3 PreviewCenter { get; private set; }

        public event UnityAction<bool> MaterialAdded;

        protected Dictionary<Buildable, BuildRequirement[]> buildableRequirements;
        protected static Collider[] s_CollisionResults = new Collider[10];


        /// <summary>
        /// Converts all of the build requirements from all of the attached buildables to one list.
        /// Make sure to cache this list and not call this every frame as it can be quite expensive.
        /// </summary>
        /// <returns></returns>
        public List<BuildRequirement> GetAllBuildRequirements()
        {
            if (buildableRequirements == null)
                return null;

            var buildRequirements = new List<BuildRequirement>();

            foreach (var buildableReq in buildableRequirements.Values)
            {
                foreach (var req in buildableReq)
                {
                    var newReq = GetRequirementWithId(buildRequirements, req.BuildMaterial);

                    if (newReq != null)
                    {
                        newReq.RequiredAmount += req.RequiredAmount;
                        newReq.CurrentAmount += req.CurrentAmount;
                    }
                    else
                    {
                        newReq = new BuildRequirement(req.BuildMaterial, req.RequiredAmount, req.CurrentAmount);
                        buildRequirements.Add(newReq);
                    }
                }
            }

            return buildRequirements;
        }

        public void EnablePreview()
        {
            // Enables and registers this preview as active.
            if (!AllPreviewsInScene.Contains(this))
                AllPreviewsInScene.Add(this);
        }

        public void DisablePreview()
        {
            // Disables and unregisters this preview from the active pool.
            AllPreviewsInScene.Remove(this);
        }

        public abstract void CancelPreview();

        public bool TryAddBuildingMaterial(DataIdReference<BuildMaterialDefinition> buildMat)
        {
            if (buildMat.IsNull)
                return false;

            if (CanAddBuildMaterial())
            {
                var buildDef = buildMat.Def;
                foreach (var buildableReq in buildableRequirements.Values)
                {
                    var buildReq = GetRequirementWithId(buildableReq, buildMat.Id);

                    if (buildReq != null && !buildReq.IsCompleted())
                    {
                        buildReq.CurrentAmount++;
                        OnMaterialAdded(buildDef);

                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual bool CanAddBuildMaterial() => true;

        public void AddBuildable(Buildable buildable)
        {
            if (buildableRequirements == null)
                buildableRequirements = new Dictionary<Buildable, BuildRequirement[]>();

            buildableRequirements.Add(buildable, buildable.Definition.GetBuildRequirements());

            CalculateCenter();

            MaterialAdded?.Invoke(false);
        }

        public void RemoveBuildable(Buildable buildable)
        {
            if (buildableRequirements == null)
                return;

            if (buildableRequirements.Remove(buildable))
                CalculateCenter();
        }

        protected virtual void OnMaterialAdded(BuildMaterialDefinition buildMat)
        {
            buildMat.UseSound.PlayAtPosition(transform.position);
            bool buildComplete = true;

            // Itterate through all of the build requirements and check if all of them are completed.
            foreach (var buildableReq in buildableRequirements)
            {
                if (AreRequirementsMet(buildableReq.Value))
                {
                    OnBuildableBuilt(buildableReq.Key);
                    CalculateCenter();
                }
                else
                    buildComplete = false;
            }

            if (buildComplete)
                OnAllBuildablesBuilt();

            MaterialAdded?.Invoke(buildComplete);
        }

        protected abstract void OnBuildableBuilt(Buildable buildable);
        protected abstract void OnAllBuildablesBuilt();
        protected abstract Vector3 GetPreviewCenter();

        protected bool AreRequirementsMet(BuildRequirement[] buildReq)
        {
            bool buildComplete = true;

            for (int i = 0; i < buildReq.Length; i++)
            {
                if (!buildReq[i].IsCompleted())
                    buildComplete = false;
            }

            return buildComplete;
        }

        protected void CalculateCenter() => PreviewCenter = GetPreviewCenter();
        protected virtual void OnDestroy() => DisablePreview();

        private BuildRequirement GetRequirementWithId(IEnumerable<BuildRequirement> list, int materialId)
        {
            foreach (var req in list)
            {
                if (req.BuildMaterial == materialId)
                    return req;
            }

            return null;
        }

        #region Save & Load
        public virtual void LoadMembers(object[] members) { }
        public virtual object[] SaveMembers() => System.Array.Empty<object>();
        #endregion
    }
}