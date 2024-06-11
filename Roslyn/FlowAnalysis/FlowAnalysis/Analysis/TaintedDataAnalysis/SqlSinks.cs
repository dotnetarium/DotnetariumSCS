// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using Analyzer.Utilities.PooledObjects;

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    internal static class SqlSinks
    {
        /// <summary>
        /// <see cref="SinkInfo"/>s for tainted data SQL sinks.
        /// </summary>
        public static ImmutableHashSet<SinkInfo> SinkInfos { get; }

        static SqlSinks()
        {
            var sinkInfosBuilder = PooledHashSet<SinkInfo>.GetInstance();

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataIDbCommand,
                SinkKind.Sql,
                isInterface: true,
                isAnyStringParameterInConstructorASink: true,
                sinkProperties: new string[] {
                    "CommandText",
                },
                sinkMethodParameters: null);

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataIDataAdapter,
                SinkKind.Sql,
                isInterface: true,
                isAnyStringParameterInConstructorASink: true,
                sinkProperties: null,
                sinkMethodParameters: null);

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemWebUIWebControlsSqlDataSource,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: new string[] {
                    "ConnectionString",
                    "DeleteCommand",
                    "InsertCommand",
                    "SelectCommand",
                    "UpdateCommand",
                },
                sinkMethodParameters: new[]
                {
                    ( ".ctor", new[] { "selectCommand", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.MicrosoftEntityFrameworkCoreRelationalQueryableExtensions,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "FromSql", new[] { "sql", } ),
                    ( "FromSqlRaw", new[] { "sql" } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataEntityDbSet1,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "SqlQuery", new[] { "sql", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataEntityDbSet,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "SqlQuery", new[] { "sql", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataLinqDataContext,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "ExecuteQuery", new[] { "query", } ),
                    ( "ExecuteCommand", new[] { "command", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.MySqlDataMySqlClientMySqlHelper,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "ExecuteDataRow", new[] { "commandText", } ),
                    ( "ExecuteDataRowAsync", new[] { "commandText", } ),
                    ( "ExecuteDataset", new[] { "commandText", } ),
                    ( "ExecuteDatasetAsync", new[] { "commandText", } ),
                    ( "ExecuteNonQuery", new[] { "commandText", } ),
                    ( "ExecuteNonQueryAsync", new[] { "commandText", } ),
                    ( "ExecuteReader", new[] { "commandText", } ),
                    ( "ExecuteReaderAsync", new[] { "commandText", } ),
                    ( "ExecuteScalar", new[] { "commandText", } ),
                    ( "ExecuteScalarAsync", new[] { "commandText", } ),
                    ( "UpdateDataSet", new[] { "commandText", } ),
                    ( "UpdateDataSetAsync", new[] { "commandText", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataSQLiteSQLiteCommand,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "Execute", new[] { "commandText", } ),      
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.SystemDataEntityDatabase,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "SqlQuery", new[] { "sql", } ),
                    ( "ExecuteSqlCommand", new[] { "sql", } ),
                    ( "ExecuteSqlCommandAsync", new[] { "sql", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.MicrosoftEntityFrameworkCoreRelationalDatabaseFacadeExtensions,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "ExecuteSqlCommand", new[] { "sql", } ),
                    ( "ExecuteSqlCommandAsync", new[] { "sql", } ),
                    ( "ExecuteSqlRaw", new[] { "sql", } ),
                    ( "ExecuteSqlRawAsync", new[] { "sql", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.MicrosoftPracticesEnterpriseLibraryDataDatabase,
                SinkKind.Sql,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "GetSqlStringCommand", new[] { "query", } ),
                    ( "ExecuteDataSet", new[] { "commandText", "storedProcedureName" } ),
                    ( "ExecuteReader", new[] { "commandText", "storedProcedureName" } ),
                    ( "ExecuteNonQuery", new[] { "commandText", "storedProcedureName" } ),
                    ( "ExecuteScalar", new[] { "commandText", "storedProcedureName" } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.NHibernateISession,
                SinkKind.Sql,
                isInterface: true,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "CreateSQLQuery", new[] { "queryString", } ),
                });

            sinkInfosBuilder.AddSinkInfo(
                WellKnownTypeNames.CassandraISession,
                SinkKind.Sql,
                isInterface: true,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "Execute", new[] { "cqlQuery", } ),
                });
            
            SinkInfos = sinkInfosBuilder.ToImmutableAndFree();
        }
    }
}
