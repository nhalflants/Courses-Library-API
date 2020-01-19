using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    // [CourseTitleDescription(ErrorMessage = "The provided description should be different from the title")]
    public class CourseForCreationDto : CourseForManipulationDto // : IValidatableObject
    {
        /* [Required(ErrorMessage = "You should fill out a title")]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; } */

        /* public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description) {
                yield return new ValidationResult(
                    "The provided description should be different from the title", 
                    new[] { "CourseForCreationDto" });
            }
        }*/
    }
}