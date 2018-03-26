using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Tests
{
    [TestClass]
    public class UserEntityTests
    {
        [TestMethod]
        public void UserEntity_MergeFromTwitterModel_ConvertsHoursToMinutes()
        {
            //Arrange
            var userEntity = new UserEntity("test", "testPassword", false);

            var userModel = new UserTwitterViewModel()
            {
                ScheduleHourInterval = 3
            };

            //Act
            userEntity.MergeFromTwitterModel(userModel);

            //Assert
            var expected = 180;
            var actual = userEntity.ScheduleMinuteInterval;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UserEntity_DisconnectFromTwitter_RemovesDetails()
        {
            //Arrange
            var userEntity = new UserEntity("test", "testPassword", false)
            {
                OAuthToken = "abcdefg",
                OAuthTokenSecret = "hijklmnop",
                TwitterId = 1234567,
                TwitterUsername = "mdo"              
            };

            //Act
            userEntity.DisconnectTwitter();

            //Assert
            Assert.AreEqual("", userEntity.TwitterUsername);
            Assert.AreEqual("", userEntity.OAuthTokenSecret);
        }
    }
}
