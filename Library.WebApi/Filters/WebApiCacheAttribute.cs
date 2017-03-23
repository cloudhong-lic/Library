using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace Library.WebApi.Filters
{
	public class WebApiCacheAttribute : ActionFilterAttribute
	{
		public WebApiCacheAttribute(int durationSeconds)
		{
			DurationSeconds = durationSeconds;
			Private = true;
		}

		public int DurationSeconds { get; set; }
		public bool Private { get; set; }

		public override void OnActionExecuted(HttpActionExecutedContext filterContext)
		{
			if (filterContext.Response?.Headers != null)
				filterContext.Response.Headers.CacheControl = new CacheControlHeaderValue
				{
					MaxAge = TimeSpan.FromSeconds(DurationSeconds),
					MustRevalidate = true,
					Private = Private
				};
		}
	}
}