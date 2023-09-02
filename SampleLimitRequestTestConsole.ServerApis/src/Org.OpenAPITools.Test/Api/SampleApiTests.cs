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

namespace Org.OpenAPITools.Test.Api
{
    /// <summary>
    ///  Class for testing SampleApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class SampleApiTests : IDisposable
    {
        private SampleApi instance;

        public SampleApiTests()
        {
            instance = new SampleApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of SampleApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' SampleApi
            //Assert.IsType<SampleApi>(instance);
        }

        /// <summary>
        /// Test ApiSampleGetLimitGlobal1PreSecondGet
        /// </summary>
        [Fact]
        public void ApiSampleGetLimitGlobal1PreSecondGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? data = null;
            //var response = instance.ApiSampleGetLimitGlobal1PreSecondGet(data);
            //Assert.IsType<string>(response);
        }

        /// <summary>
        /// Test ApiSampleGetLimitGlobal3PreSecond5PerMinutesGet
        /// </summary>
        [Fact]
        public void ApiSampleGetLimitGlobal3PreSecond5PerMinutesGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? data = null;
            //var response = instance.ApiSampleGetLimitGlobal3PreSecond5PerMinutesGet(data);
            //Assert.IsType<string>(response);
        }

        /// <summary>
        /// Test ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGet
        /// </summary>
        [Fact]
        public void ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? data = null;
            //var response = instance.ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGet(data);
            //Assert.IsType<string>(response);
        }

        /// <summary>
        /// Test ApiSampleGetLimitUser3PreSecondGet
        /// </summary>
        [Fact]
        public void ApiSampleGetLimitUser3PreSecondGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? data = null;
            //var response = instance.ApiSampleGetLimitUser3PreSecondGet(data);
            //Assert.IsType<string>(response);
        }

        /// <summary>
        /// Test ApiSampleGetNormalGet
        /// </summary>
        [Fact]
        public void ApiSampleGetNormalGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? data = null;
            //var response = instance.ApiSampleGetNormalGet(data);
            //Assert.IsType<string>(response);
        }

        /// <summary>
        /// Test ApiSamplePostLimit10BytesSizePost
        /// </summary>
        [Fact]
        public void ApiSamplePostLimit10BytesSizePostTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? body = null;
            //var response = instance.ApiSamplePostLimit10BytesSizePost(body);
            //Assert.IsType<string>(response);
        }

        /// <summary>
        /// Test ApiSamplePostNormalPost
        /// </summary>
        [Fact]
        public void ApiSamplePostNormalPostTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string? body = null;
            //var response = instance.ApiSamplePostNormalPost(body);
            //Assert.IsType<string>(response);
        }
    }
}