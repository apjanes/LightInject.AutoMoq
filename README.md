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
