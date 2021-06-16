using Autofac;
using CMG.License.UI.Views;

namespace CMG.License.UI
{
    public class UiModuleRegistery : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<UiModule>();
            builder.RegisterType<OpenLogFileView>();
        }
    }
}