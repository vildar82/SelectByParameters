using Autodesk.AutoCAD.Runtime;
[assembly: ExtensionApplication(typeof(SelectByParameters.App))]

namespace SelectByParameters
{
    public class App : IExtensionApplication
    {
        public void Initialize()
        {
            ContextMenu.Start();
        }

        public void Terminate()
        {
            throw new System.NotImplementedException();
        }
    }
}
