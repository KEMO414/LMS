﻿using Microsoft.AspNetCore.Builder;

namespace LMS.Application.Extentions
{
    public static class BuilderExtension
    {
        //public static void UseSwaggerConfiguration(this WebApplication app)
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}

        public static void UseCustomMiddlewares(this WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<PerformanceMiddleware>();

            app.MapControllers();

            app.UseCors("CorsPolicy");
        }
    }
}
