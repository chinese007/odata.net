﻿//---------------------------------------------------------------------
// <copyright file="SingletonQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.SingletonTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingletonQueryTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        public SingletonQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        #region Query singleton entity

        [TestMethod]
        public void QuerySingleton()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataEntry entry = this.QueryEntry("VipCustomer", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, entry.Properties.Single(p => p.Name == "PersonID").Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWhichIsOpenType()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataEntry entry = this.QueryEntry("Company", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(0, entry.Properties.Single(p => p.Name == "CompanyID").Value);
                }
            }
        }

        [TestMethod]
        public void QueryDerivedSingletonWithTypeCast()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataEntry entry = this.QueryEntry("Boss/Microsoft.Test.OData.Services.ODataWCFService.Customer", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Sydney", entry.Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [TestMethod]
        public void QueryDerivedSingletonWithoutTypeCast()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataEntry entry = this.QueryEntry("Boss", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Sydney", entry.Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWithExpand()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryEntries("VipCustomer?$expand=Orders", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var orders = entries.FindAll(e => e.Id.AbsoluteUri.Contains("Orders"));
                    Assert.AreEqual(2, orders.Count);

                    var customer = entries.SingleOrDefault(e => e.Id.AbsoluteUri.Contains("VipCustomer"));
                    Assert.IsNotNull(customer);
                    Assert.AreEqual(1, customer.Properties.Single(p => p.Name == "PersonID").Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWithSelect()
        {
            foreach (var mimeType in mimeTypes)
            {
                var customer = this.QueryEntry("VipCustomer?$select=PersonID,FirstName", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.IsNotNull(customer);
                    Assert.AreEqual(2, customer.Properties.Count());
                }
            }
        }

        [TestMethod]
        public void QueryDerivedSingletonWithTypeCastAndSelect()
        {
            foreach (var mimeType in mimeTypes)
            {
                var customer = this.QueryEntry("Boss/Microsoft.Test.OData.Services.ODataWCFService.Customer?$select=City", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, customer.Properties.Count());
                    Assert.AreEqual("Sydney", customer.Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWithSelectUnderExpand()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryEntries("VipCustomer?$expand=Orders($select=OrderID,OrderDate)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var orders = entries.FindAll(e => e.Id.AbsoluteUri.Contains("Orders"));
                    Assert.AreEqual(2, orders.Count);

                    foreach (var order in orders)
                    {
                        Assert.AreEqual(2, order.Properties.Count());
                    }

                    var customer = entries.SingleOrDefault(e => e.Id.AbsoluteUri.Contains("VipCustomer"));
                    Assert.IsNotNull(customer);
                    Assert.AreEqual(1, customer.Properties.Single(p => p.Name == "PersonID").Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWithMiscQueryOptions()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryEntries("VipCustomer?$select=FirstName,HomeAddress&$expand=Orders", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var orders = entries.FindAll(e => e.Id.AbsoluteUri.Contains("Orders"));
                    Assert.AreEqual(2, orders.Count);

                    var customer = entries.SingleOrDefault(e => e.Id.AbsoluteUri.Contains("VipCustomer"));
                    Assert.AreEqual(2, customer.Properties.Count());
                }
            }
        }

        [TestMethod]
        public void SelectDerivedPropertyWithoutTypeCastShouldFail()
        {
            this.BadRequest("Boss?$select=City", /*errorCode*/400);
        }

        #endregion

        #region Query singleton property

        [TestMethod]
        public void QuerySingletonProperty()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataProperty property = this.QueryProperty("VipCustomer/PersonID", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, property.Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonPropertyUnderComplexProperty()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataProperty property = this.QueryProperty("VipCustomer/HomeAddress/City", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("London", property.Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonEnumProperty()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataProperty property = this.QueryProperty("Company/CompanyCategory", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataEnumValue enumValue = (ODataEnumValue)property.Value;
                    Assert.AreEqual("IT", enumValue.Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonNavigationProperty()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryFeed("VipCustomer/Orders", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(7, entries.FirstOrDefault().Properties.Single(p => p.Name == "OrderID").Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonPropertyUnderNavigationProperty()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataProperty property = this.QueryProperty("VipCustomer/Orders(8)/OrderDate", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("3/4/2011 4:03:57 PM -08:00", property.Value.ToString());
                }
            }
        }

        [TestMethod]
        public void QueryDerivedSingletonPropertyWithTypeCast()
        {
            foreach (var mimeType in mimeTypes)
            {
                ODataProperty property = this.QueryProperty("Boss/Microsoft.Test.OData.Services.ODataWCFService.Customer/City", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Sydney", property.Value);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonNavigationPropertyWithFilter()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryFeed("VipCustomer/Orders?$filter=OrderID eq 8", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)), entries.FirstOrDefault().Properties.Single(p => p.Name == "OrderDate").Value);
                }
            }
        }

        [TestMethod]
        public void QueryDerivedPropertyWithoutTypeCastShouldFail()
        {
            this.BadRequest("Boss/City", /*errorCode*/404);
        }

        #endregion

        #region Navigation tests

        //Company(singleton)<->Employees(EntitySet)
        [TestMethod]
        public void QueryCollectionOfEntitiesFromSingletonNavigation()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryFeed("Company/Employees", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(2, entries.Count);
                    foreach (var entry in entries)
                    {
                        Assert.IsTrue(entry.Id.AbsoluteUri.Contains("Employees"));
                    }
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWithExpandedCollectionOfEntities()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryEntries("Company?$expand=Employees", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(3, entries.Count);
                    Assert.AreEqual(1, entries.FindAll(e => e.Id.AbsoluteUri.Contains("Company")).Count);
                    Assert.AreEqual(2, entries.FindAll(e => e.Id.AbsoluteUri.Contains("Employees")).Count);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonFromEntitySetNavigation()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entry = this.QueryEntry("Employees(3)/Company", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(0, entry.Properties.Single(p => p.Name == "CompanyID").Value);
                }
            }
        }

        [TestMethod]
        public void QueryEntityWithExpandedSingleton()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryEntries("Employees(3)?$expand=Company", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(2, entries.Count);
                    Assert.AreEqual(1, entries.FindAll(e => e.Id.AbsoluteUri.Contains("Company")).Count);
                    Assert.AreEqual(1, entries.FindAll(e => e.Id.AbsoluteUri.Contains("Employees")).Count);
                }
            }
        }

        //Company(singleton)<->VipCustomer(singleton)
        [TestMethod]
        public void QuerySingletonFromSingletonNavigation()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entry = this.QueryEntry("Company/VipCustomer", mimeType);
                var entry1 = this.QueryEntry("VipCustomer", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertiesAreEqual(entry.Properties, entry1.Properties);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonFromSingletonNavigation2()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entry = this.QueryEntry("VipCustomer/Company", mimeType);
                var entry1 = this.QueryEntry("Company", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertiesAreEqual(entry.Properties, entry1.Properties);
                }
            }
        }

        [TestMethod]
        public void QuerySingletonWithExpandedSingleton()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.QueryEntries("Company?$expand=VipCustomer", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(2, entries.Count);
                    Assert.AreEqual(1, entries.FindAll(e => e.Id.AbsoluteUri.Contains("Company")).Count);
                    Assert.AreEqual(1, entries.FindAll(e => e.Id.AbsoluteUri.Contains("VipCustomer")).Count);
                }
            }
        }

        //Deep navigation test
        [TestMethod]
        public void QueryCollectionOfEntitiesFromDeepLevelNavigation()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entry = this.QueryEntry("VipCustomer/Company/Employees(3)/Company/VipCustomer", mimeType);
                var entry1 = this.QueryEntry("VipCustomer", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertiesAreEqual(entry.Properties, entry1.Properties);
                }
            }
        }

        #endregion

        #region Action/Function

        [TestMethod]
        public void InvokeFunctionBoundedToSingleton()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var amount = messageReader.ReadProperty().Value;
                Assert.AreEqual(2, amount);
            }
        }

        [TestMethod]
        public void InvokeActionBoundedToSingleton()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Company/Microsoft.Test.OData.Services.ODataWCFService.IncreaseRevenue", UriKind.Absolute));
                    requestMessage.SetHeader("Accept", "*/*");
                    requestMessage.Method = "POST";

                    var oldProperty = this.QueryProperty("Company/Revenue", mimeType);
                    Int64 oldValue = (Int64)oldProperty.Value;

                    ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { PayloadBaseUri = ServiceBaseUri };
                    using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
                    {
                        var odataWriter = messageWriter.CreateODataParameterWriter(null);
                        odataWriter.WriteStart();
                        odataWriter.WriteValue("IncreaseValue", 20000);
                        odataWriter.WriteEnd();
                    }
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    ODataProperty property = this.QueryProperty("Company/Revenue", mimeType);
                    Assert.AreEqual((Int64)(oldValue + 20000), property.Value);
                }
            }
        }

        #endregion

        #region Help function

        private List<ODataEntry> QueryFeed(string requestUri, string mimeType)
        {
            List<ODataEntry> entries = new List<ODataEntry>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataFeedReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            ODataEntry entry = reader.Item as ODataEntry;
                            entries.Add(entry);
                        }
                        else if (reader.State == ODataReaderState.FeedEnd)
                        {
                            Assert.IsNotNull(reader.Item as ODataFeed);
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }

        private ODataEntry QueryEntry(string requestUri, string mimeType)
        {
            return this.QueryEntries(requestUri, mimeType).SingleOrDefault();
        }

        /// <summary>
        /// Specifically use to query single entry or multi entries(query with $expand)
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private List<ODataEntry> QueryEntries(string requestUri, string mimeType)
        {
            List<ODataEntry> entries = new List<ODataEntry>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataEntryReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            entries.Add(reader.Item as ODataEntry);
                        }
                    }
                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }

        private ODataProperty QueryProperty(string requestUri, string mimeType)
        {
            ODataProperty property = null;

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));

            requestMessage.SetHeader("Accept", mimeType);
            if (mimeType == MimeTypes.ApplicationAtomXml)
            {
                requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
            }

            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    property = messageReader.ReadProperty();
                }
            }
            return property;
        }

        private void BadRequest(string requestUri, int errorCode)
        {
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));

                requestMessage.SetHeader("Accept", mimeType);
                if (mimeType == MimeTypes.ApplicationAtomXml)
                {
                    requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(errorCode, responseMessage.StatusCode);
            }
        }

        #endregion
    }
}
