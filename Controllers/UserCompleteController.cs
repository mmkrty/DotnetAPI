using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
  DataContextDapper _dapper;
  public UserCompleteController(IConfiguration config)
  {
    Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    _dapper = new DataContextDapper(config);
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
    string parameters = "";

    if(userId != 0){
      parameters += ", @UserId=" + userId.ToString();
    }

    if(isActive){
      parameters += ", @Active=" + isActive.ToString();
    }

    sql += parameters.Substring(1);

    IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
    return users;
  }

  [HttpPut("UpsertUser")]
  public IActionResult EditUser(UserComplete user) 
  {
    string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName + 
            "', @LastName = '" + user.LastName + 
            "', @Email = '" + user.Email + 
            "', @Gender = '" + user.Gender + 
            "', @Active= '" + user.Active + 
            "', @JobTitle= '" + user.JobTitle + 
            "', @Department= '" + user.Department + 
            "', @Salary= '" + user.Salary + 
            "', @UserId = " + user.UserId;

    if (_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to update user");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    string sql = @"EXEC TutorialAppSchema.spUser_Delete 
            @UserId = " + userId.ToString();

    if(_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to Delete user");
  }
}
