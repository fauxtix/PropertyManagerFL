using PropertyManagerFL.UI.Resources;
using Syncfusion.Blazor;
using System.Resources;

namespace PropertyManagerFL.UI.Shared
{
    public class SyncfusionLocalizer : ISyncfusionStringLocalizer
    {
        public string GetText(string key)
        {
            return ResourceManager.GetString(key);
        }

        public ResourceManager ResourceManager
        {
            get
            {
                return SfResources.ResourceManager;
            }
        }
    }
}
