using EmpowerID.Common.Extentions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Runtime.Serialization;

namespace EmpowerID.Common.Enums
{
    public enum DataStatus
    {
        [EnumMember(Value = "Dirty")]
        Dirty,
        [EnumMember(Value = "Inserted")]
        Inserted,
        [EnumMember(Value = "Updated")]
        Updated,
        [EnumMember(Value = "Deleted")]
        Deleted
    }
    public class DataStatusValueConverter : ValueConverter<DataStatus, string>
    {
        public DataStatusValueConverter() : base(v => Convert(v), v => Convert(v)) { }

        private static string Convert(DataStatus value)
        {
            var retVal = value.ToString();
            return retVal;
        }

        private static DataStatus Convert(string value)
        {
            var retVal = value.ParseEnum<DataStatus>();
            return retVal;
        }
    }
}
