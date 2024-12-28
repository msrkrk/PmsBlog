using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;

namespace PmsBlog.Data;

// Add profile data for application users by adding properties to the PmsBlogUser class
public class PmsBlogUser : IdentityUser
{
    public List<UserTopic> UserTopics { get; set; }
}

