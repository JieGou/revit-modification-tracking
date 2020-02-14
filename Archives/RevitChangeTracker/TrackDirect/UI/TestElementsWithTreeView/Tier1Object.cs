// ##########################################
// Solution: KTR
// Project: KTR.Main
// File: Tier1Object.cs
// 
// Last User: Chris Hildebran
// Last Edit: 2019-02-12 10:32 AM
// ##########################################
namespace KTR.Main.TreeViewAllElementInstances.M
{
	using System.Collections.ObjectModel;

	public class Tier1Object
	{
		#region Constructors

		public Tier1Object(string tier1CategoryName)
		{
			Tier1CategoryName = tier1CategoryName;
			Tier2FamilyNames  = new ObservableCollection<Tier2Object>();
		}

		#endregion

		#region Properties

		public string Tier1CategoryName
		{
			get;
			set;
		}

		public ObservableCollection<Tier2Object> Tier2FamilyNames
		{
			get;
			set;
		}

		#endregion
	}
}