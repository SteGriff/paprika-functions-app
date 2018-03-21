using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaprikaFunctionsApp.Common.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace PaprikaFunctionsApp.Common.Tests
{
    [TestClass]
    public class HttpRequestMessageExtensionsTests
    {
        [TestMethod]
        public void AddAuthenticationHeadersFromIdentifier_DecodesAGoodIdentifier()
        {
            var source = "U3ZnR3V5LS0tLUlERU5USUZJRVItLS0tU3ZnR3V5UGFzc3dvcmQ=";
            var req = new HttpRequestMessage();
            req.AddAuthenticationHeadersFromIdentifier(source);

            Assert.IsTrue(req.Headers.Contains("username"));

            var expectedUsername = "SvgGuy";
            var actualUsername = req.Headers.GetValues("username").FirstOrDefault();

            Assert.AreEqual(expectedUsername, actualUsername);
        }
        
    }
}
