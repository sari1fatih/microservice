using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Core.CrossCuttingConcerns.Serilog.CustomLog;

public class CustomColumnWriter : ColumnWriterBase
{
    private NpgsqlDbType _ngsqlDbType { get; set; }
    public string _key { get; set; }

    public CustomColumnWriter(string key,NpgsqlDbType ngsqlDbType,int length=0) : base(ngsqlDbType, length)
    {
        _ngsqlDbType = ngsqlDbType;
        _key = key;
    }

    public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
    {
        
        if (logEvent.Properties.TryGetValue(_key, out var httpMethodValue) &&
            httpMethodValue is ScalarValue scalarValue &&
            scalarValue.Value is string body &&
            !string.IsNullOrEmpty(body))
        {
            if (_ngsqlDbType == NpgsqlDbType.Integer)
            {
                return int.Parse(body);
            }
            else
            {
                return body;
            }
   
        }

        return null;
    }
}