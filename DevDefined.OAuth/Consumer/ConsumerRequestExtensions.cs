using System;
using System.Collections;
using System.Collections.Specialized;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Consumer
{
  public static class ConsumerRequestExtensions
  {
    public static IConsumerRequest Get(this IConsumerRequest request)
    {
      return request.ForMethod("GET");
    }

    public static IConsumerRequest Delete(this IConsumerRequest request)
    {
      return request.ForMethod("DELETE");
    }

    public static IConsumerRequest Put(this IConsumerRequest request)
    {
      return request.ForMethod("PUT");
    }

    public static IConsumerRequest Post(this IConsumerRequest request)
    {
      return request.ForMethod("POST");
    }

    static void ApplyParameters(NameValueCollection destination, object anonymousClass)
    {
      ApplyParameters(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
    }

    static void ApplyParameters(NameValueCollection destination, IDictionary additions)
    {
      if (additions == null) throw new ArgumentNullException("additions");

      foreach (string parameter in additions.Keys)
      {
        destination[parameter] = Convert.ToString(additions[parameter]);
      }
    }

    public static IConsumerRequest ForMethod(this IConsumerRequest request, string method)
    {
      request.Context.RequestMethod = method;
      return request;
    }

    public static IConsumerRequest ForUri(this IConsumerRequest request, Uri uri)
    {
      request.Context.RawUri = uri;
      return request;
    }

    public static IConsumerRequest ForUrl(this IConsumerRequest request, string url)
    {
      request.Context.RawUri = new Uri(url);
      return request;
    }

    public static IConsumerRequest WithFormParameters(this IConsumerRequest request, IDictionary dictionary)
    {
      ApplyParameters(request.Context.FormEncodedParameters, dictionary);
      return request;
    }

    public static IConsumerRequest WithFormParameters(this IConsumerRequest request, object anonymousClass)
    {
      ApplyParameters(request.Context.FormEncodedParameters, anonymousClass);
      return request;
    }

    public static IConsumerRequest WithQueryParameters(this IConsumerRequest request, IDictionary dictionary)
    {
      ApplyParameters(request.Context.QueryParameters, dictionary);
      return request;
    }

    public static IConsumerRequest WithQueryParameters(this IConsumerRequest request, object anonymousClass)
    {
      ApplyParameters(request.Context.QueryParameters, anonymousClass);
      return request;
    }

    public static IConsumerRequest WithCookies(this IConsumerRequest request, IDictionary dictionary)
    {
      ApplyParameters(request.Context.Cookies, dictionary);
      return request;
    }

    public static IConsumerRequest WithCookies(this IConsumerRequest request, object anonymousClass)
    {
      ApplyParameters(request.Context.Cookies, anonymousClass);
      return request;
    }

    public static IConsumerRequest WithHeaders(this IConsumerRequest request, IDictionary dictionary)
    {
      ApplyParameters(request.Context.Headers, dictionary);
      return request;
    }

    public static IConsumerRequest WithHeaders(this IConsumerRequest request, object anonymousClass)
    {
      ApplyParameters(request.Context.Headers, anonymousClass);
      return request;
    }

    public static IConsumerRequest AlterContext(this IConsumerRequest request, Action<IOAuthContext> alteration)
    {
      alteration(request.Context);
      return request;
    }

    public static T Select<T>(this IConsumerRequest request, Func<NameValueCollection, T> selectFunc)
    {
      try
      {
        return selectFunc(request.ToBodyParameters());
      }
      catch (ArgumentNullException)
      {
        throw Error.FailedToParseResponse(request.ToString());
      }
    }
  }
}