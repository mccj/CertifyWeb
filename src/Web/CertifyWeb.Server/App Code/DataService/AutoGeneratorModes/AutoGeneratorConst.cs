namespace AutoGeneratorModes
{
    /// <summary>
    /// 相关常量
    /// </summary>
    public class AutoGeneratorConst
    {
        public const string EntityId = "Entity";
        public const string EntitySuffix = "Record";
        public const string DtoId = "Dto";
        public const string DtoSuffix = "Dto";
        public const string CreateId = "CreateInput";
        public const string CreateSuffix = "CreateInput";
        public const string CreateSummaryPrefix = "创建 ";
        public const string UpdateId = "UpdateInput";
        public const string UpdateSuffix = "UpdateInput";
        public const string UpdateSummaryPrefix = "更新 ";
        public const string DetailId = "DetailOutput";
        public const string DetailSuffix = "DetailOutput";
        public const string DetailSummaryPrefix = "详情";
        public const string SearchId = "SearchInput";
        public const string SearchSuffix = "SearchInput";
        public const string InfluxDBId = "InfluxDB";
        public const string InfluxDBSuffix = "InfluxDB";
        public const string SearchSummaryPrefix = "搜索";
        public const string SqlSugarColumnSplitField = "SqlSugar.SplitField";
        public const string SqlSugarColumnPrimaryKey = "SqlSugar.SugarColumn(IsPrimaryKey = true)";
        public const string SqlSugarColumnStringEnum = "SqlSugar.SugarColumn(ColumnDataType = InternalControlsSys.Application.Const.AutoGeneratorConst.SugarColumnEnumDataType /*, SqlParameterDbType=typeof(SqlSugar.DbConvert.EnumToStringConvert)*/)";
        public const string SqlSugarColumnString64 = "SqlSugar.SugarColumn(Length = 64)";
        public const string SqlSugarColumnString128 = "SqlSugar.SugarColumn(Length = 128)";
        public const string SqlSugarColumnString1024 = "SqlSugar.SugarColumn(Length = 1024)";
        public const string SqlSugarColumnStringMax = $"SqlSugar.SugarColumn(Length = 0x7fffffff)";
        public const string SqlSugarColumnStringBig = "SqlSugar.SugarColumn(ColumnDataType = SqlSugar.StaticConfig.CodeFirst_BigString)";
        /// <summary> 枚举类型 </summary>
        public const string SugarColumnEnumDataType = "varchar(255)";
        public const string OpenApiStringEnum = "";//"System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter)), Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))";
        public const string StringSyntaxJson = "System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Json)";
        public const string StringSyntaxXml = "System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Xml)";
        public const string StringSyntaxUri = "System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Uri)";
        public const string InfluxDBColumn = $"InfluxDB.Client.Core.Column";
        public const string InfluxDBColumnTimestamp = $"InfluxDB.Client.Core.Column(IsTimestamp = true)";
        public const string InfluxDBColumnTag = $"InfluxDB.Client.Core.Column(IsTag = true)";
        public const string InfluxDBMeasurement = $"InfluxDB.Client.Core.Measurement";
    }

    //public class SearchDateTimeRange
    //{
    //    public DateTime? StartTime { get; set; }
    //    public DateTime? EndTime { get; set; }
    //}

    /// <summary>
    /// 搜索查询区间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchRangeForStructValue<T> where T : struct
    {
        /// <summary>
        /// 搜索开始值
        /// </summary>
        public T? StartValue { get; set; }
        /// <summary>
        /// 搜索结束值
        /// </summary>
        public T? EndValue { get; set; }
    }
    public class SearchModeBase
    {
        public SearchModeEnum? SearchMode { get; set; }
    }
    public enum SearchModeEnum { And, Or }
}