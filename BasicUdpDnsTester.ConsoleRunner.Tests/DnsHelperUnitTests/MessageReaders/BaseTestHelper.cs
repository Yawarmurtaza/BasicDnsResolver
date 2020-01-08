using System;
using System.IO;
using System.Reflection;

namespace InfraServiceJobPackageUnitTest.DnsHelperUnitTests.MessageReaders
{
    public abstract class BaseTestHelper
    {
        internal virtual ArraySegment<byte> GetSampleData(ResponseDataSampleFile fileName)
        {
            string debugFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string questionQueryDataFilePath = Path.Combine(debugFolder, $@"DnsHelperUnitTests\TestFiles\{fileName.ToString()}.txt");
            byte[] bytesData = File.ReadAllBytes(questionQueryDataFilePath);
            ArraySegment<byte> responseData = new ArraySegment<byte>(bytesData);
            return responseData;
        }

        internal enum ResponseDataSampleFile 
        {
            /// <summary>
            /// Represents QuestionAndAnswerQuery.txt file.
            /// </summary>
            QuestionAndAnswerQuery,

            /// <summary>
            /// Represents QuestionQuery.txt file.
            /// </summary>
            QuestionQuery
        }
    }
}