// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using Analyzer.Utilities.PooledObjects;

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    internal static class FilePathInjectionSinks
    {
        /// <summary>
        /// <see cref="SinkInfo"/>s for tainted data file canonicalization sinks.
        /// </summary>
        public static ImmutableHashSet<SinkInfo> SinkInfos { get; }

        static FilePathInjectionSinks()
        {
            PooledHashSet<SinkInfo> builder = PooledHashSet<SinkInfo>.GetInstance();

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIODirectory,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "Exists", new[] { "path" } ),
                    ( "Delete", new[] { "path" } ),
                    ( "GetFiles", new[] { "path" } ),
                    ( "Move", new[] { "sourceDirName", "destDirName" } ),
                });
            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIOFile,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "AppendAllLines", new[] { "path" } ),
                    ( "AppendAllLinesAsync", new[] { "path" } ),
                    ( "AppendAllText", new[] { "path" } ),
                    ( "AppendAllTextAsync", new[] { "path" } ),
                    ( "AppendText", new[] { "path" } ),
                    ( "Copy", new[] { "sourceFileName", "destFileName" } ),
                    ( "Create", new[] { "path" } ),
                    ( "CreateText", new[] { "path" } ),
                    ( "Delete", new[] { "path" } ),
                    ( "Exists", new[] { "path" } ),
                    ( "Move", new[] { "sourceFileName", "destFileName" } ),
                    ( "Open", new[] { "path" } ),
                    ( "OpenRead", new[] { "path" } ),
                    ( "OpenText", new[] { "path" } ),
                    ( "OpenWrite", new[] { "path" } ),
                    ( "ReadAllBytes", new[] { "path" } ),
                    ( "ReadAllBytesAsync", new[] { "path" } ),
                    ( "ReadAllLines", new[] { "path" } ),
                    ( "ReadAllLinesAsync", new[] { "path" } ),
                    ( "ReadAllText", new[] { "path" } ),
                    ( "ReadAllTextAsync", new[] { "path" } ),
                    ( "ReadLines", new[] { "path" } ),
                    ( "WriteAllBytes", new[] { "path" } ),
                    ( "WriteAllBytesAsync", new[] { "path" } ),
                    ( "WriteAllLines", new[] { "path" } ),
                    ( "WriteAllLinesAsync", new[] { "path" } ),
                    ( "WriteAllText", new[] { "path" } ),
                    ( "WriteAllTextAsync", new[] { "path" } ),
                    ( "Replace", new[] { "sourceFileName", "destinationFileName", "destinationBackupFileName" } ),
                    ( "SetAccessControl", new[] { "path" } ),
                });
            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIOFileInfo,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: true,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "CopyTo", new[] { "destFileName" } ),
                    ( "MoveTo", new[] { "destFileName" } ),
                    ( "Replace", new[] { "destinationFileName", "destinationBackupFileName" } ),
                    ( ".ctor", new[] { "fileName" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemReflectionAssembly,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "LoadFile", new[] { "path" } ),
                    ( "LoadFrom", new[] { "assemblyFile" } ),
                    ( "ReflectionOnlyLoadFrom", new[] { "assemblyFile" } ),
                    ( "UnsafeLoadFrom", new[] { "assemblyFile" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemAppDomain,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "AppendPrivatePath", new[] { "path" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemXmlXmlReader,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "Create", new[] { "inputUri" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIOStreamReader,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( ".ctor", new[] { "path" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIOStreamWriter,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( ".ctor", new[] { "path" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIOFileStream,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( ".ctor", new[] { "path", "handle" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemWebMvcFilePathResult,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: new[] { "Filename" } ,
                sinkMethodParameters: new[] {
                    ( ".ctor", new[] { "fileName" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.MicrosoftAspNetCoreMvcPhysicalFileResult,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: new[] { "Filename" },
                sinkMethodParameters: new[] {
                    ( ".ctor", new[] { "fileName" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.MicrosoftAspNetCoreMvcRazorPagesPageModel,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "PhysicalFile", new[] { "physicalPath" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemWebUIWebControlsFileUpload,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "SaveAs", new[] { "filename" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemWebHttpResponse,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "TransmitFile", new[] { "filename" } ),
                    ( "WriteFile", new[] { "filename", "fileHandle" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemWebHttpResponseBase,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "TransmitFile", new[] { "filename" } ),
                    ( "WriteFile", new[] { "filename", "fileHandle" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemIOCompressionZipFileExtensions,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "CreateEntryFromFile", new[] { "sourceFileName" } ),
                    ( "ExtractToFile", new[] { "destinationFileName" } ),
                    ( "ExtractToDirectory", new[] { "destinationDirectoryName" } ),
                });

            builder.AddSinkInfo(
                WellKnownTypeNames.SystemNetWebClient,
                SinkKind.FilePathInjection,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    ( "DownloadFile", new[] { "fileName" } ),
                    ( "DownloadFileAsync", new[] { "fileName" } ),
                    ( "DownloadFileTaskAsync", new[] { "fileName" } ),
                });

            SinkInfos = builder.ToImmutableAndFree();
        }
    }
}
