﻿using System.Net.Http.Json;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Forum.WebApi.Modules.Comment.DTOs;
using Forum.WebApi.Modules.Post.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.IntegrationTests;

public static class BaseEndpointTests 
{
    public static void Init()
    {
        //TODO Make init;
    }
    
    public static ForumDbContext GetDbContext(IServiceScope scope)
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ForumDbContext>();

        return dbContext;
    }
    
    public static Guid CreateUser(WebAppFactory<Program> factory)
    {
        var user = new User
        {
            Id = WebAppFactory<Program>.UserId,
            Email = "test@gmail.com",
            UserName = "test"
        };

        using var scope = factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);

        dbContext.Users.AddAsync(user);
        dbContext.SaveChanges();

        return user.Id;
    }
    
    public static async Task<HttpResponseMessage> CreateComment(HttpClient client, Guid postId, Guid? parentCommentId = null)
    {
        var url = $"/api/posts/{postId}/comments";
        if (parentCommentId is not null)
            url += $"/{parentCommentId}";
        
        var response = await client.PostAsJsonAsync<CommentDto>(url, new()
        {
            Body = "body"
        });

        return response;
    }
    
    public static async Task<HttpResponseMessage> CreatePost(HttpClient client)
    {
        var response = await client.PostAsJsonAsync<PostDto>("/api/posts", new()
        {
            Header = "header",
            Body = "body"
        });

        return response!;
    }
}
