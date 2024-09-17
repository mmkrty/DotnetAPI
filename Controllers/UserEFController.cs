using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using AutoMapper;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
  DataContextEF _entityFramework;
  IMapper _mapper;
  public UserEFController(IConfiguration config)
  {
    _entityFramework = new DataContextEF(config);
    _mapper = new Mapper(new MapperConfiguration(cfg => {
      cfg.CreateMap<UserToAddDto, User>();
    }));
  } 

  [HttpGet("GetUsers")]
  public IEnumerable<User> GetUsers()
  {
    IEnumerable<User> users = _entityFramework.Users.ToList<User>();
    return users;
  }

  [HttpGet("GetSingleUser/{userId}")]
  public User GetSingleUser(int userId)
  {
    User? user = _entityFramework.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault<User>();
    if(user != null)
    {
      return user;
    }
    
    throw new Exception("Failed to get user");
  }

  [HttpPut("EditUser")]
  public IActionResult EditUser(User user) 
  {
    User? userDb = _entityFramework.Users
                .Where(u => u.UserId == user.UserId)
                .FirstOrDefault<User>();

    if (userDb != null)
    {
      userDb.Active = user.Active;
      userDb.FirstName = user.FirstName;
      userDb.LastName = user.LastName;
      userDb.Email = user.Email;
      userDb.Gender = user.Gender;
      if(_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }

      throw new Exception("Failed to update user");
    }

    throw new Exception("Failed to update user");
  }

  [HttpPost]
  public IActionResult AddUser(UserToAddDto user) 
  {
    User userDb = _mapper.Map<User>(user);

    _entityFramework.Add(userDb);
    if(_entityFramework.SaveChanges() > 0)
    {
      return Ok();
    }

    throw new Exception("Failed to add user");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    User? userDb = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();
    
    if(userDb != null) 
    {
      _entityFramework.Users.Remove(userDb);
      if(_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }
      throw new Exception("Failed to Delete user");
    }

    throw new Exception("Failed to Delete user");
  }

  [HttpGet("UserSalary/{userId}")]
  public IEnumerable<UserSalary> GetUserSalaryEF(int userId)
  {
    return _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .ToList();
  }

  [HttpPost("UserSalary")]
  public IActionResult PostUserSalaryEF(UserSalary userSalaryForInsert)
  { 
    _entityFramework.UserSalary.Add(userSalaryForInsert);
    if(_entityFramework.SaveChanges() > 0)
    {
      return Ok();
    }
    throw new Exception("Adding user salary failed on save");
  }

  [HttpPut("UserSalary")]
  public IActionResult PutUserSalaryEF(UserSalary userSalaryForUpdate)
  {
    UserSalary? userSalaryToUpdate = _entityFramework.UserSalary
            .Where(u => u.UserId == userSalaryForUpdate.UserId)
            .FirstOrDefault();

    if(userSalaryToUpdate != null)
    {
      _mapper.Map(userSalaryForUpdate, userSalaryToUpdate);
      if(_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }
      throw new Exception("Failed to update user salary on save");
    }

    throw new Exception("Failed to find user salary to update");
  }

  [HttpDelete("UserSalary/{userId}")]
  public IActionResult DeleteUserSalaryEF(int userId)
  {
    UserSalary? userSalaryToDelete = _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

    if(userSalaryToDelete != null)
    {
      _entityFramework.UserSalary.Remove(userSalaryToDelete);
      if(_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }
      throw new Exception("Failed to delete user salary on save");
    }

    throw new Exception("Failed to find user salary to delete");
  }

  [HttpGet("UserJobInfo/{userId}")]
  public IEnumerable<UserJobInfo> GetUserJobInfoEF(int userId)
  {
    return _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .ToList();
  }

  [HttpPost("UserJobInfo")]
  public IActionResult PostUserJobInfoEF(UserJobInfo userJobInfoForInsert)
  {
    _entityFramework.UserJobInfo.Add(userJobInfoForInsert);
    if(_entityFramework.SaveChanges() > 0)
    {
      return Ok();
    }
    throw new Exception("Adding user job info failed on save");
  }

  [HttpPut("UserJobInfo")]
  public IActionResult PutUserJobInfoEF(UserJobInfo userJobInfoForUpdate)
  {
    UserJobInfo? userJobInfoToUpdate = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userJobInfoForUpdate.UserId)
            .FirstOrDefault();

    if(userJobInfoToUpdate != null)
    {
      _mapper.Map(userJobInfoForUpdate, userJobInfoToUpdate);
      if(_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }
      throw new Exception("Failed to update user job info on save");
    }

    throw new Exception("Failed to find user job info to update");
  }

  [HttpDelete("UserJobInfo/{userId}")]
  public IActionResult DeleteUserJobInfoEF(int userId)
  {
    UserJobInfo? userJobInfoToDelete = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

    if(userJobInfoToDelete != null)
    {
      _entityFramework.UserJobInfo.Remove(userJobInfoToDelete);
      if(_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }
      throw new Exception("Failed to delete user job info on save");
    }

    throw new Exception("Failed to find user job info to delete");
  }
}
