using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using MetroOverhaul;
using MetroOverhaul.OptionsFramework;
using MetroOverhaul.UI;
using MetroOverhaulModPatch.RedirectionFramework.Attributes;
using UnityEngine;

namespace MOMPatch.Detours
{
    [TargetType(typeof(MetroOverhaul.LoadingExtension))]
    public class LoadingExtensionDetour //do not inherit ILoadingExtenison or it will be treated as such!
    {
        private LoadMode _cachedMode;

        [RedirectMethod]
        public void OnLevelLoaded(LoadMode mode)
        {
            _cachedMode = mode;
            while (LateBuildUpQueue.Count > 0)
            {
                try
                {
                    LateBuildUpQueue.Dequeue().Invoke();
                }
                catch (Exception e)
                {
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Enable asset in Content Manager!", e.Message, false);
                }
            }
            if (_updater == null)
            {
                _updater = new AssetsUpdater();
                _updater.UpdateExistingAssets(mode);
            }
            AssetsUpdater.UpdateBuildingsMetroPaths(mode, false);
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
            {
                SimulationManager.instance.AddAction(DespawnVanillaMetro);
                var gameObject = new GameObject("MetroOverhaulUISetup");
                gameObject.AddComponent<UpgradeSetup>();
                gameObject.AddComponent<StyleSelectionUI>();

                if (OptionsWrapper<Options>.Options.metroUi)
                {
                    UIView.GetAView().AddUIComponent(typeof(MetroStationCustomizerUI));
                }

                var transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Metro");
                transportInfo.m_netLayer = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                transportInfo.m_stationLayer = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            }
        }

        [RedirectMethod]
        public void OnLevelUnloading()
        {
            //it appears, the game caches vanilla prefabs even when exiting to main menu, and stations won't load properly on reloading from main menu
            AssetsUpdater.UpdateBuildingsMetroPaths(_cachedMode, true);
            var go = GameObject.Find("MetroOverhaulUISetup");
            if (go != null)
            {
                GameObject.Destroy(go);
            }
            var transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Metro");
            transportInfo.m_netLayer = ItemClass.Layer.MetroTunnels;
            transportInfo.m_stationLayer = ItemClass.Layer.MetroTunnels;
        }


        private static AssetsUpdater _updater
        {
            get => (AssetsUpdater) typeof(LoadingExtension)
                .GetField("_updater", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);

            set => typeof(LoadingExtension)
                .GetField("_updater", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, value);
        }

        private static Queue<System.Action> LateBuildUpQueue => (Queue<System.Action>)typeof(LoadingExtension)
            .GetField("LateBuildUpQueue", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null);

        [RedirectReverse]
        private static void DespawnVanillaMetro()
        {
            UnityEngine.Debug.Log("DespawnVanillaMetro");
        }
    }
}