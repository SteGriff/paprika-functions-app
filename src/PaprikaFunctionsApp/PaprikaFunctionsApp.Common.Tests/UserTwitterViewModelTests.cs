using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Tests
{
    [TestClass]
    public class UserTwitterViewModelTests
    {
        [TestMethod]
        public void UserTwitterViewModelCtor_ChangesHoursToMinutes()
        {
            var userEntity = new UserEntity("test", "testPassword", false)
            {
                ScheduleMinuteInterval = 120
            };

            var userModel = new UserTwitterViewModel(userEntity);

            var expectedHours = 2;
            var actualHours = userModel.ScheduleHourInterval;

            Assert.AreEqual(expectedHours, actualHours);
        }
    }
}
