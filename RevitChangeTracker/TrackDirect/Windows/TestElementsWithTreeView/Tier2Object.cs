// ##########################################
// Solution: KTR
// Project: KTR.Main
// File: Tier2Object.cs
// 
// Last User: Chris Hildebran
// Last Edit: 2019-02-12 10:32 AM
// ##########################################
namespace KTR.Main.TreeViewAllElementInstances.M
{
	using System.Collections.ObjectModel;

	public class Tier2Object
	{
		#region Constructors

		public Tier2Object(string tier2FamilyName)
		{
			Tier2FamilyName = tier2FamilyName;
			Tier3TypeNames  = new ObservableCollection<Tier3Object>();
		}

		#endregion

		#region Properties

		public string Tier2FamilyName
		{
			get;
			set;
		}

		public ObservableCollection<Tier3Object> Tier3TypeNames
		{
			get;
			set;
		}

		#endregion
	}
}