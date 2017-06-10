using System;
using System.Reflection;

namespace MetroOverhaulModPatch.RedirectionFramework.Attributes.IgnoreConditions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
    public class DisableAttribute : IgnoreConditionAttribute
    {
        public override bool IsIgnored(MemberInfo methodInfo)
        {
            return true;
        }
    }
}