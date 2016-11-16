using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaD.CustomFilters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileSizeAttribute : ValidationAttribute, IClientValidatable
    {
        public int? MaxKilobytes { get; set; }

        public FileSizeAttribute(int maxKilobytes)
            : base("Please upload a supported file.")
        {
            MaxKilobytes = maxKilobytes;
            if (MaxKilobytes.HasValue && string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "Please upload a file of less than " + MaxKilobytes.Value + " kilobytes.";
            }
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                bool result = true;

                if (MaxKilobytes.HasValue)
                {
                    result = (file.ContentLength / 1024) < MaxKilobytes.Value;
                }

                return result;
            }

            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metaData, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filesize",
                ErrorMessage = FormatErrorMessage(metaData.DisplayName)
            };

            rule.ValidationParameters.Add("maxkilobytes", MaxKilobytes);

            yield return rule;
        }
    }
}