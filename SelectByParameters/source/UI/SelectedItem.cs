namespace SelectByParameters.UI
{
	using Autodesk.AutoCAD.DatabaseServices;

	[Equals]
	public class SelectedItem
	{
		[IgnoreDuringEquals]
		public string Name { get; set; }

		public ObjectId Id { get; set; }
	}
}
