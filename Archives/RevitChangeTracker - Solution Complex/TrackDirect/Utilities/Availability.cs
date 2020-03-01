using Autodesk.Revit.UI;

namespace TrackDirect.Utilities
{
    /// <summary>
    /// This addin will be always enabled.
    /// </summary>
    public class Availability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, Autodesk.Revit.DB.CategorySet selectedCategories)
        {
            return true;
        }
    }
}
