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
Common way of authenticating a user in web applications, Server generates `Access Token` for the user on successful logging in and stores it on (`Cache`, `Database` or any data storage), JWT Token is stateless **(does not need to keep a record of issued tokens in memory or a database)**, it holds information about itself as `Expiration time`, `Issuer` and `audience`, and may hold specific user information such as `ID`, `Role` and/or `custom claims`, This token is then sent by the client in the `Authorization` header of subsequent requests, allowing the server to verify the user's `identity`, `Role` and/or `permissions` without requiring their credentials on each request, and that is the difference point between `Bearer` and `Basic` Auth.

### Learning and implementing bearer authentication concept using `ASP.NET Core`
1. Installing package `Microsoft.AspNetCore.Authentication.JwtBearer`.
2. Initiating authentication scheme on application builder
    - By calling `.AddAuthentication()` from `builder` object we are allowed to call `.AddJwtBearer()` method.
    - Pass `JwtBearerDefaults.AuthenticationScheme` authentication scheme to `.AddJwtBearer()` as first argument avoiding hardcoding `"Bearer"`, and authentication options as second argument.
    - Set up JWT validations with `options.TokenValidationParameters = new TokenValidationParameters { // Your validation settings (issuer, audience, signing key, etc.) };`.
    - We can save the token itself in `AuthenticationProperties` of the `HttpContext` by setting `options.SaveToken = true` on token validation options, makes it possible to access the token later in the request lifecycle without having to retrieve it from the request headers again.
    - Adding `Authentication` and `Authorization` middlewares to the pipeline.
    - Ensuring that we are able to access user claims consistently (*as in `"/weatherforecast" endpoint`*), And makes our endpoints protected by validating user before allowing access.
3. Creating Authentication Service
    - Here I've created `JWTAuthService` class which contains `AuthenticateUser()` method, This method is resposible for validating login model and issuing the `Access token`.
    - Injected `JWTOptions` object instead of using `IConfiguration` for faster impelementing, and `List<User>` that represents dealing with `UserRepository` allowing me to register more than a user dynamically.
    - Included `User Guid` in the claims while generating the token descriptor.
4. Creating Auth Endpoints
    - Added post method endpoint(`"/api/Register"`) that interacts with username and password within user model and add it the the database (*adding to the `List<User>` that defined on `program.cs`*).
    - `/api/Auth` which validates login model ( *username and password* ) and generates the `Access token`.
    - made `/weatherforecast` endpoint protected by calling `.RequireAuthorization()` method (**Which require `UseAuthorization()` middleware added to the pipeline**).
### Notes while implementing
I faced a problem running the application because I did not added authorization service to the builder (`builder.Services.AddAuthorization()`), I had to explicitly mention it because on minimal API it does not assume any default authorization implementations.
