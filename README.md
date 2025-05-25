[x] Authentication & Authorization Concepts

[1] Basic Authentication:
    [*] Learning and impelementing basic authentication concept using `ASP.NET Core` [*]
    [-] adding authentication scheme and handler
        - created directory (`Authentication`) that contains auth handler (`BasicAuthHandler.cs`) which inherents from `AuthenticationHandler` abstract class.
        - implemented basic auth concept inside of `HandleAuthenticateAsync`
        - configured Authentication in `program.cs` by adding authentication scheme to the builder
    [-] creating protected controller
        - adding `GetProtectedForecast` to `WeatherForecastController` with `[Authorize]` attribute
        - protected EP will be (`/weatherforecast/protected`) and will return weatherforecast object
    [-] testing EP with postman.
    [-] notes while implementing:
        - I did not specified authentication type (scheme) on claimsIdentity object that lead to fail on authorizing me even with correct credentials.
        - must use `Encoding.UTF8.GetString` instead of using `.ToString()` to get string out from byte array.

        