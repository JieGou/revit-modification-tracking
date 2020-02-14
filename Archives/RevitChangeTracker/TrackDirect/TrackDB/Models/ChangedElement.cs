using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrackDirect.Models
{
    public class ChangedElement

    {
        public enum ChangeTypeEnum
        {
            GeometryChange,
            VolumeOrLocationChange,
            NewElement,
            DeletedElement,
            SharedParameterChange,
            RevitParameterChange,
            FamilyOrTypeChange
        }

        #region Properties
        public int ElementId { get; set; }

        public string UniqueId { get; set; }

        public string Category { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChangeTypeEnum ChangeType { get; set; }

        public String Level { get; set; } = String.Empty;

        public string BoundingBoxDescription { get; set; }

        public string ChangeDescription { get; set; } = String.Empty;

        public string ElementDescription { get; set; }

        public string CreatedDate { get; set; }

        public string ModifiedDate { get; set; }

        public string User { get; set; }


               #endregion

        #region PublicMethods
        public override string ToString()
        {
            return Category + ": " + ChangeType;
        }
        #endregion


    }
}
