using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Setting")]
    public class Setting
    {
        public long Id { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }
}
