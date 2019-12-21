namespace InfraServiceJobPackage.Library.DnsHelper.RequestMessageModel
{
    public interface IDnsString
    {
        /// <summary>Gets the original query that was given.</summary>
        string Original { get; }
        
        /// <summary>Gets the parsed value of the original.</summary>
        string Value { get; }
        IDnsString RootLabel { get; }
        bool Equals(object obj);
        IDnsString FromResponseQueryString(string query);
        int GetHashCode();
        IDnsString Parse(string query);
        string ToString();
    }
}