using System.Reflection.Metadata.Ecma335;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
  // [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class PostController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    public PostController(IConfiguration config)
    {
      _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
    public IEnumerable<Post> GetPosts(int postId, int userId, string searchParam = "None")
    {
      string sql = @"EXEC TutorialAppSchema.spPosts_Get";
      string parameters = "";

      if(postId != 0)
      {
        parameters += ", @PostId=" + postId.ToString();
      }
      if(userId != 0)
      {
        parameters += ", @UserId=" + userId.ToString();
      }
      if(searchParam != "None")
      {
        parameters += ", @SearchValue='" + searchParam + "'";
      }

      if(parameters.Length > 0)
      {
        sql += parameters.Substring(1);
      }

      return _dapper.LoadData<Post>(sql);
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
      string sql = @"SELECT 
        [PostId],
        [UserId],
        [PostTitle],
        [PostContent],
        [PostCreated],
        [PostUpdated]
       FROM TutorialAppSchema.Posts
        WHERE UserId =" + this.User.FindFirst("userId")?.Value;

      return _dapper.LoadData<Post>(sql);
    }

    [HttpPost("Post")]
    public IActionResult AddPost(PostToAddDto postToAdd)
    {
      string sql = @"
         INSERT INTO TutorialAppSchema.Posts(
              [UserId],
              [PostTitle],
              [PostContent],
              [PostCreated],
              [PostUpdated]) VALUES (" +
               this.User.FindFirst("userId")?.Value + 
               ",'" + postToAdd.PostTitle +
               "','" + postToAdd.PostContent +
               "', GETDATE(), GETDATE())";

      if(_dapper.ExecuteSql(sql))
      {
        return Ok();
      }

      throw new Exception("Failed to create new post");
    }

    [HttpPut("Post")]
    public IActionResult EditPost(PostToEditDto postToEdit)
    {
      string sql = @"
         UPDATE TutorialAppSchema.Posts SET  
              PostContent = '"  + postToEdit.PostContent + 
              "', PostTitle = '" + postToEdit.PostTitle + 
              @"', PostUpdated = GETDATE()
                WHERE PostId = " + postToEdit.PostId.ToString() +
                "AND UserId = " + this.User.FindFirst("userId")?.Value;

      if(_dapper.ExecuteSql(sql))
      {
        return Ok();
      }

      throw new Exception("Failed to edit post");
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletPost(int postId)
    {
      string sql = @"DELETE TUTORIALAPPSCHEMA.POSTS WHERE 
        PostId = " + postId.ToString() +
        "AND UserId = " + this.User.FindFirst("userId")?.Value;;

      if(_dapper.ExecuteSql(sql))
      {
        return Ok();
      }

      throw new Exception("Failed to delete post");
    }
  }
}