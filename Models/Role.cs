using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TemplateApi.Models
{
    public class Role :IdentityRole
    {
        /// <summary>
        /// 角色名稱
        /// </summary>
        /// <value></value>
        [MaxLength(50)]
        public string RoleDesc { get; set; }
    }
}