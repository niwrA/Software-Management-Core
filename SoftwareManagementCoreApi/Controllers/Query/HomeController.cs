using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.DirectoryServices.AccountManagement;

namespace SoftwareManagementCoreApi.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [Authorize]
  public class HomeController : Controller
  {
    public class UserInfoDto
    {
      public string Name { get; set; }
      public string AuthenticationType { get; set; }
      public string UserName { get; set; }
      public ICollection<GroupInfoDto> Groups { get; set; } = new List<GroupInfoDto>();
      public string GivenName { get; internal set; }
      public string AccountName { get; internal set; }
      public string Email { get; internal set; }
    }
    public class GroupInfoDto
    {
      public string Name { get; set; }
    }
    public class ADUserInfoHelper
    {
      public string[] GetGroupsForUser(UserInfoDto userInfoDto)
      {
        string[] output = null;

        //using (var ctx = new PrincipalContext(ContextType.Domain))
        //using (var user = UserPrincipal.FindByIdentity(ctx, userInfoDto.UserName))
        //{
        //  if (user != null)
        //  {
        //    // userInfoDto.Name = user.Name;
        //    userInfoDto.Name = user.DisplayName;
        //    userInfoDto.GivenName = user.GivenName;
        //    userInfoDto.AccountName = user.SamAccountName;
        //    userInfoDto.Email = user.EmailAddress;
        //    output = user.GetGroups() //this returns a collection of principal objects
        //        .Select(x => x.SamAccountName) // select the name.  you may change this to choose the display name or whatever you want
        //        .ToArray(); // convert to string array
        //  }
        //}

        return output;
      }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("userinfo")]
    public UserInfoDto GetUserInfo()
    {
      var userInfoDto = new UserInfoDto();
      if (User != null)
      {
        userInfoDto.AuthenticationType = User.Identity.AuthenticationType;
        userInfoDto.UserName = User.Identity.Name;
        var groups = new ADUserInfoHelper().GetGroupsForUser(userInfoDto);
        foreach(var group in groups)
        {
          userInfoDto.Groups.Add(new GroupInfoDto { Name = group });
        }
      }
      else
      {
        userInfoDto.AuthenticationType = "None";
      }
      return userInfoDto;
    }
  }
}