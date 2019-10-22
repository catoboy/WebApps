﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Admin
    {
        public int ID { get; set; }
        [DisplayName("Brukernavn")]
        [Required(ErrorMessage = "Fyll inn brukernavn")]
        public String Brukernavn { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Fyll inn passord")]
        public String Passord { get; set; }
        public string loginMsgError { get; set; }

    }
}