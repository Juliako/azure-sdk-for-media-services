﻿//-----------------------------------------------------------------------
// <copyright file="CloudMediaContext.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
// <license>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </license>

using System;
using Microsoft.WindowsAzure.MediaServices.Client.OAuth;
using Microsoft.WindowsAzure.MediaServices.Client.Versioning;

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// Describes the context from which all entities in the Microsoft WindowsAzure Media Services platform can be accessed.
    /// </summary>
    public class CloudMediaContext : MediaContextBase
    {
        /// <summary>
        /// The certificate thumbprint for Nimbus services.
        /// </summary>
        internal const string NimbusRestApiCertificateThumbprint = "AC24B49ADEF9D6AA17195E041D3F8D07C88EC145";

        /// <summary>
        /// The certificate subject for Nimbus services.
        /// </summary>
        internal const string NimbusRestApiCertificateSubject = "CN=NimbusRestApi";

        private const string MediaServicesAccessScope = "urn:WindowsAzureMediaServices";
        private static readonly Uri _mediaServicesUri = new Uri("https://media.windows.net/");
        private static readonly Uri _mediaServicesAcsBaseAddress = new Uri("https://wamsprodglobal001acs.accesscontrol.windows.net");

        private readonly AzureMediaServicesDataServiceContextFactory _dataContextFactory;
        private readonly AssetCollection _assets;
        private readonly AssetFileCollection _files;
        private readonly AccessPolicyBaseCollection _accessPolicies;
        private readonly ContentKeyCollection _contentKeys;
        private readonly JobBaseCollection _jobs;
        private readonly JobTemplateBaseCollection _jobTemplates;
        private readonly NotificationEndPointCollection _notificationEndPoints;
        private readonly MediaProcessorBaseCollection _mediaProcessors;
        private readonly LocatorBaseCollection _locators;
        private readonly IngestManifestCollection _ingestManifests;
        private readonly IngestManifestAssetCollection _ingestManifestAssets;
        private readonly IngestManifestFileCollection _ingestManifestFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="accountName">The Microsoft WindowsAzure Media Services account name to authenticate with.</param>
        /// <param name="accountKey">The Microsoft WindowsAzure Media Services account key to authenticate with.</param>
        public CloudMediaContext(string accountName, string accountKey)
            : this(CloudMediaContext._mediaServicesUri, accountName, accountKey, CloudMediaContext.MediaServicesAccessScope, CloudMediaContext._mediaServicesAcsBaseAddress.AbsoluteUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="apiServer">A <see cref="Uri"/> representing a the API endpoint.</param>
        /// <param name="accountName">The Microsoft WindowsAzure Media Services account name to authenticate with.</param>
        /// <param name="accountKey">The Microsoft WindowsAzure Media Services account key to authenticate with.</param>
        public CloudMediaContext(Uri apiServer, string accountName, string accountKey)
            : this(apiServer, accountName, accountKey, scope: CloudMediaContext.MediaServicesAccessScope, acsBaseAddress: CloudMediaContext._mediaServicesAcsBaseAddress.AbsoluteUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="apiServer">A <see cref="Uri"/> representing a the API endpoint.</param>
        /// <param name="accountName">The Microsoft WindowsAzure Media Services account name to authenticate with.</param>
        /// <param name="accountKey">The Microsoft WindowsAzure Media Services account key to authenticate with.</param>
        /// <param name="scope">The scope of authorization.</param>
        /// <param name="acsBaseAddress">The access control endpoint to authenticate against.</param>
        public CloudMediaContext(Uri apiServer, string accountName, string accountKey, string scope, string acsBaseAddress)
        {
            this.ParallelTransferThreadCount = 10;
            this.NumberOfConcurrentTransfers = 2;

            OAuthDataServiceAdapter dataServiceAdapter =
                new OAuthDataServiceAdapter(accountName, accountKey, scope, acsBaseAddress, NimbusRestApiCertificateThumbprint, NimbusRestApiCertificateSubject);
            ServiceVersionAdapter versionAdapter = new ServiceVersionAdapter(KnownApiVersions.Current);

            this._dataContextFactory = new AzureMediaServicesDataServiceContextFactory(apiServer, dataServiceAdapter, versionAdapter, this);

            this._jobs = new JobBaseCollection(this);
            this._jobTemplates = new JobTemplateBaseCollection(this);
            this._assets = new AssetCollection(this);
            this._files = new AssetFileCollection(this);
            this._accessPolicies = new AccessPolicyBaseCollection(this);
            this._contentKeys = new ContentKeyCollection(this);
            this._notificationEndPoints = new NotificationEndPointCollection(this);
            this._mediaProcessors = new MediaProcessorBaseCollection(this);
            this._locators = new LocatorBaseCollection(this);
            this._ingestManifests = new IngestManifestCollection(this);
            this._ingestManifestAssets = new IngestManifestAssetCollection(this,null);
            this._ingestManifestFiles = new IngestManifestFileCollection(this, null);
        }

        /// <summary>
        /// Gets or sets the number of threads to use to for each blob transfer.
        /// </summary>
        /// <remarks>The default value is 10.</remarks>
        public int ParallelTransferThreadCount { get; set; }

        /// <summary>
        /// Gets or sets the number of concurrent blob transfers allowed.
        /// </summary>
        /// <remarks>The default value is 2.</remarks>
        public int NumberOfConcurrentTransfers { get; set; }

        /// <summary>
        /// Gets the collection of assets in the system.
        /// </summary>
        public override AssetBaseCollection Assets
        {
            get { return this._assets; }
        }

        /// <summary>
        /// Gets the collection of files in the system.
        /// </summary>
        public override AssetFileBaseCollection Files
        {
            get { return this._files; }
        }

        /// <summary>
        /// Gets the collection of access policies in the system.
        /// </summary>
        public override AccessPolicyBaseCollection AccessPolicies
        {
            get { return this._accessPolicies; }
        }

        /// <summary>
        /// Gets the collection of content keys in the system.
        /// </summary>
        public override ContentKeyBaseCollection ContentKeys
        {
            get { return this._contentKeys; }
        }

        /// <summary>
        /// Gets the collection of jobs available in the system.
        /// </summary>
        public override JobBaseCollection Jobs
        {
            get { return this._jobs; }
        }

        /// <summary>
        /// Gets the collection of job templates available in the system.
        /// </summary>
        public override JobTemplateBaseCollection JobTemplates
        {
            get { return this._jobTemplates; }
        }

        /// <summary>
        /// Gets the collection of media processors available in the system.
        /// </summary>
        public override MediaProcessorBaseCollection MediaProcessors
        {
            get { return this._mediaProcessors; }
        }

        /// <summary>
        /// Gets the collection of notification endpoints avaiable in the system.
        /// </summary>
        public NotificationEndPointCollection NotificationEndPoints
        {
            get { return this._notificationEndPoints; }
        }

        /// <summary>
        /// Gets the collection of locators in the system.
        /// </summary>
        public LocatorBaseCollection Locators
        {
            get { return this._locators; }
        }

        /// <summary>
        /// Gets a factory for creating data service context instances prepared for Windows Azure Media Services.
        /// </summary>
        public AzureMediaServicesDataServiceContextFactory DataContextFactory
        {
            get { return this._dataContextFactory; }
        }

        /// <summary>
        /// Gets the collection of bulk ingest manifests in the system.
        /// </summary>
        public IngestManifestCollection IngestManifests
        {
            get { return this._ingestManifests; }
        }

        /// <summary>
        /// Gets the collection of manifest asset files in the system
        /// </summary>
        public  IngestManifestFileCollection IngestManifestFiles
        {
            get { return this._ingestManifestFiles; }
        }

        /// <summary>
        /// Gets the collection of manifest assets in the system
        /// </summary>
        public IngestManifestAssetCollection IngestManifestAssets
        {
            get { return this._ingestManifestAssets; }
        }
    }
}
