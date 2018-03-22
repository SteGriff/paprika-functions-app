using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            string badPassword = "MySneaky----IDENTIFIER----Password";
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

            var expected = true;
            var actual = new UserUtilities(mockStorage).ValidatePassword(goodPassword).Success;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidateUsername_RefusesShort()
        {
            string bad = "i";
            var mockStorage = new AzureStorageProvider("");

            var expected = false;
            var actual = new UserUtilities(mockStorage).ValidatePassword(bad).Success;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidateUsername_RefusesContainingSeparator()
        {
            string bad = "MySneaky----IDENTIFIER----Name";
            var mockStorage = new AzureStorageProvider("");

            var expected = false;
            var actual = new UserUtilities(mockStorage).ValidatePassword(bad).Success;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidateUsername_AcceptsGood()
        {
            string good = "SteGriff";
            var mockStorage = new AzureStorageProvider("");

            var expected = true;
            var actual = new UserUtilities(mockStorage).ValidatePassword(good).Success;

            Assert.AreEqual(expected, actual);
        }
    }
}
