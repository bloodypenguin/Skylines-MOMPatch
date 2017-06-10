using System;
using ICities;
using MetroOverhaulModPatch.RedirectionFramework;

namespace MOMPatch
{
    public class MetroOverhaulModPatch : IUserMod
    {

        public bool initalized = false;

        public string Name
        {
            get
            {
                try
                {
                    if (!initalized)
                    {
                        AssemblyRedirector.Deploy();
                        initalized = true;
                    }
                    return "Metro Overhaul Patch 1.6.2";
                }
                catch
                {
                    return "Metro Overhaul Patch 1.6.2 [Not working :(]";
                }

            }
        }

        public string Description => "Patches some bugs in Metro Overhaul Mod";
    }
}
