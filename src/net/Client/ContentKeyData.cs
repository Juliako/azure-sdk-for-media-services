﻿//-----------------------------------------------------------------------
// <copyright file="ContentKeyData.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// Represents a content key that can be used for encryption and decryption.
    /// </summary>
    [DataServiceKey("Id")]
    internal partial class ContentKeyData : IContentKey, ICloudMediaContextInit
    {
        private CloudMediaContext _cloudMediaContext;

        /// <summary>
        /// Initializes the cloud media context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void InitCloudMediaContext(CloudMediaContext context)
        {
            this._cloudMediaContext = context;
        }

        /// <summary>
        /// Gets the clear key value.
        /// </summary>
        /// <returns>A function delegate that returns the future result to be available through the Task&lt;byte[]&gt;.</returns>
        public Task<byte[]> GetClearKeyValueAsync()
        {
            // Start a new task here because the ExecutAsync on the DataContext returns a Task<string>
            return System.Threading.Tasks.Task.Factory.StartNew<byte[]>(() =>
            {
                byte[] returnValue = null;
                if (this._cloudMediaContext != null)
                {
                    Uri uriRebindContentKey = new Uri(string.Format(CultureInfo.InvariantCulture, "/RebindContentKey?id='{0}'&x509Certificate=''", this.Id), UriKind.Relative);
                    DataServiceContext dataContext = this._cloudMediaContext.DataContextFactory.CreateDataServiceContext();

                    IEnumerable<string> results = dataContext.Execute<string>(uriRebindContentKey);
                    string reboundContentKey = results.Single();

                    returnValue = Convert.FromBase64String(reboundContentKey);
                }

                return returnValue;
            });
        }

        /// <summary>
        /// Gets the clear key value.
        /// </summary>
        /// <returns>The clear key value.</returns>
        public byte[] GetClearKeyValue()
        {
            try
            {
                Task<byte[]> task = this.GetClearKeyValueAsync();
                task.Wait();

                return task.Result;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }    
        }

        /// <summary>
        /// Gets the encrypted key value.
        /// </summary>
        /// <param name="certToEncryptTo">The cert to use.</param>
        /// <returns>A function delegate that returns the future result to be available through the Task&lt;byte[]&gt;.</returns>
        public Task<byte[]> GetEncryptedKeyValueAsync(X509Certificate2 certToEncryptTo)
        {
            if (certToEncryptTo == null)
            {
                throw new ArgumentNullException("certToEncryptTo");
            }

            // Start a new task here because the ExecutAsync on the DataContext returns a Task<string>
            return System.Threading.Tasks.Task.Factory.StartNew<byte[]>(() =>
                {
                    byte[] returnValue = null;

                    if (this._cloudMediaContext != null)
                    {
                        string certToSend = Convert.ToBase64String(certToEncryptTo.Export(X509ContentType.Cert));
                        certToSend = HttpUtility.UrlEncode(certToSend);

                        Uri uriRebindContentKey = new Uri(string.Format(CultureInfo.InvariantCulture, "/RebindContentKey?id='{0}'&x509Certificate='{1}'", this.Id, certToSend), UriKind.Relative);
                        DataServiceContext dataContext = this._cloudMediaContext.DataContextFactory.CreateDataServiceContext();

                        IEnumerable<string> results = dataContext.Execute<string>(uriRebindContentKey);
                        string reboundContentKey = results.Single();

                        returnValue = Convert.FromBase64String(reboundContentKey);
                    }

                    return returnValue;
                });
        }

        /// <summary>
        /// Gets the encrypted key value.
        /// </summary>
        /// <param name="certToEncryptTo">The cert to use.</param>
        /// <returns>The encrypted key value.</returns>
        public byte[] GetEncryptedKeyValue(X509Certificate2 certToEncryptTo)
        {
            try
            {
                Task<byte[]> task = this.GetEncryptedKeyValueAsync(certToEncryptTo);
                task.Wait();

                return task.Result;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns>A function delegate.</returns>
        public Task DeleteAsync()
        {
            ContentKeyBaseCollection.VerifyContentKey(this);

            DataServiceContext dataContext = this._cloudMediaContext.DataContextFactory.CreateDataServiceContext();
            dataContext.AttachTo(ContentKeyCollection.ContentKeySet, this);
            dataContext.DeleteObject(this);

            return dataContext.SaveChangesAsync(this);
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            try
            {
                this.DeleteAsync().Wait();
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        private static ContentKeyType GetExposedContentKeyType(int contentKeyType)
        {
            return (ContentKeyType)contentKeyType;
        }

        private static ProtectionKeyType GetExposedProtectionKeyType(int protectionKeyType)
        {
            return (ProtectionKeyType)protectionKeyType;
        }
    }
}
