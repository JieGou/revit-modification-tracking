// ##########################################
// Solution: KTR
// Project: KTR.Main
// File: Tier3Object.cs
// 
// Last User: Chris Hildebran
// Last Edit: 2019-02-12 10:32 AM
// ##########################################
namespace KTR.Main.TreeViewAllElementInstances.M
{
	using System.Collections.ObjectModel;

	public class Tier3Object
	{
		#region Constructors

		public Tier3Object(string tier3ElementTypeName)
		{
			Tier3ElementTypeName     = tier3ElementTypeName;
			Tier4ElementInstanceItem = new ObservableCollection<Tier4Object>();
		}

		#endregion

		#region Properties

		public string Tier3ElementTypeName
		{
			get;
			set;
		}
		public ObservableCollection<Tier4Object> Tier4ElementInstanceItem
		{
			get;
			set;
		}

		#endregion
	}
}