using Autodesk.AutoCAD.Runtime;
using SelectByParameters.Lib.IO;

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
        }
    }
}
