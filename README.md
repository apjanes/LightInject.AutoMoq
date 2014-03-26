LightInject.AutoMoq
===================
This project adds auto mocking functionality to the LightInject 
Inversion of Control Container (http://www.lightinject.net/)
using Moq mock objects.

In tests it is useful to be able to resolve instances of classes
under test and have all their dependencies automatically resolved
as mocks.  In addition, the mocks themselves can be retrieved from
the container to allow addition of setup or verification on those
mocks that are relevant to the test.

Why not just use AutoMoq?
-------------------------
There is already a project by Darren Cauthon (https://github.com/darrencauthon)
called AutoMoq (https://github.com/darrencauthon/AutoMoq) which provides similar
auto mocking functionality using it's own custom container.  

In many cases, this might be fine, however, I have found that sometimes it is useful 
to have the mocking container as an extension of the actual IoC you use in your 
application (not just in tests).  If you are using LightInject in your application, 
using the LightInject.AutoMoq.MockingContainer will allow you to pass the container to 
functions that accept the real LightInject.ServiceContainer.

Example
-------
Given the following (rather contrived) Facebook news stream reader 
that authenticates using the IAuthenticationService and IConfiguration:

```c#
public interface IAuthenticationService {
	bool Authenticate(string username, string password);
}

public class FacebookNewsReader {
	IAuthenticationService _authenticationService;
	IConfiguration _configuration;
	public FacebookNewsReader(IAuthenticationService authenticationService,
							  IConfiguration configuration)
	{
		_authenticationService = authenticationService;
		_configuration = configuration;
	}
	
	public IEnumerable<NewsItem> ReadNews() 
	{
		var username = _configuration.UserName;
		var password = _configuration.Password;
		if(!_authenticationService.Authenticate(username, password)) 
		{
			throw new SecurityException("Login to Facebook failed");
		}
	}
}
```

it is conceivable that you would want to write tests to verify, for example, that
an exception is thrown if authentication fails.  Using Moq, it is easy to set up
the authentication service to mock both success and failure return values.  Without
using the MockingContainer you would typically have to create new mock instances for
both the authentication service and the configuration interfaces:

```c#
[Test]
public void TestTheAuthentication() 
{
	var authenticationService = new Mock<IAuthenticationService>();
	var configuration = new Mock<IConfiguration>();
	var reader = new FacebookNewsReader(authenticationService.Object, configuration.Object);
	
	// Test code here...
}
```

This can be cumbersome and result in lots of extra, unused mocks (such as the 
configuration mock) in your tests.  In addition, if you add another parameter to the
FacebookNewsReader, you have to refactor all your tests.

The MockingContainer makes it easy:

```c#
public void TestTheAuthentication() 
{
	var container = new MockingContainer();
	container.Register<FacebookNewsReader, FacebookNewsReader>();
	var reader = container.GetInstance<FacebookNewsReader>();
	
	// Test code here...
}
```

Using the GetMock method, setup of the relevant mocks can be performed:

```c#
public void TestTheAuthentication() 
{
	var container = new MockingContainer();
	container.GetMock<IAuthenticationService>()
	         .Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>())
			 .Returns(true);
	container.Register<FacebookNewsReader, FacebookNewsReader>();
	var reader = container.GetInstance<FacebookNewsReader>();
	
	// Test code here...
}
```

