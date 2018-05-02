using CMG.License.Shared.PrismHelpers;
using CMG.License.UI.Views;
using Prism.Modularity;
using Prism.Regions;

namespace CMG.License.UI
{
    public class UiModule : IModule
    {
        private readonly IRegionManager regionManager;

        public UiModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            regionManager.RegisterViewWithRegion(PrismRegions.MainRegion.Name, typeof(OpenLogFileView));
        }
    }
}