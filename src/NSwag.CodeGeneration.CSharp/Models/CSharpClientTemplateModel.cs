//-----------------------------------------------------------------------
// <copyright file="CSharpClientTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp client template model.</summary>
    public class CSharpClientTemplateModel : CSharpTemplateModelBase
    {
        private readonly SwaggerDocument _document;
        private readonly JsonSchema4 _exceptionSchema;
        private readonly SwaggerToCSharpClientGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="CSharpClientTemplateModel" /> class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">The class name of the controller.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        public CSharpClientTemplateModel(
            string controllerName,
            string controllerClassName,
            IEnumerable<CSharpOperationModel> operations,
            JsonSchema4 exceptionSchema,
            SwaggerDocument document,
            SwaggerToCSharpClientGeneratorSettings settings)
            : base(controllerName, settings)
        {
            _document = document;
            _exceptionSchema = exceptionSchema;
            _settings = settings;

            Class = controllerClassName;
            Operations = operations;

            ExceptionClass = _settings.ExceptionClass.Replace("{controller}", controllerName);
        }

        /// <summary>Gets or sets a value indicating whether to generate client contracts (i.e. client interfaces).</summary>
        public bool GenerateContracts { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate implementation classes.</summary>
        public bool GenerateImplementation { get; set; }

        /// <summary>Gets the class name.</summary>
        public string Class { get; }

        /// <summary>Gets the base class name.</summary>
        public string BaseClass => _settings.ClientBaseClass;

        /// <summary>Gets a value indicating whether the client has a base class.</summary>
        public bool HasBaseClass => !string.IsNullOrEmpty(_settings.ClientBaseClass);

        /// <summary>Gets a value indicating whether the client has configuration class.</summary>
        public bool HasConfigurationClass => !string.IsNullOrEmpty(_settings.ConfigurationClass);

        /// <summary>Gets the configuration class name.</summary>
        public string ConfigurationClass => _settings.ConfigurationClass;

        /// <summary>Gets a value indicating whether the client has a base type.</summary>
        public bool HasBaseType => _settings.GenerateClientInterfaces || HasBaseClass;

        /// <summary>Gets or sets a value indicating whether an HttpClient instance is injected.</summary>
        public bool InjectHttpClient => _settings.InjectHttpClient;

        /// <summary>Gets a value indicating whether to use a HTTP client creation method.</summary>
        public bool UseHttpClientCreationMethod => _settings.UseHttpClientCreationMethod;

        /// <summary>Gets a value indicating whether to use a HTTP request message creation method.</summary>
        public bool UseHttpRequestMessageCreationMethod => _settings.UseHttpRequestMessageCreationMethod;

        /// <summary>Gets a value indicating whether to generate client interfaces.</summary>
        public bool GenerateClientInterfaces => _settings.GenerateClientInterfaces;

        /// <summary>Gets the service base URL.</summary>
        public string BaseUrl => _document.BaseUrl;

        /// <summary>Gets a value indicating whether the client has operations.</summary>
        public bool HasOperations => Operations.Any();

        /// <summary>Gets the exception class name.</summary>
        public string ExceptionClass { get; }

        /// <summary>Gets a value indicating whether to generate optional parameters.</summary>
        public bool GenerateOptionalParameters => _settings.GenerateOptionalParameters;

        /// <summary>Gets or sets a value indicating whether to use and expose the base URL (default: true).</summary>
        public bool UseBaseUrl => _settings.UseBaseUrl;

        /// <summary>Gets or sets a value indicating whether to generate synchronous methods (not recommended, default: false).</summary>
        public bool GenerateSyncMethods => _settings.GenerateSyncMethods;

        /// <summary>Gets or sets the client class access modifier.</summary>
        public string ClientClassAccessModifier => _settings.ClientClassAccessModifier;

        /// <summary>Gets the operations.</summary>
        public IEnumerable<CSharpOperationModel> Operations { get; }

        /// <summary>Gets the JSON serializer parameter code.</summary>
        public string JsonSerializerParameterCode
        {
            get
            {
                var handleReferences = _settings.CSharpGeneratorSettings.HandleReferences;

                IEnumerable<string> jsonConverters = _settings.CSharpGeneratorSettings.JsonConverters ?? new string[] { };
                if (RequiresJsonExceptionConverter)
                    jsonConverters = jsonConverters.Concat(new[] { "JsonExceptionConverter" });

                return CSharpJsonSerializerGenerator.GenerateJsonSerializerParameterCode(handleReferences, jsonConverters.ToList());
            }
        }

        private bool RequiresJsonExceptionConverter =>
            _document.Operations.Any(o => o.Operation.AllResponses.Any(r => r.Value.InheritsExceptionSchema(_exceptionSchema)));
    }
}