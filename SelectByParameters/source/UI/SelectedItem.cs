namespace SelectByParameters.UI
{
    using Autodesk.AutoCAD.DatabaseServices;

    public class SelectedItem
    {
        public string Name { get; set; }
        public ObjectId Id { get; set; }
    }
}