/*
 * SampleLimitRequestWebApi
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using RestSharp;
using Xunit;

using Org.OpenAPITools.Client;
using Org.OpenAPITools.Api;
// uncomment below to import models
//using Org.OpenAPITools.Model;

namespace Org.OpenAPITools.Test.Api
{
    /// <summary>
    ///  Class for testing RequestRateLimitStatusApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class RequestRateLimitStatusApiTests : IDisposable
    {
        private RequestRateLimitStatusApi instance;

        public RequestRateLimitStatusApiTests()
        {
            instance = new RequestRateLimitStatusApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of RequestRateLimitStatusApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' RequestRateLimitStatusApi
            //Assert.IsType<RequestRateLimitStatusApi>(instance);
        }

        /// <summary>
        /// Test ApiRequestRateLimitStatusGetStatusPost
        /// </summary>
        [Fact]
        public void ApiRequestRateLimitStatusGetStatusPostTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //var response = instance.ApiRequestRateLimitStatusGetStatusPost();
            //Assert.IsType<RequestRateLimitStatus>(response);
        }
    }
}