namespace SelectByParameters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.AutoCAD.Windows;

    public static class ContextMenu
    {
        public static void Start()
        {
            var cm = new ContextMenuExtension();
            var menu = new MenuItem(Properties.Resources.ContextMenu);
            menu.Click += Click;
            cm.MenuItems.Add(menu);
            Application.AddObjectContextMenuExtension(RXObject.GetClass(typeof(Entity)), cm);
        }

        private static void Click(object sender, EventArgs e)
        {
            var doc = AcadHelper.Doc;
            var sel = doc.Editor.SelectImplied();
            if (sel.Status != PromptStatus.OK) return;
            using (var t = doc.TransactionManager.StartTransaction())
            {
                var parameters = sel.Value.GetObjectIds().GetObjects<Entity>().GroupBy(g => g.GetType())
                    .OrderBy(o => o.Key.Name).Select(GetTypeParams).Where(w => w != null);
                t.Commit();
            }
        }

        private static Group GetTypeParams(IGrouping<Type, Entity> entities)
        {
            throw new NotImplementedException();
        }
    }

    public class Group
    {
        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
