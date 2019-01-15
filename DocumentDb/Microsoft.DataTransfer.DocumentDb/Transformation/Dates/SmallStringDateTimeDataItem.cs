using System;
using Microsoft.DataTransfer.Extensibility;

namespace Microsoft.DataTransfer.DocumentDb.Transformation.Dates
{
    sealed class StringWithNoUnneededPrecisionUnderTheSecondDateTimeDataItem : ConvertedDateTimeDataItemBase
    {
        public StringWithNoUnneededPrecisionUnderTheSecondDateTimeDataItem(IDataItem dataItem)
            : base(dataItem) { }

        protected override object ConvertDateTime(DateTime timeStamp)
        {
            return DateTimeConverter.ToStringWithNoUnneededPrecisionUnderTheSecond(timeStamp);
        }

        protected override IDataItem TransformDataItem(IDataItem original)
        {
            return new StringWithNoUnneededPrecisionUnderTheSecondDateTimeDataItem(original);
        }
    }
}