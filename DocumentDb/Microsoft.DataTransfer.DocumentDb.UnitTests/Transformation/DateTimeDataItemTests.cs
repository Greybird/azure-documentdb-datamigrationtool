using Microsoft.DataTransfer.DocumentDb.Transformation.Dates;
using Microsoft.DataTransfer.Extensibility;
using Microsoft.DataTransfer.Extensibility.Basics.Source;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.DataTransfer.DocumentDb.UnitTests.Transformation
{
    [TestClass]
    public class DateTimeDataItemTests
    {
        [TestMethod]
        public void GetValue_TopLevelStringDateTimeTransformation_ReturnsString()
        {
            var transformed = new StringDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Hello world!" },
                    { "DateTimeProperty", new DateTime(2015, 3, 1, 20, 10, 5, DateTimeKind.Utc) }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "DateTimeProperty" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Hello world!", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);
            Assert.AreEqual("2015-03-01T20:10:05.0000000Z", transformed.GetValue("DateTimeProperty"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        public void GetValue_NestedStringDateTimeTransformation_ReturnsString()
        {
            var transformed = new StringDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Nested world!" },
                    { "Nested", new Dictionary<string, object>
                        {
                            { "DateTimeProperty", new DateTime(2010, 5, 3, 1, 0, 0, DateTimeKind.Utc) }
                        }
                    }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "Nested" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Nested world!", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);

            var nested = transformed.GetValue("Nested") as IDataItem;

            Assert.IsNotNull(nested, TestResources.NullNestedDataItem);

            CollectionAssert.AreEquivalent(new[] { "DateTimeProperty" }, nested.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("2010-05-03T01:00:00.0000000Z", nested.GetValue("DateTimeProperty"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        public void GetValue_TopLevelEpochDateTimeTransformation_ReturnsNumber()
        {
            var transformed = new EpochDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Flat document" },
                    { "DateTimeProperty", new DateTime(2015, 3, 1, 20, 10, 5, DateTimeKind.Utc) }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "DateTimeProperty" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Flat document", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);
            Assert.AreEqual(1425240605L, transformed.GetValue("DateTimeProperty"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        public void GetValue_NestedEpochDateTimeTransformation_ReturnsNumber()
        {
            var transformed = new EpochDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Nested document" },
                    { "Nested", new Dictionary<string, object>
                        {
                            { "DateTimeProperty", new DateTime(2010, 5, 3, 1, 0, 0, DateTimeKind.Utc) }
                        }
                    }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "Nested" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Nested document", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);

            var nested = transformed.GetValue("Nested") as IDataItem;

            Assert.IsNotNull(nested, TestResources.NullNestedDataItem);

            CollectionAssert.AreEquivalent(new[] { "DateTimeProperty" }, nested.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual(1272848400L, nested.GetValue("DateTimeProperty"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        public void GetValue_TopLevelStringAndEpochDateTimeTransformation_ReturnsNumber()
        {
            var transformed = new StringAndEpochDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Hello world!" },
                    { "DateTimeProperty", new DateTime(2012, 11, 4, 8, 55, 0, DateTimeKind.Utc) }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "DateTimeProperty" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Hello world!", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);

            var dateTimeDataItem = transformed.GetValue("DateTimeProperty") as IDataItem;

            Assert.IsNotNull(dateTimeDataItem, TestResources.InvalidFieldValue);

            CollectionAssert.AreEquivalent(new[] { "Value", "Epoch" }, dateTimeDataItem.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("2012-11-04T08:55:00.0000000Z", dateTimeDataItem.GetValue("Value"), TestResources.InvalidFieldValue);
            Assert.AreEqual(1352019300L, dateTimeDataItem.GetValue("Epoch"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        public void GetValue_NestedStringAndEpochDateTimeTransformation_ReturnsNumber()
        {
            var transformed = new StringAndEpochDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Nested world!" },
                    { "Nested", new Dictionary<string, object>
                        {
                            { "DateTimeProperty", new DateTime(2001, 4, 8, 12, 0, 0, DateTimeKind.Utc) }
                        }
                    }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "Nested" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Nested world!", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);

            var nested = transformed.GetValue("Nested") as IDataItem;

            Assert.IsNotNull(nested, TestResources.NullNestedDataItem);

            CollectionAssert.AreEquivalent(new[] { "DateTimeProperty" }, nested.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            var dateTimeDataItem = nested.GetValue("DateTimeProperty") as IDataItem;

            Assert.IsNotNull(dateTimeDataItem, TestResources.InvalidFieldValue);

            CollectionAssert.AreEquivalent(new[] { "Value", "Epoch" }, dateTimeDataItem.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("2001-04-08T12:00:00.0000000Z", dateTimeDataItem.GetValue("Value"), TestResources.InvalidFieldValue);
            Assert.AreEqual(986731200L, dateTimeDataItem.GetValue("Epoch"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        [DataRow("2000-02-01T00:00:00")]
        [DataRow("2017-03-23T15:04:26.492626Z")]
        [DataRow("9999-12-31T23:59:59.997")]
        [DataRow("1986-02-25T00:00:00")]
        [DataRow("2000-01-01T00:00:00")]
        [DataRow("9999-12-31T23:59:59.9999999")]
        public void GetValue_TopLevelStringWithNoUnneededPrecisionUnderTheSecondDateTimeTransformation_ReturnsString(string source)
        {
            var transformed = new StringWithNoUnneededPrecisionUnderTheSecondDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Hello world!" },
                    { "DateTimeProperty", DateTime.Parse(source, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "DateTimeProperty" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Hello world!", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);
            Assert.AreEqual(source, transformed.GetValue("DateTimeProperty"), TestResources.InvalidFieldValue);
        }

        [TestMethod]
        [DataRow("2000-02-01T00:00:00")]
        [DataRow("2017-03-23T15:04:26.492626Z")]
        [DataRow("9999-12-31T23:59:59.997")]
        [DataRow("1986-02-25T00:00:00")]
        [DataRow("2000-01-01T00:00:00")]
        [DataRow("9999-12-31T23:59:59.9999999")]
        public void GetValue_NestedStringWithNoUnneededPrecisionUnderTheSecondDateTimeTransformation_ReturnsString(string source)
        {
            var transformed = new StringWithNoUnneededPrecisionUnderTheSecondDateTimeDataItem(
                new DictionaryDataItem(new Dictionary<string, object>
                {
                    { "StringProperty", "Nested world!" },
                    { "Nested", new Dictionary<string, object>
                        {
                            { "DateTimeProperty", DateTime.Parse(source, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) }
                        }
                    }
                }));

            CollectionAssert.AreEquivalent(new[] { "StringProperty", "Nested" }, transformed.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual("Nested world!", transformed.GetValue("StringProperty"), TestResources.InvalidFieldValue);

            var nested = transformed.GetValue("Nested") as IDataItem;

            Assert.IsNotNull(nested, TestResources.NullNestedDataItem);

            CollectionAssert.AreEquivalent(new[] { "DateTimeProperty" }, nested.GetFieldNames().ToArray(),
                TestResources.InvalidFieldNames);

            Assert.AreEqual(source, nested.GetValue("DateTimeProperty"), TestResources.InvalidFieldValue);
        }
    }
}
