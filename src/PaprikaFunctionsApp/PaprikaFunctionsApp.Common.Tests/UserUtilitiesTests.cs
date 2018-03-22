using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Tests
{
    [TestClass]
    public class UserUtilitiesTests
    {
        [TestMethod]
        public void ValidatePassword_RefusesShortPassword()
        {
            string badPassword = "sup";
            var mockStorage = new AzureStorageProvider("");

            var expected = false;
            var actual = new UserUtilities(mockStorage).ValidatePassword(badPassword).Success;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidatePassword_RefusesPasswordContainingSeparator()
        {
            string badPassword = "MySneaky----IDENTIFIER---Password";
            var mockStorage = new AzureStorageProvider("");

            var expected = false;
            var actual = new UserUtilities(mockStorage).ValidatePassword(badPassword).Success;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidatePassword_AcceptsGoodPassword()
        {
            string goodPassword = "BlueWheelieBin1010";
            var mockStorage = new AzureStorageProvider("");

            var expected = new Status<string>(true);
            var actual = new UserUtilities(mockStorage).ValidatePassword(goodPassword);

            Assert.AreEqual(expected, actual);
        }
    }
}
