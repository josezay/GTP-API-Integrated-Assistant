namespace CompanionAPI;

public static class ProjectConfigurator
{
    public static IApplicationBuilder ConfigureProject(this IApplicationBuilder app)
    {
        app.AddMiddlewares();

        app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

        app.UseAuthorization();


        return app;
    }

    private static IApplicationBuilder AddMiddlewares(this IApplicationBuilder app)
    {
        //app.UseMiddleware<ValidationMiddleware>();
        
        return app;
    }


}
