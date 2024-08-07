﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public static class AttributeDataExtensions
    {
        public static T DecodeNamedArgument<T>(this AttributeData data, string name, SpecialType specialType, T defaultValue = default)
        {
            var namedArguments = data.NamedArguments;
            int index = IndexOfNamedArgument(namedArguments, name);
            return index >= 0 ? namedArguments[index].Value.DecodeValue<T>(specialType) : defaultValue;
        }

        private static int IndexOfNamedArgument(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, string name)
        {
            // For user defined attributes VB allows duplicate named arguments and uses the last value.
            // Dev11 reports an error for pseudo-custom attributes when emitting metadata. We don't.
            for (int i = namedArguments.Length - 1; i >= 0; i--)
            {
                // even for VB this is case sensitive comparison:
                if (string.Equals(namedArguments[i].Key, name, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public static class TypedConstantExtensions
    {
        public static T DecodeValue<T>(this TypedConstant c, SpecialType specialType)
        {
            c.TryDecodeValue(specialType, out T value);
            return value;
        }

        public static  bool TryDecodeValue<T>(this TypedConstant c, SpecialType specialType, out T value)
        {
            if (c.Kind == TypedConstantKind.Error)
            {
                value = default;
                return false;
            }

            if (c.Type!.SpecialType == specialType || (c.Type.TypeKind == TypeKind.Enum && specialType == SpecialType.System_Enum))
            {
                value = (T)c.Value!;
                return true;
            }

            // the actual argument type doesn't match the type of the parameter - an error has already been reported by the binder
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Used for logging compiler diagnostics to a stream in the standardized SARIF
    /// (Static Analysis Results Interchange Format) v2.1.0 format.
    /// http://docs.oasis-open.org/sarif/sarif/v2.1.0/sarif-v2.1.0.html
    /// </summary>
    internal sealed class SarifV2ErrorLogger : SarifErrorLogger, IDisposable
    {
        private readonly DiagnosticDescriptorSet _descriptors;

        private readonly string _toolName;
        private readonly string _toolFileVersion;
        private readonly Version _toolAssemblyVersion;

        public SarifV2ErrorLogger(Stream stream, string toolName, string toolFileVersion, Version toolAssemblyVersion, CultureInfo culture)
            : base(stream, culture)
        {
            _descriptors = new DiagnosticDescriptorSet();

            _toolName = toolName;
            _toolFileVersion = toolFileVersion;
            _toolAssemblyVersion = toolAssemblyVersion;

            _writer.WriteObjectStart(); // root
            _writer.Write("$schema", "https://schemastore.azurewebsites.net/schemas/json/sarif-2.1.0-rtm.5.json");
            _writer.Write("version", "2.1.0");
            _writer.WriteArrayStart("runs");
            _writer.WriteObjectStart(); // run

            _writer.WriteArrayStart("results");
        }

        protected override string PrimaryLocationPropertyName => "physicalLocation";

        public override void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo)
        {
            _writer.WriteObjectStart(); // result
            _writer.Write("ruleId", diagnostic.Id);
            int ruleIndex = _descriptors.Add(diagnostic.Descriptor);
            _writer.Write("ruleIndex", ruleIndex);

            _writer.Write("level", GetLevel(diagnostic.Severity));

            string? message = diagnostic.GetMessage(_culture);
            if (!String.IsNullOrEmpty(message))
            {
                _writer.WriteObjectStart("message");
                _writer.Write("text", message);
                _writer.WriteObjectEnd();
            }

            if (diagnostic.IsSuppressed)
            {
                _writer.WriteArrayStart("suppressions");
                _writer.WriteObjectStart(); // suppression
                _writer.Write("kind", "inSource");
                string? justification = suppressionInfo?.Attribute?.DecodeNamedArgument<string>("Justification", SpecialType.System_String);
                if (justification != null)
                {
                    _writer.Write("justification", justification);
                }

                _writer.WriteObjectEnd(); // suppression
                _writer.WriteArrayEnd();
            }

            WriteLocations(diagnostic.Location, diagnostic.AdditionalLocations);

            WriteResultProperties(diagnostic);

            _writer.WriteObjectEnd(); // result
        }

        private void WriteLocations(Location location, IReadOnlyList<Location> additionalLocations)
        {
            if (HasPath(location))
            {
                _writer.WriteArrayStart("locations");
                _writer.WriteObjectStart(); // location
                _writer.WriteKey(PrimaryLocationPropertyName);

                WritePhysicalLocation(location);

                _writer.WriteObjectEnd(); // location
                _writer.WriteArrayEnd(); // locations
            }

            // See https://github.com/dotnet/roslyn/issues/11228 for discussion around
            // whether this is the correct treatment of Diagnostic.AdditionalLocations
            // as SARIF relatedLocations.
            if (additionalLocations != null &&
                additionalLocations.Count > 0 &&
                additionalLocations.Any(l => HasPath(l)))
            {
                _writer.WriteArrayStart("relatedLocations");

                for (int i = 0; i < additionalLocations.Count; i++)
                {
                    Location? additionalLocation = additionalLocations[i];
                    if (HasPath(additionalLocation))
                    {
                        _writer.WriteObjectStart(); // annotatedCodeLocation
                        _writer.Write("id", i);

                        int sinkId = additionalLocations.Count - 1;
                        if (i == 0 || i == sinkId) // source location
                        {
                            _writer.WriteObjectStart("message");
                            if (i == 0) // source
                            {
                                _writer.Write("text", "Source: Origin of tainted data");
                            }
                            else // sink
                            {
                                _writer.Write("text", "Sink: Destination where tainted data can be expoited");
                            }
                            
                            _writer.WriteObjectEnd();
                        }

                        

                        _writer.WriteKey("physicalLocation");

                        WritePhysicalLocation(additionalLocation);

                        _writer.WriteObjectEnd(); // annotatedCodeLocation
                    }
                }

                _writer.WriteArrayEnd(); // relatedLocations
            }
        }

        protected override void WritePhysicalLocation(Location diagnosticLocation)
        {
            Debug.Assert(HasPath(diagnosticLocation));

            FileLinePositionSpan span = diagnosticLocation.GetLineSpan();

            _writer.WriteObjectStart(); // physicalLocation

            _writer.WriteObjectStart("artifactLocation");
            _writer.Write("uri", GetUri(span.Path));
            _writer.WriteObjectEnd(); // artifactLocation

            WriteRegion(span);

            _writer.WriteObjectEnd();
        }

        public override void Dispose()
        {
            _writer.WriteArrayEnd(); //results

            WriteTool();

            _writer.Write("columnKind", "utf16CodeUnits");

            _writer.WriteObjectEnd(); // run
            _writer.WriteArrayEnd();  // runs
            _writer.WriteObjectEnd(); // root
            base.Dispose();
        }

        private void WriteTool()
        {
            _writer.WriteObjectStart("tool");
            _writer.WriteObjectStart("driver");
            _writer.Write("name", _toolName);
            _writer.Write("version", _toolFileVersion);
            _writer.Write("dottedQuadFileVersion", _toolAssemblyVersion.ToString());
            _writer.Write("semanticVersion", _toolAssemblyVersion.ToString(fieldCount: 3));
            _writer.Write("language", "en");

            WriteRules();

            _writer.WriteObjectEnd(); // driver
            _writer.WriteObjectEnd(); // tool
        }

        private void WriteRules()
        {
            if (_descriptors.Count > 0)
            {
                _writer.WriteArrayStart("rules");

                foreach (var pair in _descriptors.ToSortedList())
                {
                    DiagnosticDescriptor descriptor = pair.Value;

                    _writer.WriteObjectStart(); // rule
                    _writer.Write("id", descriptor.Id);

                    string? shortDescription = descriptor.Title.ToString(_culture);
                    if (!String.IsNullOrEmpty(shortDescription))
                    {
                        _writer.WriteObjectStart("shortDescription");
                        _writer.Write("text", shortDescription);
                        _writer.WriteObjectEnd();
                    }

                    string? fullDescription = descriptor.Description.ToString(_culture);
                    if (!String.IsNullOrEmpty(fullDescription))
                    {
                        _writer.WriteObjectStart("fullDescription");
                        _writer.Write("text", fullDescription);
                        _writer.WriteObjectEnd();
                    }

                    WriteDefaultConfiguration(descriptor);

                    if (!string.IsNullOrEmpty(descriptor.HelpLinkUri))
                    {
                        _writer.Write("helpUri", descriptor.HelpLinkUri);
                    }

                    if (!string.IsNullOrEmpty(descriptor.Category) || descriptor.CustomTags.Any())
                    {
                        _writer.WriteObjectStart("properties");

                        if (!string.IsNullOrEmpty(descriptor.Category))
                        {
                            _writer.Write("category", descriptor.Category);
                        }

                        if (descriptor.CustomTags.Any())
                        {
                            _writer.WriteArrayStart("tags");

                            foreach (string tag in descriptor.CustomTags)
                            {
                                _writer.Write(tag);
                            }

                            _writer.WriteArrayEnd(); // tags
                        }

                        _writer.WriteObjectEnd(); // properties
                    }

                    _writer.WriteObjectEnd(); // rule
                }

                _writer.WriteArrayEnd(); // rules
            }
        }

        private void WriteDefaultConfiguration(DiagnosticDescriptor descriptor)
        {
            string defaultLevel = GetLevel(descriptor.DefaultSeverity);

            // Don't bother to emit default values.
            bool emitLevel = defaultLevel != "warning";

            // The default value for "enabled" is "true".
            bool emitEnabled = !descriptor.IsEnabledByDefault;

            if (emitLevel || emitEnabled)
            {
                _writer.WriteObjectStart("defaultConfiguration");

                if (emitLevel)
                {
                    _writer.Write("level", defaultLevel);
                }

                if (emitEnabled)
                {
                    _writer.Write("enabled", descriptor.IsEnabledByDefault);
                }

                _writer.WriteObjectEnd(); // defaultConfiguration
            }
        }

        /// <summary>
        /// Represents a distinct set of <see cref="DiagnosticDescriptor"/>s and provides unique integer indices
        /// to distinguish them.
        /// </summary>
        private sealed class DiagnosticDescriptorSet
        {
            // DiagnosticDescriptor -> integer index
            private readonly Dictionary<DiagnosticDescriptor, int> _distinctDescriptors = new Dictionary<DiagnosticDescriptor, int>(SarifDiagnosticComparer.Instance);

            /// <summary>
            /// The total number of descriptors in the set.
            /// </summary>
            public int Count => _distinctDescriptors.Count;

            /// <summary>
            /// Adds a descriptor to the set if not already present.
            /// </summary>
            /// <returns>
            /// The unique key assigned to the given descriptor.
            /// </returns>
            public int Add(DiagnosticDescriptor descriptor)
            {
                if (_distinctDescriptors.TryGetValue(descriptor, out int index))
                {
                    // Descriptor has already been seen.
                    return index;
                }
                else
                {
                    _distinctDescriptors.Add(descriptor, Count);
                    return Count - 1;
                }
            }

            /// <summary>
            /// Converts the set to a list, sorted by index.
            /// </summary>
            public List<KeyValuePair<int, DiagnosticDescriptor>> ToSortedList()
            {
                Debug.Assert(Count > 0);

                var list = new List<KeyValuePair<int, DiagnosticDescriptor>>(Count);

                foreach (var pair in _distinctDescriptors)
                {
                    Debug.Assert(list.Capacity > list.Count);
                    list.Add(new KeyValuePair<int, DiagnosticDescriptor>(pair.Value, pair.Key));
                }

                Debug.Assert(list.Capacity == list.Count);
                list.Sort((x, y) => x.Key.CompareTo(y.Key));
                return list;
            }
        }
    }
}
