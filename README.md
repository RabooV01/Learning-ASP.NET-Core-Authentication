# Authentication & Authorization Concepts

## Basic Authentication
weakest way of authenticating a user, since we are sending the credentials everytime on each request headers.
a common way is that a request contains a header "Authorization" formatted as "[authType] [base64-encoded-creds]"

### Learning and impelementing basic authentication concept using `ASP.NET Core`.
1. adding authentication scheme and handler
    - created directory (`Authentication`) that contains auth handler (`BasicAuthHandler.cs`) which inherents from `AuthenticationHandler` abstract class.
    - implemented basic auth concept inside of `HandleAuthenticateAsync`.
    - configured Authentication in `program.cs` by adding authentication scheme to the builder.
2. creating protected controller
    - adding `GetProtectedForecast` to `WeatherForecastController` with `[Authorize]` attribute
    - protected EP will be (`/weatherforecast/protected`) and will return weatherforecast object
3. testing EP with postman with hardcoded credentials.

### Notes while implementing
I did not specified authentication type (scheme) on `claimsIdentity` object that lead to fail on authorizing me even with correct credentials.
must use `Encoding.UTF8.GetString` instead of using `.ToString()` to get string out from byte array.

## Bearer Authentication (JWT)
Common way of authenticating a user in web applications, Server generates `Access Token` for the user on successful logging in and stores it on (`Cache`, `Database` or any data storage), JWT Token holds information about itself as `Expiration time`, `Issuer and audience` and may hold specific user information such as (`User ID`, `Role` and/or `custom claims`), This token is then sent by the client in the `Authorization` header of subsequent requests, allowing the server to verify the user's `identity`, `Role` and/or permissions without requiring their credentials on each request, and that is the difference point between `Bearer` and `Basic` Auth.