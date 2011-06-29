namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
  using DevDefined.OAuth.Framework;
  using DevDefined.OAuth.Provider.Inspectors;

  using Xunit;

  public class XAuthValidationInspectorTests
  {
    public bool ValidateXAuthMode(string authMode)
    {
      return authMode == "client_auth";
    }

    public bool AuthenticateXAuthUsernameAndPassword(string username, string password)
    {
      return username == "username" && password == "password";
    }

    [Fact]
    public void EmptyModeThrows()
    {
      var context = new OAuthContext { XAuthMode = string.Empty };

      var inspector = new XAuthValidationInspector(ValidateXAuthMode, this.AuthenticateXAuthUsernameAndPassword);
      var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.GetAccessTokenUsingXAuth, context));
      Assert.Equal("The x_auth_mode parameter must be present", ex.Message);
    }

 		[Fact]
    public void InvalidModeThrows()
    {
      var context = new OAuthContext { XAuthMode = "test_mode" };

      var inspector = new XAuthValidationInspector(ValidateXAuthMode, this.AuthenticateXAuthUsernameAndPassword);
      var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.GetAccessTokenUsingXAuth, context));
      Assert.Equal("The x_auth_mode parameter is invalid", ex.Message);
    }

    [Fact]
    public void EmptyUsernameThrows()
    {
      var context = new OAuthContext { XAuthMode = "client_auth" };

      var inspector = new XAuthValidationInspector(ValidateXAuthMode, this.AuthenticateXAuthUsernameAndPassword);
      var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.GetAccessTokenUsingXAuth, context));
      Assert.Equal("The x_auth_username parameter must be present", ex.Message);
    }

    [Fact]
    public void EmptyPasswordThrows()
    {
      var context = new OAuthContext { XAuthMode = "client_auth", XAuthUsername = "username" };

      var inspector = new XAuthValidationInspector(ValidateXAuthMode, this.AuthenticateXAuthUsernameAndPassword);
      var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.GetAccessTokenUsingXAuth, context));
      Assert.Equal("The x_auth_password parameter must be present", ex.Message);
    }

    [Fact]
    public void AuthenticationFailureThrows()
    {
      var context = new OAuthContext { XAuthMode = "client_auth", XAuthUsername = "Joe", XAuthPassword = "Bloggs" };

      var inspector = new XAuthValidationInspector(ValidateXAuthMode, this.AuthenticateXAuthUsernameAndPassword);
      var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.GetAccessTokenUsingXAuth, context));
      Assert.Equal("Authentication failed with the specified username and password", ex.Message);
    }

    [Fact]
    public void AuthenticationPasses()
    {
      var context = new OAuthContext { XAuthMode = "client_auth", XAuthUsername = "username", XAuthPassword = "password" };

      var inspector = new XAuthValidationInspector(ValidateXAuthMode, this.AuthenticateXAuthUsernameAndPassword);
      Assert.DoesNotThrow(() => inspector.InspectContext(ProviderPhase.GetAccessTokenUsingXAuth, context));
    }
  }
}