[x] Authentication & Authorization Concepts
[1] Basic Authentication:
    [*] Learning and impelementing basic authentication concept using `ASP.NET Core`
    [-] adding authentication scheme and handler
        - created directory (`Authentication`) that contains auth handler (`BasicAuthHandler.cs`) which inherents from `AuthenticationHandler` abstract class.
        - implemented basic auth concept inside of `HandleAuthenticateAsync`
        - configured Authentication in `program.cs` by adding authentication scheme to the builder

        