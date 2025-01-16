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

  [HttpPut("EditUser")]
  public IActionResult EditUser(User user) 
  {
    string sql = @"
          UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName + 
                "',[LastName] = '" + user.LastName + 
                "', [Email] = '" + user.Email + 
                "', [Gender] = '" + user.Gender + 
                "',[Active]= '" + user.Active + 
            "' WHERE [UserId] = " + user.UserId;
    if (_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to update user");
  }

  [HttpPost]
  public IActionResult AddUser(UserToAddDto user) 
  {
    string sql = @"
      INSERT INTO TutorialAppSchema.Users(
        [FirstName],
        [LastName],
        [Email],
        [Gender],
        [Active]
      ) VALUES (" +
        "'" + user.FirstName + 
        "', '" + user.LastName + 
        "', '" + user.Email + 
        "', '" + user.Gender + 
        "', '" + user.Active + 
      "')";

    if(_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to add user");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    string sql = @"DELETE FROM TutorialAppSchema.Users 
                    WHERE UserId = " + userId.ToString();

    if(_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to Delete user");
  }

  [HttpGet("GetUserSalary/all")]
  public IEnumerable<UserSalary> GetUserSalaries()
  {
    string sql = @"
          SELECT [UserId],
            [Salary]
          FROM TutorialAppSchema.UserSalary";
    IEnumerable<UserSalary> userSalaries = _dapper.LoadData<UserSalary>(sql);
    return userSalaries;
  }

  [HttpPost("UserSalary")]
  public IActionResult PostUserSalary(UserSalary userSalary)
  {
    string sql = @"
          INSERT INTO TutorialAppSchema.UserSalary ([UserId], [Salary])
          VALUES (" + userSalary.UserId + ", " + userSalary.Salary + ")";
    if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
    {
      return Ok(userSalary);
    }

    throw new Exception("Failed to add user salary");
  }

  [HttpPut("EditUserSalary")]
  public IActionResult EditUserSalary(UserSalary userSalary) 
  {
    string sql = @"
          UPDATE TutorialAppSchema.UserSalary
            SET [Salary] = " + userSalary.Salary + 
            " WHERE [UserId] = " + userSalary.UserId;
    if (_dapper.ExecuteSql(sql))
    {
      return Ok(userSalary);
    }

    throw new Exception("Failed to update user salary");
  }

  [HttpDelete("UserSalary/{userId}")]
  public IActionResult DeleteUserSalary(int userId)
  {
    string sql = @"
          DELETE FROM TutorialAppSchema.UserSalary
          WHERE [UserId] = " + userId;

    if (_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to delete user salary");
  }

  [HttpPost("UserJobInfo")]
  public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
  {
    string sql = @"
          INSERT INTO TutorialAppSchema.UserJobInfo (
              [UserId], 
              [JobTitle], 
              [Department]
          ) VALUES (" + userJobInfoForInsert.UserId 
               + ", '" + userJobInfoForInsert.JobTitle 
               + "', '" + userJobInfoForInsert.Department 
               + "')";
    if (_dapper.ExecuteSql(sql))
    {
      return Ok(userJobInfoForInsert);
    }
    throw new Exception("Adding user job info failed on save");
  }

  [HttpPut("EditUserJobInfo")]
  public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
  {
    string sql = @"
          UPDATE TutorialAppSchema.UserJobInfo
          SET [JobTitle] = '" 
            + userJobInfoForUpdate.JobTitle 
            + "', [Department] = '" 
            + userJobInfoForUpdate.Department 
            + "' WHERE [UserId] = " 
            + userJobInfoForUpdate.UserId;
    
    if (_dapper.ExecuteSql(sql))
    {
      return Ok(userJobInfoForUpdate);
    }
    throw new Exception("Updating user job info failed on save");
  }

  [HttpDelete("UserJobInfo/{userId}")]
  public IActionResult DeleteUserJobInfo(int userId)
  {
    string sql = @"
          DELETE FROM TutorialAppSchema.UserJobInfo
          WHERE [UserId] = " + userId;

    if (_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to delete user job info");
  }
}
