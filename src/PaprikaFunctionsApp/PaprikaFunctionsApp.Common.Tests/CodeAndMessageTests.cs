using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaprikaFunctionsApp.Common.Extensions;
using System.Net;

namespace PaprikaFunctionsApp.Common.Tests
{
    [TestClass]
    public class CodeAndMessageTests
    {
        [TestMethod]
        public void GetHttpStatusCodeFor500ShouldReturnInternalServerError()
        {
            var target = new CodeAndMessage(500, "Failed to do it right");
            var expected = HttpStatusCode.InternalServerError;
            var actual = target.GetHttpStatusCode();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetHttpStatusCodeFor201ShouldReturnCreated()
        {
            var target = new CodeAndMessage(201, "1010");
            var expected = HttpStatusCode.Created;
            var actual = target.GetHttpStatusCode();
            Assert.AreEqual(expected, actual);
        }
    }
}
