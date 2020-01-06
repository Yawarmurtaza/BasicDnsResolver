namespace InfraServiceJobPackage.Library.DnsHelper.Records
{
    /// <summary> A DnsResourceRecord not representing any specific resource record. Used if unsupported ResourceRecordTypes are found in the result. </summary>
    public class EmptyRecord : DnsResourceRecord
    {
        /// <summary> Initializes a new instance of the <see cref="EmptyRecord"/> class. </summary>
        /// <param name="info">The information.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="info"/> is null.</exception>
        public EmptyRecord(BaseResourceRecordInfo info) : base(info)
        {
        }

        protected override string RecordToString()
        {
            return string.Empty;
        }
    }
}