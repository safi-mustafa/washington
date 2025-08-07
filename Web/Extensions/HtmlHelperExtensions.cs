using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Helpers.Extensions;

namespace Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IEnumerable<SelectListItem> GetEnumSelectListWithDefaultValue<TEnum>(this IHtmlHelper htmlHelper, TEnum? defaultValue, string? defaultLabel = null)
            where TEnum : struct
        {
            var selectList = htmlHelper.GetEnumSelectList<TEnum>().ToList();
            selectList.Insert(0, new SelectListItem { Text = (defaultLabel ?? "Select All"), Value = "-1" });
            selectList.Single(x => x.Value == "-1").Selected = true;
            return selectList;
        }

        public static SelectList GetEnumSelectListWithStrings<TEnum>() where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum))
                             .Cast<TEnum>()
                             .Select(e => new SelectListItem
                             {
                                 Value = e.GetEnumDescription(), // Convert enum value to string
                                 Text = e.ToString()
                             });

            return new SelectList(values, "Value", "Text");
        }

        public static Task<IHtmlContent> PartialAsync(this IHtmlHelper htmlHelper, string partialViewName, object model, string prefix)
        {
            var viewData = new ViewDataDictionary(htmlHelper.ViewData);
            var htmlPrefix = viewData.TemplateInfo.HtmlFieldPrefix;
            viewData.TemplateInfo.HtmlFieldPrefix += !Equals(htmlPrefix, string.Empty) ? $".{prefix}" : prefix;
            return htmlHelper.PartialAsync(partialViewName, model, viewData);
        }

        public static Task<IHtmlContent> PartialAsync(this IHtmlHelper htmlHelper, string partialViewName, object model, string prefix, ViewDataDictionary viewDataDictionary)
        {
            var viewData = new ViewDataDictionary(viewDataDictionary);
            var htmlPrefix = viewData.TemplateInfo.HtmlFieldPrefix;
            viewData.TemplateInfo.HtmlFieldPrefix += !Equals(htmlPrefix, string.Empty) ? $".{prefix}" : prefix;
            return htmlHelper.PartialAsync(partialViewName, model, viewData);
        }
    }
}
