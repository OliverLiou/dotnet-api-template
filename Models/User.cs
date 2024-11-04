using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using TemplateApi.Interface;

namespace TemplateApi.Models
{
    public class UserAttribute : IdentityUser
    {
        public string? DepartmentId { get; set; }

        public required string EmployeeId { get; set; }

        public required string EmployeeName { get; set; }
    }

    public class User : UserAttribute
    {
        // [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // public int UserId { get; set; }

        // public required override string Id { get; set; }
    }

    public class UserLog : UserAttribute, ILogInterface
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int UserLogId { get; set; }

        public required string Method { get; set; }

        public required DateTime ExcuteTime { get; set; }

        public required string EditorName { get; set; }
    }
}