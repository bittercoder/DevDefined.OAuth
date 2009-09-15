using System.Web;

namespace ExampleProviderSite
{
  public static class OAuthServicesLocator
  {
    public static IOAuthServices Services
    {
      get { return (HttpContext.Current.ApplicationInstance as IOAuthServices); }
    }
  }
}