using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  DataContextDapper _dapper;
  public UserController(IConfiguration config)
  {
    Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    _dapper = new DataContextDapper(config);
  } 

  [HttpGet("TestConnection")]
  public DateTime TestConnection()
  {
    return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
  }

  [HttpGet("GetUsers")]
  public IEnumerable<User> GetUsers()
  {
    string sql = @"
          SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] 
          FROM TutorialAppSchema.Users";
    IEnumerable<User> users = _dapper.LoadData<User>(sql);
    return users;
  }

  [HttpGet("GetSingleUser/{userId}")]
  public User GetSingleUser(int userId)
  {
    string sql = @"
          SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] 
          FROM TutorialAppSchema.Users
          WHERE [UserId] = " + userId.ToString();
    User user = _dapper.LoadDataSingle<User>(sql);
    return user;
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

  [HttpGet("GetUserSalary/{userId}")]
  public UserSalary GetUserSalary(int userId)
  {
    string sql = @"
          SELECT [UserId],
            [Salary]
          FROM TutorialAppSchema.UserSalary
          WHERE [UserId] = " + userId.ToString();
    UserSalary userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
    return userSalary;
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
            " WHERE [UserId] = " + userSalary.UserId.ToString();
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
          WHERE [UserId] = " + userId.ToString();
    if (_dapper.ExecuteSql(sql))
    {
      return Ok();
    }

    throw new Exception("Failed to delete user salary");
  }
}
