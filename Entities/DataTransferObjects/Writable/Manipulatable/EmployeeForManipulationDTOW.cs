﻿using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.Writable.Manipulatable
{
    public abstract class EmployeeForManipulationDTOW
    {
        [Required(ErrorMessage = "Employee name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the name is 30 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Age is a required field.")]
        [Range(18, int.MaxValue, ErrorMessage = "Age is required and it can't be lower than 18")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the position is 20 charcters.")]
        public string Position { get; set; }
    }
}