using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using Dapper;
using System.Data;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
  private readonly DataContextDapper _dapper;
  private readonly ReusableSql _reusableSql;
  public UserCompleteController(IConfiguration config)
  {
    Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    _dapper = new DataContextDapper(config);
    _reusableSql = new ReusableSql(config);
  } 

  [HttpGet("TestConnection")]
  public DateTime TestConnection()
  {
    return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
  }

  [HttpGet("GetUsers/{userId}/{isActive}")]
  public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
  {
    string sql = @"EXEC TutorialAppSchema.spUsers_Get";
    string stringParameters = "";
    DynamicParameters sqlParameters = new DynamicParameters();

    if(userId != 0)
    {
      stringParameters += ", @UserId=@UserIdParameter";
      sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
    }

    if(isActive)
    {
      stringParameters += ", @Active=@ActiveParameter";
      sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
    }

    if(stringParameters.Length > 0)
    {
      sql += stringParameters.Substring(1);
    }

    IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
    return users;
  }

  [HttpPut("UpsertUser")]
  public IActionResult UpsertUser(UserComplete user) 
  {
    if (_reusableSql.UpsertUser(user))
    {
      return Ok();
    }

    throw new Exception("Failed to update user");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    string sql = @"EXEC TutorialAppSchema.spUser_Delete 
            @UserId = @UserIdParameter";

    DynamicParameters sqlParameters = new DynamicParameters();
    sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

    if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
    {
      return Ok();
    }

    throw new Exception("Failed to Delete user");
  }
}
