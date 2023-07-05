using AmplitudeFeatureLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Reflection;

namespace AmplitudeFeatureTests
{
    [TestClass]
    public class AmplitudeFeatureTests
    {
        private const string MOCK_API_KEY = "12345";
        
        [TestInitialize]
        public void Setup()
        {
            ResetStaticHttpClient();        
        }

        [TestMethod]
        public void AmplitudeFeature_implements_Interface()
        {
            var amplitudeFeature = new AmplitudeFeature(MOCK_API_KEY);
            Assert.IsInstanceOfType(amplitudeFeature, typeof(IAmplitudeFeature));
        }

        #region FeatureIsEnabled tests

        [TestMethod]
        public void FeatureIsEnabled_returns_true_when_flag_value_is_on_with_custom_userId()
        {
            var flagName = "demo-flag2";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "on");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName, "TestUser");

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=TestUser&flag_key=demo-flag2", actualRequest);

            Assert.IsTrue(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_true_when_flag_value_is_on()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "on");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);  

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_true_when_flag_value_is_yes()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "yes");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_true_when_flag_value_is_TRUE()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "TRUE");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_true_when_flag_value_is_1()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "1");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_false_when_empty_flag_response()
        {
            var flagName = "demo-flag";
            var mockResponse = @"{}"; // This is the API response when the flag doesn't exist
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsFalse(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_false_when_flag_value_is_not_truthy()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "off");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsFalse(featureEnabled);
        }

        [TestMethod]
        public void FeatureIsEnabled_returns_false_when_flag_value_is_empty()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabled(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsFalse(featureEnabled);
        }

        #endregion

        #region FeatureIsEnabledAsync tests
        [TestMethod]
        public void FeatureIsEnabledAsync_returns_true_when_flag_value_is_on_with_custom_userId()
        {
            var flagName = "demo-flag2";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "on");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName, "TestUser");

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=TestUser&flag_key=demo-flag2", actualRequest);

            Assert.IsTrue(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_true_when_flag_value_is_on()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "on");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_true_when_flag_value_is_yes()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "yes");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_true_when_flag_value_is_TRUE()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "TRUE");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_true_when_flag_value_is_1()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "1");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsTrue(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_false_when_empty_flag_response()
        {
            var flagName = "demo-flag";
            var mockResponse = @"{}"; // This is the API response when the flag doesn't exist
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsFalse(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_false_when_flag_value_is_not_truthy()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "off");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsFalse(featureEnabled.Result);
        }

        [TestMethod]
        public void FeatureIsEnabledAsync_returns_false_when_flag_value_is_empty()
        {
            var flagName = "demo-flag";
            var mockResponse = CreateMockJsonFlagResponse(flagName, "");
            var mockHttpHandler = CreateMockMessageHandler(mockResponse);

            var featureApi = new AmplitudeFeature(MOCK_API_KEY, mockHttpHandler.Object);
            var featureEnabled = featureApi.FeatureIsEnabledAsync(flagName);

            var actualRequest = mockHttpHandler.Invocations[0].Arguments[0] as HttpRequestMessage;
            AssertRequestMessage("GET", "https://api.lab.amplitude.com/v1/vardata?user_id=DEFAULT-USER&flag_key=demo-flag", actualRequest);

            Assert.IsFalse(featureEnabled.Result);
        }
        #endregion

        #region Test helpers
        private void AssertRequestMessage(String expectedHttpMethod, String expectedUri, HttpRequestMessage actualRequest)
        {
            Assert.AreEqual(expectedHttpMethod, actualRequest.Method.ToString(), 
                $"Expected HTTP method: <{expectedHttpMethod}>, actual: <{actualRequest.Method}>");

            Assert.AreEqual(expectedUri, actualRequest.RequestUri.ToString(),
                $"Expected Request URI: <{expectedUri}>, actual: <{actualRequest.RequestUri}>");

            // Assert authorization header is on the request
            var authorizationHeader = actualRequest.Headers.FirstOrDefault(x => x.Key == "Authorization");
            Assert.IsNotNull(authorizationHeader);
            Assert.AreEqual($"Api-Key {MOCK_API_KEY}", authorizationHeader.Value.First());
        }

        private Mock<HttpMessageHandler> CreateMockMessageHandler(string response)
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(response) };
            mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);
 
            return mockHttpHandler;
        }

        private string CreateMockJsonFlagResponse(string flagName, string flagValue)
        {
            // Create a JSON string similiar to: {"demo-flag":{"key": "on"}}
            // This is the response that comes back from the Amplitude API
            var jsonString = "{\"" + flagName + "\":{\"key\": \"" + flagValue + "\"}}";
            Console.WriteLine($"Amplitude API flag response mock: {jsonString}");
            return jsonString;
        }

        private void ResetStaticHttpClient()
        {
            // FeatureApi uses a static client, which is only set once, so we'll cheat and use reflection to reset it
            var type = typeof(AmplitudeFeature);
            var field = type.GetField("client", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, null);
        }
        #endregion
    }
}
