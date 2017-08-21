using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.Interfaces;
using TQL.Common.Timezone;

namespace TQL.Common.Tests
{
    [TestClass]
    public class DaylightSavingTimeTrackerTests
    {

        private class TestEvaluator : IFireTimeEvaluator
        {
            private int _index = 0;
            private readonly DateTimeOffset?[] _scores;

            public TestEvaluator(DateTimeOffset?[] scores)
            {
                _scores = scores;
            }
            public bool IsSatisfiedBy(DateTimeOffset dateTime)
            {
                throw new NotImplementedException();
            }

            public DateTimeOffset? NextFire()
            {
                return _scores[_index++];
            }
        }

        [TestMethod]
        public void DaylightSavingTime_MonthsResolution_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[]
            {
                new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2019, 5, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero)
            });

            Assert.AreEqual(DateTimeOffset.Parse("01.01.2018 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("01.01.2019 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("01.05.2019 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("01.01.2020 00:00:00"), evaluator.NextFire());
        }
        [TestMethod]
        public void DaylightSavingTime_DayResolution_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[]
            {
                new DateTimeOffset(2017, 10, 29, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 10, 30, 0, 0, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 00:00:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("30.10.2017 00:00:00 +01:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_HoursResolution_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[]
            {
                new DateTimeOffset(2017, 10, 29, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 10, 29, 5, 0, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 00:00:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 05:00:00 +01:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_HoursResolution_WithShiftingBackTime_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[]
            {
                new DateTimeOffset(2017, 10, 29, 1, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 10, 29, 2, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 10, 29, 3, 0, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 01:00:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:00:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:00:00 +01:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_HoursResolution_WithShiftingBackTime2_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[] {
                new DateTimeOffset(2017, 10, 29, 1, 55, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 10, 29, 3, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 10, 29, 4, 5, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 01:55:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:00:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 03:05:00 +01:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_HoursResolution_WithShiftingForwardTime_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[] {
                new DateTimeOffset(2017, 3, 26, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 1, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 3, 0, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 00:00:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:00:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:00:00 +02:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_MinutesResolution_WithShiftingForwardTime_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[] {
                new DateTimeOffset(2017, 3, 26, 1, 58, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 3, 1, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 3, 4, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:58:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:01:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:04:00 +02:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_HoursResolution2_WithShiftingForwardTime_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[] {
                new DateTimeOffset(2017, 3, 26, 1, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 3, 0, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:00:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:00:00 +02:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_HoursResolution3_WithShiftingForwardTime_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[] {
                new DateTimeOffset(2017, 3, 26, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 1, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 2, 0, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 00:00:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:00:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:00:00 +02:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void DaylightSavingTime_MinutesResolution2_WithShiftingForwardTime_ShouldPass()
        {
            var evaluator = CreateEvaluator(new DateTimeOffset?[] {
                new DateTimeOffset(2017, 3, 26, 1, 58, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 2, 1, 0, TimeSpan.Zero),
                new DateTimeOffset(2017, 3, 26, 2, 4, 0, TimeSpan.Zero),
            });

            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:58:00 +01:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:01:00 +02:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:04:00 +02:00"), evaluator.NextFire());
        }

        private IFireTimeEvaluator CreateEvaluator(DateTimeOffset?[] scores)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return new DaylightSavingTimeTracker(timeZone, new TimeZoneAdjuster(timeZone, new TestEvaluator(scores)));
        }
    }
}
