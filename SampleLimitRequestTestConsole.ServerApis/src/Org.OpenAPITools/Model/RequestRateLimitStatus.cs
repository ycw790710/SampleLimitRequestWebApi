/*
 * SampleLimitRequestWebApi
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Org.OpenAPITools.Client.OpenAPIDateConverter;

namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// RequestRateLimitStatus
    /// </summary>
    [DataContract(Name = "RequestRateLimitStatus")]
    public partial class RequestRateLimitStatus : IEquatable<RequestRateLimitStatus>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRateLimitStatus" /> class.
        /// </summary>
        /// <param name="updatedTime">updatedTime.</param>
        /// <param name="typesContainers">typesContainers.</param>
        public RequestRateLimitStatus(DateTime updatedTime = default(DateTime), Dictionary<string, List<RequestRateLimitStatusContainer>> typesContainers = default(Dictionary<string, List<RequestRateLimitStatusContainer>>))
        {
            this.UpdatedTime = updatedTime;
            this.TypesContainers = typesContainers;
        }

        /// <summary>
        /// Gets or Sets UpdatedTime
        /// </summary>
        [DataMember(Name = "updatedTime", EmitDefaultValue = false)]
        public DateTime UpdatedTime { get; set; }

        /// <summary>
        /// Gets or Sets TypesContainers
        /// </summary>
        [DataMember(Name = "typesContainers", EmitDefaultValue = true)]
        public Dictionary<string, List<RequestRateLimitStatusContainer>> TypesContainers { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class RequestRateLimitStatus {\n");
            sb.Append("  UpdatedTime: ").Append(UpdatedTime).Append("\n");
            sb.Append("  TypesContainers: ").Append(TypesContainers).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as RequestRateLimitStatus);
        }

        /// <summary>
        /// Returns true if RequestRateLimitStatus instances are equal
        /// </summary>
        /// <param name="input">Instance of RequestRateLimitStatus to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RequestRateLimitStatus input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.UpdatedTime == input.UpdatedTime ||
                    (this.UpdatedTime != null &&
                    this.UpdatedTime.Equals(input.UpdatedTime))
                ) && 
                (
                    this.TypesContainers == input.TypesContainers ||
                    this.TypesContainers != null &&
                    input.TypesContainers != null &&
                    this.TypesContainers.SequenceEqual(input.TypesContainers)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.UpdatedTime != null)
                {
                    hashCode = (hashCode * 59) + this.UpdatedTime.GetHashCode();
                }
                if (this.TypesContainers != null)
                {
                    hashCode = (hashCode * 59) + this.TypesContainers.GetHashCode();
                }
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
