using GlobalTimers.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace RequestRateLimit.Services.Tests
{
    [TestClass()]
    public class RequestRateLimitServiceTests
    {
        [TestMethod()]
        public void IsRequestOverLimitTest()
        {
            var offset1s = new TimeSpan(0, 0, 1);
            var offset1m = new TimeSpan(0, 1, 0);
            var offset1h = new TimeSpan(1, 0, 0);
            var testdatas = 
                new (TimeSpan clear_offset, string controllerName, string actionName)[] {
                (offset1s, nameof(FakeGlobalActions), nameof(FakeGlobalActions.Action3s)),
                (offset1m, nameof(FakeGlobalActions), nameof(FakeGlobalActions.Action3m)),
                (offset1m, nameof(FakeGlobalActions), nameof(FakeGlobalActions.Action3m4m)),
                (offset1m, nameof(FakeGlobalActions), nameof(FakeGlobalActions.Action4m3m)),
                (offset1h, nameof(FakeGlobalActions), nameof(FakeGlobalActions.Action3h)),
                (offset1s, nameof(FakeGlobalController3s), nameof(FakeGlobalController3s.Action)),
                (offset1m, nameof(FakeGlobalController3m), nameof(FakeGlobalController3m.Action)),
                (offset1m, nameof(FakeGlobalController4m3m), nameof(FakeGlobalController4m3m.Action)),
                (offset1m, nameof(FakeGlobalController3m4m), nameof(FakeGlobalController3m4m.Action)),
                (offset1h, nameof(FakeGlobalController3h), nameof(FakeGlobalController3h.Action)),
            };
            foreach (var testdata in testdatas)
                IsRequestOverLimitTestBase(testdata.controllerName, testdata.actionName, testdata.clear_offset, 3);

            var testIpdatas =
                new (TimeSpan clear_offset, string controllerName, string actionName)[] {
                (offset1s, nameof(FakeIpGlobalActions), nameof(FakeIpGlobalActions.Action3s)),
                (offset1m, nameof(FakeIpGlobalActions), nameof(FakeIpGlobalActions.Action3m)),
                (offset1m, nameof(FakeIpGlobalActions), nameof(FakeIpGlobalActions.Action3m4m)),
                (offset1m, nameof(FakeIpGlobalActions), nameof(FakeIpGlobalActions.Action4m3m)),
                (offset1h, nameof(FakeIpGlobalActions), nameof(FakeIpGlobalActions.Action3h)),
                (offset1s, nameof(FakeIpGlobalController3s), nameof(FakeIpGlobalController3s.Action)),
                (offset1m, nameof(FakeIpGlobalController3m), nameof(FakeIpGlobalController3m.Action)),
                (offset1m, nameof(FakeIpGlobalController4m3m), nameof(FakeIpGlobalController4m3m.Action)),
                (offset1m, nameof(FakeIpGlobalController3m4m), nameof(FakeIpGlobalController3m4m.Action)),
                (offset1h, nameof(FakeIpGlobalController3h), nameof(FakeIpGlobalController3h.Action)),
            };
            foreach (var testIpdata in testIpdatas)
                IsIpRequestOverLimitTestBase(testIpdata.controllerName, testIpdata.actionName, testIpdata.clear_offset, 3);
        }
        private void IsRequestOverLimitTestBase(string controllerName, string actionName, TimeSpan clear_offset, int limit)
        {
            // test parameters
            string httpMethod = "GET";
            string ipAddress = "::1";
            (TypeInfo controllerInfo, MethodInfo actionInfo) = GetInfos(controllerName, actionName);

            // test instances
            var specialFakeGlobalTimerService = new SpecialFakeGlobalTimerService();
            var requestRateLimitService = GetRequestRateLimitService(specialFakeGlobalTimerService);

            // test action
            var testHasIpAction = () =>
            {
                return requestRateLimitService.IsRequestOverLimit(httpMethod,
                   actionInfo, controllerInfo, controllerName, actionName,
                   ipAddress);
            };
            var testNoIpAction = () =>
            {
                return requestRateLimitService.IsRequestOverLimit(httpMethod,
                   actionInfo, controllerInfo, controllerName, actionName,
                   null);
            };

            // test

            for (int i = 0; i < 999; i++)
            {
                var res = testNoIpAction();
                Assert.IsTrue(res);
            }

            for (int i = 0; i < limit; i++)
            {
                var res = testHasIpAction();
                Assert.IsFalse(res);
            }
            var res_4 = testHasIpAction();
            Assert.IsTrue(res_4);

            specialFakeGlobalTimerService.AddOffset(clear_offset);

            for (int i = 0; i < limit; i++)
            {
                var res = testHasIpAction();
                Assert.IsFalse(res);
            }
            var res_8 = testHasIpAction();
            Assert.IsTrue(res_8);
        }
        private void IsIpRequestOverLimitTestBase(string controllerName, string actionName, TimeSpan clear_offset, int limit)
        {
            // test parameters
            string httpMethod = "GET";
            (TypeInfo controllerInfo, MethodInfo actionInfo) = GetInfos(controllerName, actionName);

            // test instances
            var specialFakeGlobalTimerService = new SpecialFakeGlobalTimerService();
            var requestRateLimitService = GetRequestRateLimitService(specialFakeGlobalTimerService);

            // test action
            var testHasIpAction = (string ipAddress) =>
            {
                return requestRateLimitService.IsRequestOverLimit(httpMethod,
                   actionInfo, controllerInfo, controllerName, actionName,
                   ipAddress);
            };
            var testNoIpAction = () =>
            {
                return requestRateLimitService.IsRequestOverLimit(httpMethod,
                   actionInfo, controllerInfo, controllerName, actionName,
                   null);
            };

            // test
            var ipAddress1 = "ip1";
            var ipAddress2 = "ip2";

            for (int i = 0; i < 999; i++)
            {
                var res = testNoIpAction();
                Assert.IsTrue(res);
            }

            for (int i = 0; i < limit; i++)
            {
                var res = testHasIpAction(ipAddress1);
                Assert.IsFalse(res);
            }
            var res_1_4 = testHasIpAction(ipAddress1);
            Assert.IsTrue(res_1_4);

            for (int i = 0; i < limit; i++)
            {
                var res = testHasIpAction(ipAddress2);
                Assert.IsFalse(res);
            }
            var res_2_4 = testHasIpAction(ipAddress2);
            Assert.IsTrue(res_2_4);

            specialFakeGlobalTimerService.AddOffset(clear_offset);

            for (int i = 0; i < limit; i++)
            {
                var res = testHasIpAction(ipAddress1);
                Assert.IsFalse(res);
            }
            var res_1_8 = testHasIpAction(ipAddress1);
            Assert.IsTrue(res_1_8);

            for (int i = 0; i < limit; i++)
            {
                var res = testHasIpAction(ipAddress2);
                Assert.IsFalse(res);
            }
            var res_2_8 = testHasIpAction(ipAddress2);
            Assert.IsTrue(res_2_8);
        }

        [TestMethod()]
        public void IsUserRequestOverLimitTest()
        {
            var offset1s = new TimeSpan(0, 0, 1);
            var offset1m = new TimeSpan(0, 1, 0);
            var offset1h = new TimeSpan(1, 0, 0);
            (TimeSpan clear_offset, string controllerName, string actionName)[] testdatas =
                new (TimeSpan clear_offset, string controllerName, string actionName)[] {
                (offset1s, nameof(FakeUserGlobalActions), nameof(FakeUserGlobalActions.Action3s)),
                (offset1m, nameof(FakeUserGlobalActions), nameof(FakeUserGlobalActions.Action3m)),
                (offset1m, nameof(FakeUserGlobalActions), nameof(FakeUserGlobalActions.Action3m4m)),
                (offset1m, nameof(FakeUserGlobalActions), nameof(FakeUserGlobalActions.Action4m3m)),
                (offset1h, nameof(FakeUserGlobalActions), nameof(FakeUserGlobalActions.Action3h)),
                (offset1s, nameof(FakeUserGlobalController3s), nameof(FakeUserGlobalController3s.Action)),
                (offset1m, nameof(FakeUserGlobalController3m), nameof(FakeUserGlobalController3m.Action)),
                (offset1m, nameof(FakeUserGlobalController4m3m), nameof(FakeUserGlobalController4m3m.Action)),
                (offset1m, nameof(FakeUserGlobalController3m4m), nameof(FakeUserGlobalController3m4m.Action)),
                (offset1h, nameof(FakeUserGlobalController3h), nameof(FakeUserGlobalController3h.Action)),
            };
            foreach (var testdata in testdatas)
                IsUserRequestOverLimitTestBase(testdata.controllerName, testdata.actionName, testdata.clear_offset, 3);

        }
        private void IsUserRequestOverLimitTestBase(string controllerName, string actionName, TimeSpan clear_offset, int limit)
        {
            // test parameters
            string httpMethod = "GET";
            (TypeInfo controllerInfo, MethodInfo actionInfo) = GetInfos(controllerName, actionName);

            // test instances
            var specialFakeGlobalTimerService = new SpecialFakeGlobalTimerService();
            var requestRateLimitService = GetRequestRateLimitService(specialFakeGlobalTimerService);

            // test action
            var testAction = (int userId) => {
                return requestRateLimitService.IsUserRequestOverLimit(httpMethod,
                   actionInfo, controllerInfo, controllerName, actionName,
                   userId);
            };
            var testNoUserIdAction = () => {
                return requestRateLimitService.IsUserRequestOverLimit(httpMethod,
                   actionInfo, controllerInfo, controllerName, actionName,
                   null);
            };

            // test
            var user1 = 1;
            var user2 = 2;

            for (int i = 0; i < 999; i++)
            {
                var res = testNoUserIdAction();
                Assert.IsFalse(res);
            }

            for (int i = 0; i < limit; i++)
            {
                var res = testAction(user1);
                Assert.IsFalse(res);
            }
            var res_1_4 = testAction(user1);
            Assert.IsTrue(res_1_4);
            for (int i = 0; i < limit; i++)
            {
                var res = testAction(user2);
                Assert.IsFalse(res);
            }
            var res_2_4 = testAction(user2);
            Assert.IsTrue(res_2_4);

            specialFakeGlobalTimerService.AddOffset(clear_offset);

            for (int i = 0; i < limit; i++)
            {
                var res = testAction(user1);
                Assert.IsFalse(res);
            }
            var res_1_8 = testAction(user1);
            Assert.IsTrue(res_1_8);
            for (int i = 0; i < limit; i++)
            {
                var res = testAction(user2);
                Assert.IsFalse(res);
            }
            var res_2_8 = testAction(user2);
            Assert.IsTrue(res_2_8);

        }

        private static RequestRateLimitService GetRequestRateLimitService(IGlobalTimerService globalTimerService)
        {
            var fakeRequestRateLimitStatusCacheService =
                new FakeRequestRateLimitStatusCacheService();
            var requestRateLimitCacheService = new RequestRateLimitCacheService(fakeRequestRateLimitStatusCacheService,
                globalTimerService);
            var requestRateLimitService = new RequestRateLimitService(requestRateLimitCacheService);
            return requestRateLimitService;
        }

        private (TypeInfo controllerInfo, MethodInfo actionInfo) GetInfos(
            string controllerName, string actionName)
        {
            var assembly = Assembly.GetAssembly(GetType());
            if (assembly == null) throw new Exception("ERROR");
            var typeInfo = assembly.GetTypes()
                .First(n => n.Name == controllerName).GetTypeInfo();
            if (typeInfo == null) throw new Exception("ERROR");
            var methodInfo = typeInfo.GetMethod(actionName);
            if (methodInfo == null) throw new Exception("ERROR");
            return (typeInfo, methodInfo);
        }

    }

    class FakeRequestRateLimitStatusCacheService : IRequestRateLimitStatusCacheService
    {
        public byte[] GetStatusInfoJsonBytes() => Array.Empty<byte>();
        public string? GetStatusJson() => null;
        public byte[] GetStatusJsonBytes() => Array.Empty<byte>();
        public void SendContainer(RequestRateLimitStatusContainerActionType actionType, RequestRateLimitStatusContainer container)
        {
        }
        public void UpdateStatuses()
        {
        }
    }

    #region FakeGlobalActions
    class FakeGlobalActions
    {
        [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        public void Action3s() { }

        [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action3m() { }

        [GlobalRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
        [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action4m3m() { }

        [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        [GlobalRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action3m4m() { }

        [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Hours)]
        public void Action3h() { }
    }
    #endregion

    #region FakeGlobalControllers
    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    class FakeGlobalController3s
    {
        public void Action() { }
    }

    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeGlobalController3m
    {
        public void Action() { }
    }

    [GlobalRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeGlobalController4m3m
    {
        public void Action() { }
    }

    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    [GlobalRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeGlobalController3m4m
    {
        public void Action() { }
    }

    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Hours)]
    class FakeGlobalController3h
    {
        public void Action() { }
    }
    #endregion

    #region FakeIpGlobalActions
    class FakeIpGlobalActions
    {
        [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        public void Action3s() { }

        [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action3m() { }

        [IpRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
        [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action4m3m() { }

        [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        [IpRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action3m4m() { }

        [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Hours)]
        public void Action3h() { }
    }
    #endregion

    #region FakeIpGlobalControllers
    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    class FakeIpGlobalController3s
    {
        public void Action() { }
    }

    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeIpGlobalController3m
    {
        public void Action() { }
    }

    [IpRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeIpGlobalController4m3m
    {
        public void Action() { }
    }

    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    [IpRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeIpGlobalController3m4m
    {
        public void Action() { }
    }

    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Hours)]
    class FakeIpGlobalController3h
    {
        public void Action() { }
    }
    #endregion

    #region FakeUserGlobalActions
    class FakeUserGlobalActions
    {
        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        public void Action3s() { }

        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action3m() { }

        [UserRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action4m3m() { }

        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
        [UserRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
        public void Action3m4m() { }

        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Hours)]
        public void Action3h() { }
    }
    #endregion

    #region FakeUserGlobalControllers
    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    class FakeUserGlobalController3s
    {
        public void Action() { }
    }

    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeUserGlobalController3m
    {
        public void Action() { }
    }

    [UserRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeUserGlobalController4m3m
    {
        public void Action() { }
    }

    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Minutes)]
    [UserRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    class FakeUserGlobalController3m4m
    {
        public void Action() { }
    }

    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Hours)]
    class FakeUserGlobalController3h
    {
        public void Action() { }
    }
    #endregion

    class SpecialFakeGlobalTimerService : IGlobalTimerService
    {
        private TimeSpan _offset;
        public SpecialFakeGlobalTimerService()
        {
            _offset = TimeSpan.Zero;
        }

        public void Add1s()
        {
            _offset = _offset.Add(new TimeSpan(0, 0, 1));
        }

        public void Add1m()
        {
            _offset = _offset.Add(new TimeSpan(0, 1, 0));
        }

        public void Add1h()
        {
            _offset = _offset.Add(new TimeSpan(1, 0, 0));
        }

        public void AddOffset(TimeSpan add)
        {
            _offset = _offset.Add(add);
        }

        public TimeSpan NowTimeSpan()
        {
            return new TimeSpan(NowTicks()).Add(_offset);
        }
        public TimeSpan AddTimeSpanForNowTimeSpan(TimeSpan addTime)
        {
            return NowTimeSpan().Add(addTime);
        }
        public TimeSpan AddMillisecondsForNowTimeSpan(int addMilliseconds)
        {
            return NowTimeSpan().Add(new TimeSpan(0, 0, 0, 0, addMilliseconds));
        }

        public long NowTicks()
        {
            return Stopwatch.GetTimestamp();
        }
        public long AddTimeSpanForNowTicks(TimeSpan addTime)
        {
            return AddTimeSpanForNowTimeSpan(addTime).Ticks;
        }
        public long AddMillisecondsForNowTicks(int addMilliseconds)
        {
            return AddMillisecondsForNowTimeSpan(addMilliseconds).Ticks;
        }
    }
}