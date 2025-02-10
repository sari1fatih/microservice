using Core.CrossCuttingConcerns.Serilog.ConfigurationModels;
using Core.CrossCuttingConcerns.Serilog.CustomLog;
using Core.CrossCuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Core.CrossCuttingConcerns.Serilog.Loggers;

public class PostgreSqlLogger : LoggerServiceBase
{
    public  PostgreSqlLogger(IConfiguration configuration)
    {
        PostgreSqlConfiguration logConfiguration =
            configuration.GetSection("SeriLogConfigurations:PostgreSqlConfiguration").Get<PostgreSqlConfiguration>()
            ?? throw new Exception(SerilogMessages.NullOptionsMessage);

        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            { "http_method", new CustomColumnWriter("http_method",NpgsqlDbType.Varchar,10) }, 
            { "path", new CustomColumnWriter("path",NpgsqlDbType.Varchar,50) },
            { "query_params", new CustomColumnWriter("query_params",NpgsqlDbType.Varchar,100) },
            { "body", new CustomColumnWriter( "body",NpgsqlDbType.Jsonb,0) }, 
            { "user_id", new CustomColumnWriter("user_id",NpgsqlDbType.Integer,0) }
            
        };

        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.PostgreSQL(logConfiguration.ConnectionString, logConfiguration.TableName, columnWriters,needAutoCreateTable: logConfiguration.AutoCreateSqlTable)
            .CreateLogger();
        Logger = logger;
    }
}