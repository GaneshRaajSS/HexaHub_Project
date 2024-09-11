using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;

using static Hexa_Hub.Models.MultiValues;


public class UserProfile
{
    [NotMapped]
    [Required]
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(55)]
    public string? UserName { get; set; }

    [Required]
    [EmailAddress]
    public string? UserMail { get; set; }

    [Required]
    public string? Gender { get; set; }

    [Required]
    public string? Dept { get; set; }

    [Required]
    public string? Designation { get; set; }

    [Required]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Address { get; set; }

    public byte[]? ProfileImage { get; set; }

    //Navigation Properties
    // 1- 1 Relation
    [JsonIgnore]
    public User? User { get; set; }

   
}
