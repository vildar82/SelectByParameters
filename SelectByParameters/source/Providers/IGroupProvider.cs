namespace SelectByParameters.Providers
{
    using System;
    using System.Collections.Generic;
    using Autodesk.AutoCAD.DatabaseServices;
    using UI;

    public interface IGroupProvider
    {
        string Name { get; set; }

        void Filter(Dictionary<Type, List<Entity>> allItems, Action<Entity> blockEntity);
        void OnClosed();
    }
}
