using System;
using ICities;
using MetroOverhaul;
using MetroOverhaul.Extensions;
using MetroOverhaulModPatch.RedirectionFramework.Attributes;
using UnityEngine;

namespace MOMPatch.Detours
{
    [TargetType(typeof(AssetsUpdater))]
    public class AssetsUpdaterDetour : AssetsUpdater
    {
        [RedirectMethod]
        public new void UpdateExistingAssets(LoadMode mode)
        {
            UpdateMetroTrainEffects();
            if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
                return;
            }
            try
            {
                UpdateTrainTracks();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                UpdateMetroStationsMeta();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        [RedirectMethod]
        private static void UpdateMetroStationsMeta()
        {
            var vanillaMetroStation = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");

            var infos = Resources.FindObjectsOfTypeAll<BuildingInfo>();
            if (infos == null)
            {
                return;
            }
            foreach (var info in infos)
            {
                try
                {
                    if (info == null || info.m_buildingAI == null || !info.IsMetroDepot())
                    {
                        continue;
                    }

                    var ai = info.m_buildingAI as TransportStationAI;
                    if (ai != null)
                    {
                        var transportStationAi = ai;
                        transportStationAi.m_maxVehicleCount = 0;
                    }
                    info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                    ((DepotAI) info.m_buildingAI).m_createPassMilestone = ((DepotAI) vanillaMetroStation.m_buildingAI)
                        .m_createPassMilestone;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"MOM: Failed to update meta of {info?.name}:");
                    UnityEngine.Debug.LogException(e);
                }
            }
        }



        [RedirectMethod]
        private static void SetupTunnelTracks(BuildingInfo info, bool toVanilla = false)
        {
            if (info?.m_paths == null)
            {
                return;
            }
            foreach (var path in info.m_paths)
            {
                if (path?.m_netInfo?.name == null)
                {
                    continue;
                }
                if (toVanilla)
                {
                    if (path.m_netInfo.name.Contains("Metro Station Track Tunnel"))
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
                    }
                    else if (path.m_netInfo.name.Contains("Metro Track Tunnel"))
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                    }
                }
                else
                {
                    if (path.m_netInfo.name == "Metro Station Track")
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                    }
                    else if (path.m_netInfo.name == "Metro Track")
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track Tunnel");
                    }
                }
            }
        }

        [RedirectReverse]
        private static void UpdateTrainTracks()
        {
            UnityEngine.Debug.LogError("UpdateTrainTracks");
        }

        [RedirectReverse]
        private static void UpdateMetroTrainEffects()
        {
            UnityEngine.Debug.LogError("UpdateMetroTrainEffects");
        }
    }
}