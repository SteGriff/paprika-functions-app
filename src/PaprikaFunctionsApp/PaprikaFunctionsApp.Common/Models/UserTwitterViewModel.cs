using System;

namespace PaprikaFunctionsApp.Common.Models
{
    public class UserTwitterViewModel
    {
        public string TwitterUsername { get; set; }
        public long TwitterId { get; set; }

        public int ScheduleHourInterval { get; set; }
        public string ScheduleQuery { get; set; }
        public bool ScheduleEnable { get; set; }
        public DateTime ScheduleLastPosted { get; set; }

        public UserTwitterViewModel()
        { }

        public UserTwitterViewModel(UserEntity userEntity)
        {
            TwitterUsername = userEntity.TwitterUsername;
            TwitterId = userEntity.TwitterId;
            ScheduleEnable = userEntity.ScheduleEnable;
            ScheduleLastPosted = userEntity.ScheduleLastPosted;
            ScheduleHourInterval = userEntity.ScheduleMinuteInterval / 60;
            ScheduleQuery = userEntity.ScheduleQuery;
        }

        public override string ToString()
        {
            return string.Format("UserTwitterViewModel @{0}, {1}, '{2}' every {3} hours",
                TwitterUsername,
                ScheduleEnable ? "Enabled" : "Disabled",
                ScheduleQuery,
                ScheduleHourInterval);
        }
    }
}
